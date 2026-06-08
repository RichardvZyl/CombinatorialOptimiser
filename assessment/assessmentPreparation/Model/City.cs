namespace AssessmentPreparation.Model;

/// <summary>
/// An immutable city in the Travelling Salesman Problem.
/// Each city has a name and an optional (X, Y) coordinate for Euclidean distance.
/// </summary>
public readonly record struct City(string Name, double X = 0, double Y = 0)
{
    public override string ToString() => Name;
}
