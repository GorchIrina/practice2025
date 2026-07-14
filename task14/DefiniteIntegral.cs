using System;
using System.Threading;
using System.Collections.Generic;

namespace task14;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        double totalSum = 0.0;
        
        if (a >= b)
        {
            throw new ArgumentException("Нижний предел не должен быть больше верхнего");
        }
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        } 
        if (step <= 0)
        {
            throw new ArgumentException("Шаг должен быть положительным", nameof(step));
        }
        if (threadsnumber <= 0)
        {
            throw new ArgumentException("Количество потоков должно быть положительным", nameof(threadsnumber));
        }    
        
        double len = b - a;
        double partLen = len / threadsnumber;

        List<Thread> threads = new List<Thread>();
        Barrier barrier = new Barrier(threadsnumber+1);

        for (int i=0; i < threadsnumber; i++)
        {
            double partLeft = a + i * partLen;
            double partRight = a + (i+1) * partLen;

            double localPartLeft = partLeft;
            double localPartRight = partRight;

            Thread thread = new Thread(()=>
            {
                try
                {
                    double partRes = TrapMethod(localPartLeft, localPartRight, function, step);
                    AtomAdd(ref totalSum, partRes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                finally
                {
                    barrier.SignalAndWait();
                }
            });

            threads.Add(thread);
            thread.Start();
        }
        barrier.SignalAndWait();

        return totalSum;
    }

    private static double TrapMethod(double partLeft, double partRight, Func<double, double> function, double step)
    {
        double sum = 0.0;

        int stepsCount = (int)((partRight - partLeft) / step);
        if (stepsCount < 1)
        {
            stepsCount =1;
        }
        double newStep = (partRight - partLeft) / stepsCount;

        for (int i = 0; i < stepsCount; i++)
        {
            double left = partLeft + i * newStep;        
            double right = partLeft + (i+1) * newStep;  
            
            double trap = (newStep * (function(left) + function(right))) / 2.0;
            sum += trap;
        }
        return sum;
    }

    private static void AtomAdd(ref double current, double value)
    {
        double curValue=0.0;
        double newValue=0.0;
        do
        {
            curValue = current;
            newValue = curValue + value;
        }
        while (curValue != Interlocked.CompareExchange(ref current, newValue, curValue));
    }

    
}
