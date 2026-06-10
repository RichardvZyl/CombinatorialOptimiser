namespace CombinatorialOptimiser.Core;

public readonly record struct Node(string Name, double X = 0, double Y = 0)
{
    public override string ToString() => Name;
}
