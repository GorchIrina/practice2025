using Xunit;

public class CalculatorTests
{
    [Fact]
    public void Create_ShouldCreateCalcWithCorrectMethods()
    {
        var calc = @"
public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";
        var calcGenerate = new CalculatorFactory();
        var calculator = calcGenerate.CreateInstance<ICalculator>(calc);

        Assert.Equal(8, calculator.Add(6,2));
        Assert.Equal(4, calculator.Minus(6,2));
        Assert.Equal(12, calculator.Mul(6,2));
        Assert.Equal(3, calculator.Div(6,2));
    }

    [Fact]
    public void Create_DivZeroShouldThrowDivideByZeroException()
    {
        var calc = @"
public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";
        var calcGenerate = new CalculatorFactory();
        var calculator = calcGenerate.CreateInstance<ICalculator>(calc);
        
        Assert.Throws<DivideByZeroException>(() => calculator.Div(1,0));
    }

    [Fact]
    public void Create_IncorrectCodeShouldThrowException()
    {
        var calc = @"
public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
}";
        var calcGenerate = new CalculatorFactory();
        Assert.Throws<Exception>(() => calcGenerate.CreateInstance<ICalculator>(calc));
    }
}
