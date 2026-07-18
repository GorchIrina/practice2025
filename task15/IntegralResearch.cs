using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using ScottPlot; 
using task14;
using System.Collections.Generic;   

namespace task15;

public class IntegralResearch
{
    static void Main()
    {
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        int iterations = 5;  
        var stepResults = new List<(double Step, double Time, double Res)>();
        int testThreads = 4; 

        foreach (double step in steps)
        {
            double solution = 0;
            var stepTimes = new List<double>();
            for (int i = 0; i < iterations; i++)
            {
                var watch = Stopwatch.StartNew();
                solution = DefiniteIntegral.Solve(-100, 100, Math.Sin, step, testThreads);
                watch.Stop();
                stepTimes.Add(watch.Elapsed.TotalMilliseconds);
            }
            double avg=stepTimes.Average();
            stepResults.Add((step, avg, Math.Abs(solution-0))); 
        }

        var optimalStep=stepResults.Where(r => r.Res <= 1e-4).ToList().OrderBy(r => r.Time).First().Step;
        
        var threadResults = new List<(int Threads, double Time)>();
        int maxThreads = Environment.ProcessorCount;
        for (int threads = 1; threads <= maxThreads; threads++)
        {
            double time = 0;
            for (int i = 0; i < iterations; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegral.Solve(-100, 100, Math.Sin, optimalStep, threads);
                watch.Stop();
                time += watch.Elapsed.TotalMilliseconds;
            }
            double avg  = time/iterations;
            threadResults.Add((threads, avg));
        }
        
        
        int optimalThreads = threadResults.OrderBy(x => x.Time).First().Threads;
        double optimalTime = threadResults.OrderBy(x => x.Time).First().Time;


        double singleTotalTime = 0;
        for (int i = 0; i < (iterations*2); i++)  
        {
            var watch = Stopwatch.StartNew();
            double result = SingleThreadSolve(-100, 100, Math.Sin, optimalStep);  
            watch.Stop();
            singleTotalTime += watch.Elapsed.TotalMilliseconds;
        }
        double singleTime = singleTotalTime / (iterations * 2);

        double speed = singleTime / optimalTime;
        double improvement = (1 - optimalTime / singleTime) * 100;
        if (improvement<15)
        {
            Optim(-100, 100, Math.Sin, optimalStep, ref optimalThreads, ref optimalTime, ref improvement, iterations);
        }

        SaveResults(optimalStep, optimalThreads, singleTime, optimalTime, speed, improvement);
        SaveStepResults(stepResults);
        SaveThreadResults(threadResults);
        Graph(threadResults);
    }

    static void Graph(List<(int Threads, double Time)> threadResults)
    {
        var plt = new ScottPlot.Plot();
        
        var sorted = threadResults.OrderBy(x => x.Time).ToList();
        
        double[] xvalues = sorted.Select(x => x.Time).ToArray();
        double[] yvalues = sorted.Select(x => (double)x.Threads).ToArray();

        var scatter = plt.Add.Scatter(xvalues, yvalues);
        scatter.LegendText = "Время выполнения";
        scatter.Color = Colors.Blue;
        scatter.MarkerSize = 10;

        plt.Title("Зависимость времени выполнения от числа потоков");
        plt.XLabel("Время вычисления функции Solve (мс)");
        plt.YLabel("Количество потоков");
        plt.ShowLegend();
        plt.SavePng("thread_analysis.png", 800, 600);
    }

    static double SingleThreadSolve(double a, double b, Func<double, double> function, double step)
    {
        double sum = 0.0;
        int stepsCount = (int)((b - a)/ step);
        if (stepsCount < 1)
        {
            stepsCount = 1;
        }
        double newStep = (b - a)/ stepsCount;
        for (int i = 0; i < stepsCount; i++)
        {
            double x1 = a + i*newStep;
            double x2 = a + (i+1)*newStep;
            double trap = (newStep * (function(x1) + function(x2))) / 2.0;
            sum += trap;
        }
        return sum;
    }

    static void Optim(double a, double b, Func<double, double> function, double step, ref int optimalThreads, ref double optimalTime, ref double improvement, int iterations)
    {
        int maxThreads = Environment.ProcessorCount * 2;
        var threadResults = new List<(int Threads, double Time)>();

        for (int threads = 1; threads <= maxThreads; threads++)
        {
            double totalTime = 0;
            for (int i = 0; i < iterations; i++)
            {
                var watch = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, function, step, threads);
                watch.Stop();
                totalTime += watch.Elapsed.TotalMilliseconds;
            }

            double avg = totalTime / iterations;
            threadResults.Add((threads, avg));
        }

        var best = threadResults.OrderBy(x => x.Time).First();
        int newOptimalThreads = best.Threads;
        double newOptimalTime = best.Time;

        double singleTotalTime = 0;
        for (int i = 0; i < iterations*2; i++)
        {
            var watch = Stopwatch.StartNew();
            double result = SingleThreadSolve(a, b, function, step);
            watch.Stop();
            singleTotalTime += watch.Elapsed.TotalMilliseconds;
        }
        double singleTime = singleTotalTime / (iterations*2);

        double newImprovement = (1 - newOptimalTime / singleTime)*100;
        if (newOptimalTime < optimalTime)
        {
            optimalThreads = newOptimalThreads;
            optimalTime = newOptimalTime;
            improvement = newImprovement;
        }
    }

    static void SaveResults(double step, int threads, double singleTime, double multiTime, double speedup, double improvement)
    {
        using (var writer = new StreamWriter("results.txt"))
        {
            writer.WriteLine("---Результаты исследования---");
            writer.WriteLine();
            writer.WriteLine($"Функция sin(x) на отрезке [-100, 100]");
            writer.WriteLine($"Требуемая точность: 1e-4");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine($"-Оптимальные параметры-");
            writer.WriteLine();
            writer.WriteLine($"Оптимальный шаг: {step:E2}");
            writer.WriteLine($"Оптимальное количество потоков: {threads}");
            writer.WriteLine();
            writer.WriteLine($"Скорость оптимальной многопоточной версии:{multiTime:F2} мс");
            writer.WriteLine($"Скорость оптимальной однопоточной версии:{singleTime:F2} мс");
            writer.WriteLine();
            writer.WriteLine($"Ускорение: {speedup:F2}x");
            writer.WriteLine($"Улучшение на {improvement:F2}%");
            writer.WriteLine();
            if (improvement >= 15)
            {
                writer.WriteLine("Улучшение >= 15%, многопоточная версия эффективна");
            }
            else
            {
                writer.WriteLine("Улучшение < 15%: многопоточная версия не достаточно эффективна");
            }
        }
    }

    static void SaveStepResults(List<(double Step, double Time, double Res)> results)
    {
        using (var writer = new StreamWriter("step_results.csv"))
        {
            writer.WriteLine("Step,Time_ms,Error");
            foreach (var r in results)
            {
                writer.WriteLine($"{r.Step:E2},{r.Time:F2},{r.Res:E4}");
            }
        }
    }

    static void SaveThreadResults(List<(int Threads, double Time)> results)
    {
        using (var writer = new StreamWriter("thread_results.csv"))
        {
            writer.WriteLine("Threads,Time_ms");
            foreach (var r in results)
            {
                writer.WriteLine($"{r.Threads},{r.Time:F2}");
            }
        }
    }
}
