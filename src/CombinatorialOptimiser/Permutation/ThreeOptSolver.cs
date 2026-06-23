using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

// Local search improvement that considers all triples of edges simultaneously, testing all
// valid reconnection patterns including the pure 3-opt double-reversal move. Stronger than
// 2-opt but O(n³) per pass. Seeds from Nearest Neighbor unless an external seed is provided.
internal sealed class ThreeOptSolver : ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "3-opt (local search)";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public int[]? Seed { get; init; }
    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var permutation = Seed ?? new NearestNeighborSolver().Solve(m).Order.ToArray(); var n = permutation.Length;
            if (n < 4) return permutation; var improved = true;
            while (improved) { improved = false; for (var a = 0; a < n - 1; a++) for (var b = a + 1; b < n; b++) for (var c = b + 1; c < n; c++) { var i1=a;var i2=(a+1)%n;var i3=b;var i4=(b+1)%n;var i5=c;var i6=(c+1)%n;var p1=permutation[i1];var p2=permutation[i2];var p3=permutation[i3];var p4=permutation[i4];var p5=permutation[i5];var p6=permutation[i6];if((m[p1,p3]+m[p2,p4])-(m[p1,p2]+m[p3,p4])<-1e-10){PermutationUtils.Reverse(permutation,i2,i3);improved=true;goto nextA;}if((m[p1,p5]+m[p2,p6])-(m[p1,p2]+m[p5,p6])<-1e-10){PermutationUtils.Reverse(permutation,i2,i5);improved=true;goto nextA;}if((m[p3,p5]+m[p4,p6])-(m[p3,p4]+m[p5,p6])<-1e-10){PermutationUtils.Reverse(permutation,i4,i5);improved=true;goto nextA;}if((m[p1,p4]+m[p3,p6]+m[p5,p2])-(m[p1,p2]+m[p3,p4]+m[p5,p6])<-1e-10){PermutationUtils.Reverse(permutation,i4,i5);PermutationUtils.Reverse(permutation,i2,i5);improved=true;goto nextA;}}nextA:;}
            return permutation;
        });
}
