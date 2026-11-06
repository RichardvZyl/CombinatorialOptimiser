using System.Globalization;
using CombinatorialOptimiser.Core;
using CombinatorialOptimiser.Permutation;

// Beam search CLI options (parsed later)
var argBeamEnabled = false; var argBeamWidth = 4; double argBeamTemp = 1.0; string? argBeamRank = null;

ISolver<DistanceMatrix, PermutationResult>[] SolversFor(int n, DistanceMatrix? matrix)
{
    var solvers = SolverRegistry.AllPermutationSolvers(n, matrix).ToArray();
    if (argBeamEnabled)
    {
        var useLog = string.Equals(argBeamRank, "logprob", StringComparison.OrdinalIgnoreCase);
        var beam = new BeamSearchSolver(argBeamWidth, argBeamTemp, useLog);
        solvers = solvers.Concat(new[] { beam }).ToArray();
    }
    return solvers;
}

ISolver<DistanceMatrix, PermutationResult> RecommendedFor(int n) =>
    SolverRegistry.RecommendPermutation(n);

void RunExample(string title, IReadOnlyList<Node> nodes, bool includeExact)
{
    Console.WriteLine("\n" + new string('=', 75) + "\n" + title + "  (" + nodes.Count.ToString(CultureInfo.InvariantCulture) + " nodes)\n" + new string('=', 75));
    var matrix = new DistanceMatrix(nodes);
    var solvers = includeExact
        ? SolverRegistry.AllPermutationSolvers(nodes.Count, matrix).ToArray()
        : SolversFor(nodes.Count, matrix);
    double? optimal = null;
    var results = new List<PermutationResult>();
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format(CultureInfo.InvariantCulture, "{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    Console.WriteLine("\nBest route found:"); var best = results.MinBy(r => r.Distance)!; Console.WriteLine("  " + best.RouteText(nodes));
}

ISolver<DistanceMatrix, PermutationResult>[] FilterByName(ISolver<DistanceMatrix, PermutationResult>[] solvers, string? name)
{
    if (name is null) return solvers;
    var filtered = solvers.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || s.GetType().Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToArray();
    if (filtered.Length == 0) { Console.Error.WriteLine("No solver matched '" + name + "'."); Environment.Exit(1); }
    return filtered;
}

void PrintHelp()
{
    Console.WriteLine("CombinatorialOptimiser - 12 algorithms across 4 paradigms");
    Console.WriteLine("Usage: dotnet run [-- [options]]");
    Console.WriteLine("Options:");
    Console.WriteLine("  --cities <n>     Number of random cities to generate and solve");
    Console.WriteLine("  --seed <n>       RNG seed for reproducible city placement");
    Console.WriteLine("  --solver <name>  Run only solvers whose name contains <name> (case-insensitive)");
    Console.WriteLine("  --beam-width <n>  Enable BeamSearch solver with beam width n (default 4)");
    Console.WriteLine("  --beam-temp <t>   Temperature for beam softmax (default 1.0)");
    Console.WriteLine("  --beam-rank <m>   Final ranking mode: 'tour' (distance) or 'logprob' (accumulated log-prob)");
    Console.WriteLine("  --help, -h       Show this message");
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run                              # interactive prompt + demos");
    Console.WriteLine("  dotnet run -- --cities 10              # 10 random cities, all solvers");
    Console.WriteLine("  dotnet run -- --cities 8 --solver HeldKarp");
    Console.WriteLine();
    Console.WriteLine("Solver recommendations by problem size:");
    for (var sn = 4; sn <= 100; sn *= 2)
        Console.WriteLine($"  {sn,3} nodes \u2192 {RecommendedFor(sn).Name}");
    Console.WriteLine("  >100 nodes \u2192 Iterated Local Search or Lin-Kernighan");
}

var argCities = 0; var argSeed = -1; string? argSolver = null; var showHelp = false;
var argSeedChristofides = false;
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--cities" when i + 1 < args.Length: _ = int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out argCities); break;
        case "--seed" when i + 1 < args.Length: _ = int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out argSeed); break;
        case "--solver" when i + 1 < args.Length: argSolver = args[++i]; break;
        case "--beam-width" when i + 1 < args.Length: _ = int.TryParse(args[++i], NumberStyles.Integer, CultureInfo.InvariantCulture, out argBeamWidth); argBeamEnabled = true; break;
        case "--beam-temp" when i + 1 < args.Length: _ = double.TryParse(args[++i], NumberStyles.Float, CultureInfo.InvariantCulture, out argBeamTemp); argBeamEnabled = true; break;
        case "--beam-rank" when i + 1 < args.Length: argBeamRank = args[++i]; argBeamEnabled = true; break;
        case "--help": case "-h": showHelp = true; break;
        case "--seed-christofides": argSeedChristofides = true; break;
    }
}
if (showHelp) { PrintHelp(); return; }

if (argCities > 0)
{
    var rng = argSeed >= 0 ? new Random(argSeed) : new Random();
    var nodes = Enumerable.Range(0, argCities).Select(i => new Node("N" + i, rng.Next(0, 1000), rng.Next(0, 1000))).ToArray();
    var matrix = new DistanceMatrix(nodes);
    var solvers = FilterByName(SolversFor(argCities, matrix), argSolver);
    double? optimal = null; var results = new List<PermutationResult>();
    Console.WriteLine("\n=== " + argCities.ToString(CultureInfo.InvariantCulture) + " random nodes" + (argSeed >= 0 ? " (seed " + argSeed.ToString(CultureInfo.InvariantCulture) + ")" : "") + " ===");
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format(CultureInfo.InvariantCulture, "{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    return;
}

Console.Write("Enter number of destinations (or press Enter to run demos): ");
var input = Console.ReadLine()?.Trim();
if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n > 0)
{
    var rng = new Random(); var nodes = Enumerable.Range(0, n).Select(i => new Node("N" + i, rng.Next(0, 1000), rng.Next(0, 1000))).ToArray();
    var matrix = new DistanceMatrix(nodes); var solvers = SolversFor(n, matrix);
    double? optimal = null; var results = new List<PermutationResult>();
    Console.WriteLine("\n=== " + n.ToString(CultureInfo.InvariantCulture) + " random destinations ===");
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format(CultureInfo.InvariantCulture, "{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    Console.WriteLine("\nDone."); return;
}

if (string.IsNullOrEmpty(input))
{
    RunExample("Example 1 - Unit square (sanity check)", new[] { new Node("SW", 0, 0), new Node("SE", 1, 0), new Node("NE", 1, 1), new Node("NW", 0, 1) }, true);
    RunExample("Example 2 - 8 towns", new[] { new Node("Alpha", 209, 84), new Node("Bravo", 189, 175), new Node("Charlie", 48, 216), new Node("Delta", 183, 13), new Node("Echo", 74, 140), new Node("Foxtrot", 150, 122), new Node("Golf", 203, 195), new Node("Hotel", 84, 62) }, true);
    var rng = new Random(42);
    RunExample("Example 3 - 30 random nodes (heuristics only)", Enumerable.Range(0, 30).Select(i => new Node("N" + i.ToString("00", CultureInfo.InvariantCulture), rng.Next(0, 500), rng.Next(0, 500))).ToArray(), false);
    Console.WriteLine("\nDone. See README.md for details.");
}
