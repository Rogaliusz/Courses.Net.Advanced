using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Async.Samples
{
    internal class ProducerConsumerProblem
    {
        private readonly object _lock = new object();
        private readonly Queue<int> queue = new Queue<int>();

        public void ProduceProblem()
        {
            var producerConsumerProblem = new ProducerConsumerProblem();
            
            var t1 = new Task(
                () =>
                {
                    producerConsumerProblem.Produce(1);
                });
            t1.Start();

            producerConsumerProblem.Consume();
            
            var t2 = new Task(
                () =>
                {
                    for (var i = 0; i < 10; i++)
                    {
                        producerConsumerProblem.Produce(i);
                    }

                }
            );

            t2.Start();
        }

        public void Produce(int input)
        {
            lock (_lock)
            {
                queue.Enqueue(input);
                Monitor.Pulse(_lock);
            }
        }
 
        public int Consume()
        {
            lock (_lock)
            {
                while (!queue.Any())
                {
                    Monitor.Wait(_lock);
                }
 
                return queue.Dequeue();
            }
        }
        
    }
}