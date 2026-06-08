namespace CombinatorialOptimiser.Core.Metaheuristics;

internal static class PermutationUtils
{
    internal static void Reverse(int[] order, int i, int k) { while (i < k) { (order[i], order[k]) = (order[k], order[i]); i++; k--; } }

    internal static int[] DoubleBridge(int[] p, Random rng) { var n = p.Length; var a = rng.Next(1, n/3); var b = rng.Next(n/3, 2*n/3); var c = rng.Next(2*n/3, n); var r = new int[n]; r[0] = p[0]; var pos = 1; for (var i = 1; i <= a; i++) r[pos++] = p[i]; for (var i = b+1; i <= c; i++) r[pos++] = p[i]; for (var i = a+1; i <= b; i++) r[pos++] = p[i]; for (var i = c+1; i < n; i++) r[pos++] = p[i]; return r; }
}
