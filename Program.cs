using System;
using System.Collections.Generic;
using System.Threading;

class SequenceSumThread
{
    private int threadId;
    private int step;
    private long sum;
    private int count;
    private volatile bool shouldStop;

    public Thread ExecutableThread { get; private set; }

    public int ThreadId => threadId;
    public int Step => step;
    public long Sum => sum;
    public int Count => count;

    public SequenceSumThread(int id, int step)
    {
        this.threadId = id;
        this.step = step;
        this.sum = 0;
        this.count = 0;
        this.shouldStop = false;
        this.ExecutableThread = new Thread(Run);
    }

    public void Start()
    {
        ExecutableThread.Start();
    }

    public void Join()
    {
        ExecutableThread.Join();
    }

    private void Run()
    {
        int current = 0;
        while (!shouldStop)
        {
            sum += current;
            current += step;
            count++;
            Thread.Sleep(10);
        }

        Console.WriteLine($"[Thread {threadId}]: Sum = {sum}, Terms Count =  {count}");
    }

    public void Stop()
    {
        shouldStop = true;
    }
}

class ThreadManager
{
    private List<SequenceSumThread> threads;
    private List<int> stopTimes;

    public ThreadManager()
    {
        threads = new List<SequenceSumThread>();
        stopTimes = new List<int>();
    }

    public void InputData()
    {
        Console.Write("Enter the number of threads: ");
        int numThreads = int.Parse(Console.ReadLine());

        for (int i = 0; i < numThreads; i++)
        {
            Console.WriteLine($"Parameters for thread  #{i + 1}:");
            Console.Write("Step: ");
            int step = int.Parse(Console.ReadLine());

            Console.Write(" Duration (in minutes): ");
            double durationMinutes = double.Parse(Console.ReadLine());

            int durationMilliseconds = (int)(durationMinutes * 60 * 1000);

            var thread = new SequenceSumThread(i + 1, step);
            threads.Add(thread);
            stopTimes.Add(durationMilliseconds);
        }
    }

    public void Breaker()
    {
        foreach (var thread in threads)
            thread.Start();

        Thread controlThread = new Thread(OperationTime);
        controlThread.Start();
        controlThread.Join();

        foreach (var thread in threads)
        { 
            thread.Join();
        }


        Console.WriteLine("All threads have completed.");
    }

    private void OperationTime()
    {
        for (int i = 0; i < threads.Count; i++)
        {
            int index = i; 
            new Thread(() =>
            {
                Thread.Sleep(stopTimes[index]);
                threads[index].Stop();
            }).Start();
        }
    }

}

class Program
{
    static void Main()
    {
        ThreadManager mainThread = new ThreadManager();
        mainThread.InputData();
        mainThread.Breaker();
    }
}
