using System;
using System.Collections.Generic;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Sends metrics to supplied channel using a background task. Metrics are send in batches.
    /// </summary>
    public class BackgroundStatsdClient : IStatsdClient, IDisposable
    {
        private readonly MetricsQueue _queue;
        private readonly SendWorker _sender;

        /// <summary>
        /// Creates new instance of <see cref="BackgroundStatsdClient"/> class.
        /// </summary>
        /// <param name="channel">Channel to send metrics to</param>
        /// <param name="prefix">Prefix which is will be for every metric.</param>
        /// <param name="initialQueueSize">Initial size of queue for background metrics</param>
        public BackgroundStatsdClient(IChannel channel, string prefix = null, int initialQueueSize = 10000)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }
            if (prefix != null && (string.IsNullOrWhiteSpace(prefix) || prefix.EndsWith(".")))
            {
                const string message = "Must be either null or period delemited string and not end with '.'." +
                    " For instance, 'my.favourite.prefix' is a right one";
                throw new ArgumentException(message, nameof(prefix));
            }
            _queue = new MetricsQueue(initialQueueSize);
            _sender = new SendWorker(_queue, channel, prefix);
        }

        /// <inheritdoc />
        public void LogCount(Count count1)
        {
            _queue.Enqueue(new Metric(count1));
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2)
        {
            _queue.Enqueue(new Metric(count1), new Metric(count2));
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2, Count count3)
        {
            _queue.Enqueue(new Metric(count1), new Metric(count2), new Metric(count3));
        }

        /// <inheritdoc />
        public void LogCount<TCounts>(TCounts counts) where TCounts : IEnumerable<Count>
        {
            if (counts == null)
            {
                throw new ArgumentNullException(nameof(counts));
            }
            _queue.EnqueueCounts(counts);
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge)
        {
            _queue.Enqueue(new Metric(gauge));
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2)
        {
            _queue.Enqueue(new Metric(gauge1), new Metric(gauge2));
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2, Gauge gauge3)
        {
            _queue.Enqueue(new Metric(gauge1), new Metric(gauge2), new Metric(gauge3));
        }

        /// <inheritdoc />
        public void LogGauge<TGauges>(TGauges gauges) where TGauges : IEnumerable<Gauge>
        {
            if (gauges == null)
            {
                throw new ArgumentNullException(nameof(gauges));
            }
            _queue.EnqueueGauges(gauges);
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing)
        {
            _queue.Enqueue(new Metric(timing));
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2)
        {
            _queue.Enqueue(new Metric(timing1), new Metric(timing2));
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2, Timing timing3)
        {
            _queue.Enqueue(new Metric(timing1), new Metric(timing2), new Metric(timing3));
        }

        /// <inheritdoc />
        public void LogTiming<TTimings>(TTimings timings) where TTimings : IEnumerable<Timing>
        {
            if (timings == null)
            {
                throw new ArgumentNullException(nameof(timings));
            }
            _queue.EnqueueTimings(timings);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _sender?.Dispose();
        }
    }
}