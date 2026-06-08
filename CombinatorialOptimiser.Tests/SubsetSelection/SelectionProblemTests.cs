using CombinatorialOptimiser.SubsetSelection;

namespace CombinatorialOptimiser.Tests.SubsetSelection;

public class SelectionProblemTests
{
    [Fact]
    public void NullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SelectionProblem(null!, 10));
    }

    [Fact]
    public void ZeroCapacity_Throws()
    {
        var items = new[] { new SelectionItem("A", 1, 1) };
        Assert.Throws<ArgumentOutOfRangeException>(() => new SelectionProblem(items, 0));
    }
}
