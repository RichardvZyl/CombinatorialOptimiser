using AssessmentPreparation.Algorithms;
using AssessmentPreparation.Model;

// Generate a small random set of cities and compare brute force vs. greedy.
Console.Write("Enter number of cities (4-10 for brute force, or higher for NN): ");
var input = Console.ReadLine()?.Trim();
if (!int.TryParse(input, out var n) || n < 2)
{
    Console.WriteLine("Running default demo with 8 cities.");
    n = 8;
}

var rng = new Random(42);
var cities = Enumerable.Range(0, n)
    .Select(i => new City($"City {i}", rng.Next(0, 500), rng.Next(0, 500)))
    .ToArray();

var matrix = new DistanceMatrix(cities);

// Brute force is infeasible beyond ~10 cities
ITspSolver[] solvers = n <= 10
    ? [new BruteForceSolver(), new NearestNeighborSolver()]
    : [new NearestNeighborSolver()];

Console.WriteLine($"\n=== {n} random cities ===");
Console.WriteLine($"{"Algorithm",-30}{"Distance",12}{"Gap",10}{"Time",14}");
Console.WriteLine(new string('-', 68));

double? optimal = null;
foreach (var solver in solvers)
{
    var r = solver.Solve(matrix);
    if (optimal is null && solver is BruteForceSolver)
        optimal = r.Distance;
    var gap = optimal is { } opt && opt > 0
        ? $" {(r.Distance - opt) / opt * 100,+6:0.0}%"
        : "   n/a";
    Console.WriteLine($"{r.Algorithm,-30}{r.Distance,10:0.00}{gap,10}{r.Elapsed.TotalMilliseconds,11:0.000} ms");
}

Console.WriteLine("\nDone. Brute force is exact but O(n!); NN is fast but approximate.");
