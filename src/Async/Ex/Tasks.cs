using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Shared.Services;

namespace Async.Ex
{
    public class Tasks
    {
        private readonly IUserService _userService = new UserService();
        
        public Tasks()
        {
            
        }
        
        // 1. Zamien zdarzenie na task
        public async Task<string> DownloadStringAsync()
        {
            var wc = new WebClient();
            var json = string.Empty;

            DownloadStringCompletedEventHandler onCompleted = null;
            onCompleted += (s, e) =>
            {
                json = e.Result;
            };

            wc.DownloadStringCompleted += onCompleted;
            wc.DownloadStringAsync(new Uri("https://jsonplaceholder.typicode.com/todos/1"));

            await Task.Delay(1000 * 10);
            wc.DownloadStringCompleted -= onCompleted;
            
            return json;
        }
        
        // 2. Zwiększ wydajność metody
        public async Task UpdateUsersAsync()
        {
            var users = await _userService.GetUsersAsync();

            foreach (var user in users)
            {
                user.Phone = $"+48 {user.Phone}";
                await _userService.UpdateUserAsync(user);
            }
        }
        
        // 3. Stworz najwydajniejsza asynchorniczna metode do drukowania akutalnego indeksu petli na ekranie
        private readonly int _size = 1000000;
        
        public void PrintIndex()
        {
            for (int i = 0; i < _size; i++)
            {
                
            }
        }

        // 4. Popraw dzialanie programu producer consumer tak by zakonczyl on swoje dzialanie poprawnie
        private Queue<int> _queue = new Queue<int>();
        private int _products = 10000;

        public async Task ProducesAndConsumeTasks(CancellationTokenSource tcs)
        {
            Task producer = Task.Run(async () =>
            {
                for (var i = 0; i < _products; i++)
                {
                    if (tcs.IsCancellationRequested)
                    {
                        Console.WriteLine("Work for producer ended");
                        return;
                    }

                    await Task.Delay(50);
                    Console.WriteLine($"Produced: {i} ");

                    while (_queue.Any())
                    {
                        await Task.Delay(50);
                    }

                    _queue.Enqueue(i);
                }

                tcs.Cancel();
            });

            Task consumer = Task.Run(async () =>
            {
                while (true)
                {
                    if (tcs.IsCancellationRequested && !_queue.Any())
                    {
                        Console.WriteLine("Work for consumer ended");
                        break;
                    }

                    while (_queue.Any() && _queue.TryDequeue(out var value))
                    {
                        Console.WriteLine($"Consumed {value}");
                    }

                    await Task.Delay(50);
                }
            });

            await Task.WhenAll(consumer, producer);
            Console.WriteLine("Work for 2 tasks was ended");
        }
        
        
    }
}