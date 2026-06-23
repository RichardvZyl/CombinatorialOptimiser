using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

// Local search subroutine used as the move oracle by GeneticAlgorithmSolver and
// IteratedLocalSearchSolver. Combines 2-opt with candidate-list pruning (breaks the
// O(n²) scan once the nearest-neighbour distance exceeds the removed edge) and Or-opt
// segment relocations of length 1–3, which correspond to deeper sequential LK moves
// that pure 2-opt cannot reach. Both passes repeat until neither finds an improvement.
internal sealed class LinKernighanSolver : ISolver<DistanceMatrix, PermutationResult>
{
    private const int CandidateCount = 5;

    public string Name => "Lin-Kernighan (local search)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int[]? Seed { get; init; }

    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var tour = Seed != null ? (int[])Seed.Clone() : new NearestNeighborSolver().Solve(m).Order.ToArray();
            var n = tour.Length;
            if (n < 4) return tour;
            var neighbors = BuildNeighborLists(m, n);
            bool improved;
            do
            {
                improved  = TwoOptPass(tour, m, neighbors, n);
                improved |= OrOptPass(tour, m, n, 1);
                improved |= OrOptPass(tour, m, n, 2);
                improved |= OrOptPass(tour, m, n, 3);
            } while (improved);
            return tour;
        });

    private static int[][] BuildNeighborLists(DistanceMatrix m, int n)
    {
        var k = Math.Min(CandidateCount, n - 1);
        var lists = new int[n][];
        for (var i = 0; i < n; i++)
            lists[i] = Enumerable.Range(0, n).Where(j => j != i).OrderBy(j => m[i, j]).Take(k).ToArray();
        return lists;
    }

    // Candidate-list 2-opt: for each edge (t1,t2), only test t3 values that are in t1's
    // nearest-neighbour list. The list is sorted ascending, so we stop as soon as
    // d(t1,t3) >= d(t1,t2) — any further t3 can only make the gain worse.
    private static bool TwoOptPass(int[] tour, DistanceMatrix m, int[][] neighbors, int n)
    {
        var pos = new int[n];
        for (var i = 0; i < n; i++) pos[tour[i]] = i;
        var found = false;
        for (var i = 0; i < n; i++)
        {
            var t1 = tour[i]; var t2 = tour[(i + 1) % n]; var d12 = m[t1, t2];
            foreach (var t3 in neighbors[t1])
            {
                if (m[t1, t3] >= d12) break;
                var j = pos[t3]; var t4 = tour[(j + 1) % n];
                if (t4 == t1) continue;
                if (m[t1, t3] + m[t2, t4] < d12 + m[t3, t4] - 1e-10)
                {
                    // Reverse the segment between t2 (at i+1) and t3 (at j).
                    // If i+1 > j the segment wraps; reverse the complementary segment instead.
                    var i2 = (i + 1) % n;
                    int lo, hi;
                    if (i2 <= j) { lo = i2; hi = j; }
                    else { lo = j + 1; hi = i; }
                    PermutationUtils.Reverse(tour, lo, hi);
                    for (var p = 0; p < n; p++) pos[tour[p]] = p;
                    found = true; break;
                }
            }
        }
        return found;
    }

    // Or-opt: try relocating every consecutive segment of length segLen to the best
    // other position in the tour. For segLen > 1, also tries the reversed segment.
    // Restarts from the beginning after each successful relocation.
    private static bool OrOptPass(int[] tour, DistanceMatrix m, int n, int segLen)
    {
        if (n <= segLen + 2) return false;
        var found = false;
        var i = 0;
        while (i < n)
        {
            var prevNode  = tour[(i - 1 + n) % n];
            var firstNode = tour[i % n];
            var lastNode  = tour[(i + segLen - 1) % n];
            var nextNode  = tour[(i + segLen) % n];
            var removeSaving = m[prevNode, firstNode] + m[lastNode, nextNode] - m[prevNode, nextNode];

            var bestGain = 1e-10; var bestJ = -1; var bestReversed = false;
            for (var j = 0; j < n; j++)
            {
                if (IsInOrAdjacentToSegment(j, i, segLen, n)) continue;
                var a = tour[j]; var b = tour[(j + 1) % n];
                var saving = m[a, b] - m[a, firstNode] - m[lastNode, b];
                if (removeSaving + saving > bestGain) { bestGain = removeSaving + saving; bestJ = j; bestReversed = false; }
                if (segLen > 1)
                {
                    saving = m[a, b] - m[a, lastNode] - m[firstNode, b];
                    if (removeSaving + saving > bestGain) { bestGain = removeSaving + saving; bestJ = j; bestReversed = true; }
                }
            }

            if (bestJ >= 0) { Relocate(tour, n, i, segLen, bestJ, bestReversed); found = true; i = 0; }
            else i++;
        }
        return found;
    }

    // Exclude the position immediately before the segment (re-inserting there is a no-op)
    // and all positions within the segment itself.
    private static bool IsInOrAdjacentToSegment(int j, int segStart, int segLen, int n)
    {
        for (var k = -1; k < segLen; k++)
            if (j == ((segStart + k) % n + n) % n) return true;
        return false;
    }

    private static void Relocate(int[] tour, int n, int segStart, int segLen, int insertAfterIdx, bool reversed)
    {
        var segment = new int[segLen];
        for (var k = 0; k < segLen; k++) segment[k] = tour[(segStart + k) % n];
        if (reversed) Array.Reverse(segment);

        var inSeg = new bool[n];
        for (var k = 0; k < segLen; k++) inSeg[(segStart + k) % n] = true;

        var rest = new List<int>(n - segLen);
        for (var k = 0; k < n; k++) if (!inSeg[k]) rest.Add(tour[k]);

        var insertAfterNode = tour[insertAfterIdx];
        var insertPos = rest.IndexOf(insertAfterNode);
        rest.InsertRange(insertPos + 1, segment);
        rest.CopyTo(tour);
    }
}
