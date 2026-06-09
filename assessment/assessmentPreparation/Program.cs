using AssessmentPreparation.Algorithms;
using AssessmentPreparation.Model;

ISolver[] AllSolvers() => new ISolver[]
{
    new BruteForceSolver(),
    new BranchAndBoundSolver(),
    new HeldKarpSolver(),
    new NearestNeighborSolver(),
    new ChristofidesSolver { UseExactMatching = true },
    new ChristofidesSolver { UseExactMatching = false },
    new TwoOptSolver(),
    new ThreeOptSolver(),
    new LinKernighanSolver(),
    new IteratedLocalSearchSolver(),
    new SimulatedAnnealingSolver(),
    new GeneticAlgorithmSolver(),
};

Console.Write("Enter number of destinations: ");
var input = Console.ReadLine()?.Trim();
if (!int.TryParse(input, out var n) || n < 2) { Console.WriteLine("Running demo with 8 nodes."); n = 8; }

var rng = new Random(42);
var nodes = Enumerable.Range(0, n).Select(i => new Node("N" + i, rng.Next(0, 500), rng.Next(0, 500))).ToArray();
var matrix = new DistanceMatrix(nodes);
var solvers = n <= 10 ? AllSolvers() : AllSolvers().Where(s => s is not BruteForceSolver).ToArray();

double? optimal = null;
var results = new List<SolverResult>();
foreach (var solver in solvers)
{
    var r = solver.Solve(matrix);
    results.Add(r);
    if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance;
}

Console.WriteLine();
Console.WriteLine("=== " + n + " random nodes ===");
Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
Console.WriteLine(new string('-', 88));
foreach (var r in results)
{
    var gap = optimal is { } opt && opt > 0 ? string.Format("{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
    Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
}
Console.WriteLine();
Console.WriteLine("Done.");
