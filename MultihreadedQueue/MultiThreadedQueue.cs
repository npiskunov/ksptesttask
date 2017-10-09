using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadedQueue
{
    public class MultiThreadedQueue<T>
    {
        private readonly object _writeSync = new object();
        private readonly object _readSync = new object();
        private readonly int _maxCapacity;
        private readonly Queue<T> _innerQueue;
        private readonly Semaphore _semaphore;

        public MultiThreadedQueue()
        {
            _maxCapacity = int.MaxValue;
            //Leave default behaviour of dynamic queue length
            //(in case when queu length is not known)
            _innerQueue = new Queue<T>();
            _semaphore = new Semaphore(0, _maxCapacity);
        }

        public MultiThreadedQueue(int maxCapacity)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Queue must have at least one item");
            }
            _maxCapacity = maxCapacity;
            _innerQueue = new Queue<T>(maxCapacity);
            _semaphore = new Semaphore(0, maxCapacity);
        }

        public T Pop()
        {
            _semaphore.WaitOne();
            lock (_readSync)
            {
                return _innerQueue.Dequeue();
            }
        }

        public void Push(T value)
        {
            lock (_writeSync)
            {
                if (_innerQueue.Count == _maxCapacity)
                {
                    //If multithreaded queue is at capacity - throw an exception
                    throw new InvalidOperationException($"Queue is at capacity of {_innerQueue.Count} items");
                }
                _innerQueue.Enqueue(value);
                _semaphore.Release();
            }
        }
    }
}
