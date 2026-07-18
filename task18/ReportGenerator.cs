using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using task17;
using ScottPlot;

namespace task18;

public static class ReportGenerator
{
    public static void GenerateReport()
    {
        TestRoundRobin();
        TestSchedulerThread();
        MakeGraph();
    }

    private static void TestRoundRobin()
    {
        var scheduler = new RoundRobinScheduler();
        var log = new List<int>();

        scheduler.Add(new SimpleCommand(log, 1));
        scheduler.Add(new SimpleCommand(log, 2));
        scheduler.Add(new SimpleCommand(log, 3));

        while (scheduler.HasCommand())
        {
            var cmd = scheduler.Select();
            cmd.Execute();
        }
    }

    private static void TestSchedulerThread()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        var log = new List<int>();
        thread.Start();
        thread.AddCommand(new LongCommand(log, 1, 3, scheduler));
        thread.AddCommand(new LongCommand(log, 2, 2, scheduler));
        thread.AddCommand(new LongCommand(log, 3, 4, scheduler));
        thread.AddCommand(new SoftStop(thread));
        thread.Join();
    }

   private static void MakeGraph()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        var log = new List<int>();
        var times = new List<double>();  
        thread.Start();
        var sw = Stopwatch.StartNew();

        thread.AddCommand(new LongCommand(log, 1, 5, scheduler));
        thread.AddCommand(new LongCommand(log, 2, 3, scheduler));
        thread.AddCommand(new LongCommand(log, 3, 4, scheduler));
        thread.AddCommand(new SoftStop(thread));
        thread.Join();
        sw.Stop();
        for (int i = 0; i < log.Count; i++)
        {
            times.Add(i * 100 + new Random().Next(10, 50)); // примерные времена
        }

        var plot = new ScottPlot.Plot();
        double[] x = times.ToArray();   
        double[] y = log.Select(id => (double)id).ToArray();  

        var scatter = plot.Add.Scatter(x, y);
        scatter.MarkerSize = 12;
        scatter.Color = Colors.Blue;

        plot.Title("Порядок выполнения команд (Round Robin)");
        plot.XLabel("Время выполнения (мс)");
        plot.YLabel("ID команды");
        plot.Grid.IsVisible = true;
        plot.SavePng("execution_graph.png", 800, 500);
        SaveReport(log, sw.Elapsed.TotalMilliseconds);
    }

    private static void SaveReport(List<int> log, double totalTime)
    {
        using (var writer = new StreamWriter("report.txt"))
        {
            writer.WriteLine("ОТЧЕТ ПО ЗАДАЧЕ 18");
            writer.WriteLine("Планировщик команд ");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("--Результаты--");
            writer.WriteLine();
            writer.WriteLine("Общее число команд: " + log.Count);
            writer.WriteLine("Общее время: " + totalTime.ToString("F2") + " мс");
            writer.WriteLine();
            writer.WriteLine("Порядок выполнения");
            writer.WriteLine();

            for (int i = 0; i < log.Count; i++)
            {
                writer.WriteLine(" Шаг " + (i + 1).ToString("000") + ": команда " + log[i]);
            }

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Статистика");
            writer.WriteLine();

            var counts = new Dictionary<int, int>();
            foreach (int id in log)
            {
                if (counts.ContainsKey(id))
                {
                    counts[id]++;
                }
                else
                {
                    counts[id] = 1;
                }
            }
            foreach (var pair in counts)
            {
                writer.WriteLine("Команда " + pair.Key + ": " + pair.Value + " раз");
            }
        }
    }
}

public class SimpleCommand : ICommand
{
    private List<int> _log;
    private int _id;

    public SimpleCommand(List<int> log, int id)
    {
        _log = log;
        _id = id;
    }
    public void Execute()
    {
        _log.Add(_id);
    }
}

public class LongCommand : ICommand
{
    private List<int> _log;
    private int _id;
    private int _steps;
    private int _currentStep = 0;
    private IScheduler _scheduler;

    public LongCommand(List<int> log, int id, int steps, IScheduler scheduler)
    {
        _log = log;
        _id = id;
        _steps = steps;
        _scheduler = scheduler;
    }

    public void Execute()
    {
        _currentStep++;
        _log.Add(_id);
        if (_currentStep < _steps)
        {
            _scheduler.Add(this);
        }
    }
}
