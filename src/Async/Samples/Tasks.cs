using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Async.Ex
{
    public static class Tasks
    {
        private const int interval = 60 * 1000;
        
        public async static void Ex_1_Task()
        {
            var datetime = DateTime.Now;
            var tasks = new List<Task<string>>();
            
            for (var i = 0; i < 4000; i++)
            {
                tasks.Add(RefactorStringAsync("tasks", i));
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                Console.WriteLine(task.Result);
            }
            
            Console.WriteLine($"Completed {(DateTime.Now - datetime).TotalMilliseconds}");
        }

        private static async Task RunTaskAsync(string name, int i)
        {
            DoSomething(name, i); 
        }
        
        private static void RunTask(string name, int i)
        {
            Task.Run(() => { DoSomething("task", i); });
        }

        private static void DoSomething(string name, object i)
        {
            Console.WriteLine($"B {name}: {i}");
            Thread.Sleep(interval);
            Console.WriteLine($"A {name}: {i}");
        }
        
        private static Task DontRunTasksByThis(string name, int i)
        {
            return Task.Run(async () => await DoSomethingAsync(name, i));
        }
        
        private static async Task DoSomethingAsync(string name, object i)
        {
            Console.WriteLine($"B {name}: {i}");
            await Task.Delay(interval);
            Console.WriteLine($"A {name}: {i}");
        }

        private static async Task<string> RefactorStringAsync(string name, object i)
        {
            await Task.Delay(interval);
            return $"E {name}: {i}";
        }
    }
}