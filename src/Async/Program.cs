using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Async.Samples;
using Shared.Services;
using Tasks = Async.Ex.Tasks;
using Timer = System.Timers.Timer;

namespace Async
{
    class Program
    {
        private static readonly DateTime _date = DateTime.UtcNow;
        private static readonly int _size = 100000;
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private static int _number;
        
        private static object _numberLock = new object();

        public static int Number
        {
            get { lock (_numberLock) return _number; }
            set { lock (_numberLock) _number = value; }
        }

        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var t1 = Task.Run(async () => { await DoAsync(); }, cts.Token);
            var t2 = Task.Run(async () => { await DoAsync(); }, cts.Token);
            var t3 = Task.Run(async () => { await DoAsync(); }, cts.Token);
            var t4 = Task.Run(async () => { await DoAsync(); }, cts.Token);
            
            Console.ReadLine();
            cts.Cancel();
            Console.ReadLine();
        }

        private static async Task DoAsync()
        {
            while (true)
            {
                lock (_numberLock)
                {
                    _number++;
                    Console.WriteLine(_number);
                }
                
                await Task.Delay(500);
            }
        }

        private static void ThreadSafeInc()
        {
            lock (_numberLock)
            {
                _number++;
                Console.WriteLine(_number);
            }
        }

        private static void DoSometing(CancellationToken cts)
        {
            var timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (sender, eventArgs) =>
            {
                var ti = (Timer) sender;
                Console.WriteLine(eventArgs.SignalTime);

                if (cts.IsCancellationRequested)
                {
                    ti.Stop();
                    ti.Dispose();
                }
            };
            
            timer.Start();
        }
    }
}