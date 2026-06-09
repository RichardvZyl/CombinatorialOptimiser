namespace AssessmentPreparation.Model;

/// <summary>
/// An immutable node in the permutation graph. Coordinates are optional and
/// only meaningful when using the Euclidean DistanceMatrix constructor.
/// For non-geographic use cases (scheduling, wiring, routing) construct with
/// just a name: new Node("JobA").
/// </summary>
public readonly record struct Node(string Name, double X = 0, double Y = 0)
{
    public override string ToString() => Name;
}
