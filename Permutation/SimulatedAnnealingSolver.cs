using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Core.Metaheuristics;

namespace CombinatorialOptimiser.Permutation;

internal sealed class SimulatedAnnealingSolver : SimulatedAnnealingBase<DistanceMatrix, int[]>, ISolver<DistanceMatrix, PermutationResult>
{
    public string Name => "Simulated Annealing";
    public SolverParadigm Paradigm => SolverParadigm.Improvement;
    public PermutationResult Solve(DistanceMatrix m) =>
        SolverRunner.Timed(Name, Paradigm, m, () =>
        {
            var initial = new NearestNeighborSolver().Solve(m).Order.ToArray();
            return initial.Length < 4 ? initial : RunAnnealing(m, initial);
        });
    protected override double GetCost(DistanceMatrix m, int[] solution) => m.TourLength(solution);
    protected override int[] Clone(int[] solution) => (int[])solution.Clone();
    protected override double GetInitialTemperature(DistanceMatrix m, int[] initial, Random rng)
    {
        var n = initial.Length; var deltas = new List<double>(); var samples = Math.Min(1000, n * n);
        for (var s = 0; s < samples; s++) { var i = rng.Next(1, n); var k = rng.Next(1, n); if (i == k) continue; if (i > k) (i, k) = (k, i); var a = initial[i==0?n-1:i-1]; var b = initial[i]; var c = initial[k]; var d = initial[(k+1)%n]; if (a == c || b == d) continue; var delta = (m[a,c]+m[b,d])-(m[a,b]+m[c,d]); if (delta > 0) deltas.Add(delta); }
        return deltas.Count == 0 ? 1.0 : -deltas.Average() / Math.Log(0.8);
    }
    protected override double Step(DistanceMatrix m, int[] current, double temperature, Random rng)
    {
        var n = current.Length;
        var i = rng.Next(1, n); var k = rng.Next(1, n); if (i == k) return 0;
        if (i > k) (i, k) = (k, i);
        var a = current[i==0?n-1:i-1]; var b = current[i]; var c = current[k]; var d = current[(k+1)%n];
        if (a == c || b == d) return 0;
        var delta = (m[a,c]+m[b,d])-(m[a,b]+m[c,d]);
        if (delta < 0 || (temperature > 0 && rng.NextDouble() < Math.Exp(-delta / temperature))) { PermutationUtils.Reverse(current, i, k); return delta; }
        return 0;
    }
}
