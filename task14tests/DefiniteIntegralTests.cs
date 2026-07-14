using Xunit;
using task14;

namespace task14tests;

public class DefiniteIntegralTests
{
    private readonly Func<double, double> X = (double x) => x;
    private readonly Func<double, double> Sin = (double x) => Math.Sin(x);

    [Fact]
    public void IntegralOfX1__ShouldReturnZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void IntegralOfSin__ShouldReturnZero()
    {
        double result = DefiniteIntegral.Solve(-1, 1, Sin, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void IntegralOfX2_OnZeroToFive_ShouldReturnTwelvePointFive()
    {
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-5);
    }

    [Fact]
    public void InvalidInterval_ThrowsArgumentException()
    {
        Action act = ()=> DefiniteIntegral.Solve(2, 1, X, 1e-4, 5);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void NullFunction_ThrowsArgumentNullException()
    {
        Action act = ()=> DefiniteIntegral.Solve(0, 1, null!, 1e-4, 5);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void NegativeStep_ThrowsArgumentException()
    {
        Action act = ()=> DefiniteIntegral.Solve(0, 1, X, -1e-4, 5);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ZeroThreads_ThrowsArgumentException()
    {
        Action act = ()=> DefiniteIntegral.Solve(0, 1, X, 1e-4, 0);
        Assert.Throws<ArgumentException>(act);
    }
}

