using PermutationOptimiser.Algorithms;
using PermutationOptimiser.Model;

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

ISolver[] SolversFor(int n, DistanceMatrix? matrix) =>
    FilterSolvers(n, matrix) switch
    {
        { } solvers when n >= 4 && n <= (matrix is not null ? 80 : 80) =>
            solvers.Concat(ChristofidesSeededVariants(matrix)).ToArray(),
        var solvers => solvers,
    };

ISolver[] FilterSolvers(int n, DistanceMatrix? matrix)
{
    var all = AllSolvers();
    return n switch
    {
        <= 10 => all,
        <= 16 => all.Where(s => s is not BruteForceSolver).ToArray(),
        <= 18 => all.Where(s => s is not BruteForceSolver and not BranchAndBoundSolver).ToArray(),
        <= 40 => all.Where(s => s is not BruteForceSolver and not BranchAndBoundSolver and not HeldKarpSolver and not ChristofidesSolver { UseExactMatching: true }).ToArray(),
        _ => all.Where(s => s is not BruteForceSolver and not BranchAndBoundSolver and not HeldKarpSolver and not ChristofidesSolver { UseExactMatching: true }).ToArray(),
    };
}

ISolver[] ChristofidesSeededVariants(DistanceMatrix? matrix)
{
    if (matrix is null || matrix.Count < 3) return Array.Empty<ISolver>();
    try
    {
        var seedSolver = new ChristofidesSolver { UseExactMatching = matrix.Count <= 20 };
        var seed = seedSolver.Solve(matrix).Order.ToArray();
        return new ISolver[] { new TwoOptSolver { Seed = seed }, new ThreeOptSolver { Seed = seed }, new LinKernighanSolver { Seed = seed }, new IteratedLocalSearchSolver { Seed = seed } };
    }
    catch { return Array.Empty<ISolver>(); }
}

ISolver RecommendedFor(int n) => n switch { <= 10 => new BruteForceSolver(), <= 16 => new HeldKarpSolver(), <= 40 => new ChristofidesSolver { UseExactMatching = true }, _ => new LinKernighanSolver() };

void RunExample(string title, IReadOnlyList<Node> nodes, bool includeExact)
{
    Console.WriteLine("\n" + new string('=', 75) + "\n" + title + "  (" + nodes.Count + " nodes)\n" + new string('=', 75));
    var matrix = new DistanceMatrix(nodes);
    var solvers = includeExact ? AllSolvers().Concat(ChristofidesSeededVariants(matrix)).ToArray() : SolversFor(nodes.Count, matrix);
    double? optimal = null;
    var results = new List<SolverResult>();
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format("{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    Console.WriteLine("\nBest route found:"); var best = results.MinBy(r => r.Distance)!; Console.WriteLine("  " + best.RouteText(nodes));
}

ISolver[] FilterByName(ISolver[] solvers, string? name)
{
    if (name is null) return solvers;
    var filtered = solvers.Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || s.GetType().Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToArray();
    if (filtered.Length == 0) { Console.Error.WriteLine("No solver matched '" + name + "'."); Environment.Exit(1); }
    return filtered;
}

void PrintHelp()
{
    Console.WriteLine("PermutationOptimiser - 12 algorithms across 4 paradigms");
    Console.WriteLine("Usage: dotnet run [-- [options]]");
    Console.WriteLine("Options:");
    Console.WriteLine("  --cities <n>     Number of random cities to generate and solve");
    Console.WriteLine("  --seed <n>       RNG seed for reproducible city placement");
    Console.WriteLine("  --solver <name>  Run only solvers whose name contains <name> (case-insensitive)");
    Console.WriteLine("  --help, -h       Show this message");
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run                              # interactive prompt + demos");
    Console.WriteLine("  dotnet run -- --cities 10              # 10 random cities, all solvers");
    Console.WriteLine("  dotnet run -- --cities 8 --solver HeldKarp");
}

var argCities = 0; var argSeed = -1; string? argSolver = null; var showHelp = false;
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--cities" when i + 1 < args.Length: int.TryParse(args[++i], out argCities); break;
        case "--seed" when i + 1 < args.Length: int.TryParse(args[++i], out argSeed); break;
        case "--solver" when i + 1 < args.Length: argSolver = args[++i]; break;
        case "--help": case "-h": showHelp = true; break;
    }
}
if (showHelp) { PrintHelp(); return; }

if (argCities > 0)
{
    var rng = argSeed >= 0 ? new Random(argSeed) : new Random();
    var nodes = Enumerable.Range(0, argCities).Select(i => new Node("N" + i, rng.Next(0, 1000), rng.Next(0, 1000))).ToArray();
    var matrix = new DistanceMatrix(nodes); var solvers = FilterByName(SolversFor(argCities, matrix), argSolver);
    double? optimal = null; var results = new List<SolverResult>();
    Console.WriteLine("\n=== " + argCities + " random nodes" + (argSeed >= 0 ? " (seed " + argSeed + ")" : "") + " ===");
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format("{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    return;
}

Console.Write("Enter number of destinations (or press Enter to run demos): ");
var input = Console.ReadLine()?.Trim();
if (int.TryParse(input, out var n) && n > 0)
{
    var rng = new Random(); var nodes = Enumerable.Range(0, n).Select(i => new Node("N" + i, rng.Next(0, 1000), rng.Next(0, 1000))).ToArray();
    var matrix = new DistanceMatrix(nodes); var solvers = SolversFor(n, matrix);
    double? optimal = null; var results = new List<SolverResult>();
    Console.WriteLine("\n=== " + n + " random destinations ===");
    foreach (var solver in solvers) { var r = solver.Solve(matrix); results.Add(r); if (solver is HeldKarpSolver or BruteForceSolver) optimal ??= r.Distance; }
    Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10}{3,10}{4,14}", "Algorithm", "Paradigm", "Distance", "Gap", "Time"));
    Console.WriteLine(new string('-', 88));
    foreach (var r in results)
    {
        var gap = optimal is { } opt && opt > 0 ? string.Format("{0,7:0.0}%", (r.Distance - opt) / opt * 100) : "   n/a";
        Console.WriteLine(string.Format("{0,-38}{1,-14}{2,10:0.00}{3,10}{4,11:0.000} ms", r.Algorithm, r.Paradigm, r.Distance, gap, r.Elapsed.TotalMilliseconds));
    }
    Console.WriteLine("\nDone."); return;
}

if (string.IsNullOrEmpty(input))
{
    RunExample("Example 1 - Unit square (sanity check)", new[] { new Node("SW",0,0), new Node("SE",1,0), new Node("NE",1,1), new Node("NW",0,1) }, true);
    RunExample("Example 2 - 8 towns", new[] { new Node("Alpha",209,84), new Node("Bravo",189,175), new Node("Charlie",48,216), new Node("Delta",183,13), new Node("Echo",74,140), new Node("Foxtrot",150,122), new Node("Golf",203,195), new Node("Hotel",84,62) }, true);
    var rng = new Random(42);
    RunExample("Example 3 - 30 random nodes (heuristics only)", Enumerable.Range(0,30).Select(i => new Node("N" + i.ToString("00"), rng.Next(0,500), rng.Next(0,500))).ToArray(), false);
    Console.WriteLine("\nDone. See README.md for details.");
}
