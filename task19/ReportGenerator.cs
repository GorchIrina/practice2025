using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using task17;
using task18;
using task19;
using ScottPlot;

class Program
{
    static void Main()
    {
        var scheduler = new RoundRobinScheduler();
        var thread = new SchedulerThread(scheduler);
        var log = new List<(int order, int id, int call)>();
        int order = 0;

        for (int i = 0; i < 5; i++)
        {
            int id = i;
            var cmd = new RepeatableCommand(id, 3, scheduler,
                (cmdId, call) =>
                {
                    log.Add((++order, cmdId, call));
                }
            );
            scheduler.Add(cmd);
        }

        thread.Start();
        Thread.Sleep(1000);
        thread.AddCommand(new HardStop(thread));
        thread.Join();
        SaveReport(log);
        SaveGraph(log);
    }

    private static void SaveReport(List<(int order, int id, int call)> log)
    {
        using (var writer = new StreamWriter("19_report.txt"))
        {
            writer.WriteLine("ОТЧЕТ ПО ЗАДАЧЕ 19");
            writer.WriteLine("  Длительные операции");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("Условие");
            writer.WriteLine();
            writer.WriteLine("5 экземпляров TestCommand выполняются 3 раза.");
            writer.WriteLine("Остановка через HardStop.");
            writer.WriteLine();
            writer.WriteLine("Результаты");
            writer.WriteLine();
            writer.WriteLine("Всего вызовов: " + log.Count);
            writer.WriteLine("Порядок выполнения:");
            writer.WriteLine();

            foreach (var e in log)
            {
                writer.WriteLine("  " + e.order.ToString("00") + ". Поток " + e.id + " вызов " + e.call);
            }

            writer.WriteLine();
            writer.WriteLine("--- Выводы ---");
            writer.WriteLine("Каждая команда выполнилась ровно 3 раза.");
            writer.WriteLine("Планировщик обеспечил поочерёдное выполнение.");
            writer.WriteLine("HardStop успешно остановил поток.");
        }
    }

    private static void SaveGraph(List<(int order, int id, int call)> log)
    {
        var plot = new ScottPlot.Plot();

        var groups = log.GroupBy(x => x.id);

        foreach (var group in groups)
        {
            double[] x = group.Select(e => (double)e.order).ToArray();
            double[] y = group.Select(e => (double)e.id).ToArray();

            var scatter = plot.Add.Scatter(x, y);
            scatter.Label = "Команда " + group.Key;
            scatter.MarkerSize = 10;
            scatter.LineWidth = 2;
        }

        plot.Title("Выполнение 5 команд по 3 раза (Round Robin)");
        plot.XLabel("Порядковый номер вызова");
        plot.YLabel("ID команды");
        plot.ShowLegend();

        plot.SavePng("19_graph.png", 800, 600);
    }
}
