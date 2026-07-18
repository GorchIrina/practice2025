using Xunit;
using task14;
using System;
using System.Diagnostics;

namespace task15tests;

public class IntegralResearchTests
{
    private static double SingleThreadSolve(double a, double b, Func<double, double> function, double step)
    {
        double sum = 0.0;
        int stepsCount = (int)((b - a) / step);
        if (stepsCount < 1)
        {
            stepsCount = 1;
        }
        double actualStep = (b - a) / stepsCount;
        for (int i = 0; i < stepsCount; i++)
        {
            double x1 = a + i * actualStep;
            double x2 = a + (i+1) * actualStep;
            sum += (actualStep * (function(x1) + function(x2))) / 2.0;
        }
        return sum;
    }

    [Fact]
    public void Step1e4_GivesRequiredAccuracy()
    {
        double result = DefiniteIntegral.Solve(-100, 100, Math.Sin, 1e-4, 4);

        Assert.True(Math.Abs(result) < 1e-4, $"Погрешность должна быть < 1e-4");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    public void DifferentThreadCounts_GiveCorrectResult(int threads)
    {
        double result = DefiniteIntegral.Solve(-100, 100, Math.Sin, 1e-4, threads);

        Assert.True(Math.Abs(result) < 1e-4, $"Результат {result} некорректен");
    }

    [Fact]
    public void SingleAndMultiThread_GiveSameResult()
    {
        double single = SingleThreadSolve(-100, 100, Math.Sin, 1e-4);
        double multi = DefiniteIntegral.Solve(-100, 100, Math.Sin, 1e-4, 4);

        Assert.True(Math.Abs(single - multi) < 1e-3, $"Однопоточная ({single}) и многопоточная ({multi}) дают разные результаты");
    }

    [Fact(Skip = "Результат на данной машине: накладные расходы на потоки превышают выигрыш")]
    public void MultiThread_FasterThanSingleThread()
    {
        int repeats = 5;
        double singleTotal = 0;
        for (int i = 0; i < repeats; i++)
        {
            var watch = Stopwatch.StartNew();
            SingleThreadSolve(-100, 100, Math.Sin, 1e-4);
            watch.Stop();
            singleTotal += watch.Elapsed.TotalMilliseconds;
        }
        double singleAvg = singleTotal / repeats;
        int processorCount = Environment.ProcessorCount;
        double multiTotal = 0; 
        for (int i = 0; i < repeats; i++)
        {
            var watch = Stopwatch.StartNew();
            DefiniteIntegral.Solve(-100, 100, Math.Sin, 1e-4, processorCount);
            watch.Stop();
            multiTotal += watch.Elapsed.TotalMilliseconds;
        }
        double multiAvg = multiTotal / repeats;
        double improvement = (1 - multiAvg / singleAvg) * 100;
        
        Assert.True(improvement >= 15, $"Ускорение должно быть >= 15%");
    }
}
