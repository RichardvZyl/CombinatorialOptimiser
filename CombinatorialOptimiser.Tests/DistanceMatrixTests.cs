using CombinatorialOptimiser.Core;

namespace CombinatorialOptimiser.Tests;

public class DistanceMatrixTests
{
    private static readonly Node[] ThreeNodes = [new Node("A", 0, 0), new Node("B", 3, 4), new Node("C", 6, 0)];
    [Fact] public void IsSymmetric() { var m = new DistanceMatrix(ThreeNodes); for (var i = 0; i < 3; i++) for (var j = 0; j < 3; j++) Assert.Equal(m[i, j], m[j, i]); }
    [Fact] public void DiagonalIsZero() { var m = new DistanceMatrix(ThreeNodes); for (var i = 0; i < 3; i++) Assert.Equal(0.0, m[i, i]); }
    [Fact] public void KnownDistance_ThreeFourFiveTriangle() { var m = new DistanceMatrix([new Node("A",0,0),new Node("B",3,4)]); Assert.Equal(5.0, m[0,1], 9); }
    [Fact] public void TourLength_ClosedLoopRoundTrip() { var m = new DistanceMatrix([new Node("A",0,0),new Node("B",1,0)]); Assert.Equal(2.0, m.TourLength([0,1]), 9); }
    [Fact] public void EmptyNodes_Throws() { Assert.Throws<ArgumentException>(() => new DistanceMatrix(Array.Empty<Node>())); }
    [Fact] public void NullNodes_Throws() { Assert.Throws<ArgumentNullException>(() => new DistanceMatrix((IReadOnlyList<Node>)null!)); }
}
