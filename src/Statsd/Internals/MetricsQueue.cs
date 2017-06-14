using System;
using System.Collections.Generic;
using System.Threading;

namespace Codestellation.Statsd.Internals
{
    internal class MetricsQueue
    {
        private readonly Queue<Metric> _queue;

        public MetricsQueue(int initialQueueSize)
        {
            if (initialQueueSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialQueueSize), initialQueueSize, "Must be not less than 0");
            }
            _queue = initialQueueSize > 0
                ? new Queue<Metric>(initialQueueSize)
                : new Queue<Metric>();
        }

        public void EnqueueCounts<TCounts>(TCounts counts) where TCounts : IEnumerable<Count>
        {
            lock (_queue)
            {
                foreach (Count count in counts)
                {
                    _queue.Enqueue(new Metric(count));
                }
                Monitor.PulseAll(_queue);
            }
        }

        public void EnqueueGauges<TGauges>(TGauges gauges) where TGauges : IEnumerable<Gauge>
        {
            lock (_queue)
            {
                foreach (Gauge gauge in gauges)
                {
                    _queue.Enqueue(new Metric(gauge));
                }
                Monitor.PulseAll(_queue);
            }
        }

        public void EnqueueTimings<TTimings>(TTimings timings) where TTimings : IEnumerable<Timing>
        {
            lock (_queue)
            {
                foreach (Timing timing in timings)
                {
                    _queue.Enqueue(new Metric(timing));
                }
                Monitor.PulseAll(_queue);
            }
        }

        public void Enqueue(Metric metric)
        {
            lock (_queue)
            {
                _queue.Enqueue(metric);
                Monitor.PulseAll(_queue);
            }
        }

        public void Enqueue(Metric metric1, Metric metric2)
        {
            lock (_queue)
            {
                _queue.Enqueue(metric1);
                _queue.Enqueue(metric2);
                Monitor.PulseAll(_queue);
            }
        }

        public void Enqueue(Metric metric1, Metric metric2, Metric metric3)
        {
            lock (_queue)
            {
                _queue.Enqueue(metric1);
                _queue.Enqueue(metric2);
                _queue.Enqueue(metric3);
                Monitor.PulseAll(_queue);
            }
        }

        public int DequeueInto(Metric[] batch)
        {
            lock (_queue)
            {
                var items = Math.Min(_queue.Count, batch.Length);

                if (items > 0)
                {
                    for (int i = 0; i < items; i++)
                    {
                        batch[i] = _queue.Dequeue();
                    }
                    return items;
                }

                Monitor.Wait(_queue, TimeSpan.FromMilliseconds(100));
                return 0;
            }
        }

        //NOTE this method mostly used to stop background worker faster. 
        public void Stop()
        {
            lock (_queue)
            {
                Monitor.PulseAll(_queue);
            }
        }
    }
}