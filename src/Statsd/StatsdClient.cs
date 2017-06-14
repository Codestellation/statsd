using System;
using System.Collections.Generic;
using Codestellation.Statsd.Channels;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Sends metrics in a synchrounous manner. <remarks>This class is not thread safe. See <see cref="BackgroundStatsdClient"/> for thread safe</remarks>
    /// </summary>
    public class StatsdClient : IStatsdClient, IDisposable
    {
        private readonly StatsdWriter _writer;
        private readonly IChannel _channel;

        /// <summary>
        /// Creates a new instance of <see cref="StatsdClient"/> class.
        /// </summary>
        /// <param name="channel">A channel to send data to statsd server. Possible implementations are <see cref="UdpChannel"/> and <see cref="TcpChannel"/></param>
        /// <param name="prefix">Prefix which is will be for every metric.</param>
        public StatsdClient(IChannel channel, string prefix = null)
        {
            if (prefix != null && (string.IsNullOrWhiteSpace(prefix) || prefix.EndsWith(".")))
            {
                const string message = "Must be either null or period delimited string and not end with '.'." +
                    " For instance, 'my.favorite.prefix' is a right one.";
                throw new ArgumentException(message, nameof(prefix));
            }

            _writer = new StatsdWriter(prefix);
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        /// <inheritdoc />
        public void LogCount(Count count1)
        {
            LogMetric(count1);
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2)
        {
            LogMetric(count1, count2);
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2, Count count3)
        {
            LogMetric(count1, count2, count3);
        }

        /// <inheritdoc />
        public void LogCount<TCounts>(TCounts counts)
            where TCounts : IEnumerable<Count>
        {
            if (counts == null)
            {
                throw new ArgumentNullException(nameof(counts));
            }

            foreach (var count in counts)
            {
                if (_writer.ContainsData)
                {
                    _writer.WriteSeparator();
                }
                WriteMetric(count);
                if (_writer.MtuExceeded)
                {
                    Send();
                }
            }
            if (_writer.ContainsData)
            {
                Send();
            }
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge)
        {
            LogMetric(gauge);
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2)
        {
            LogMetric(gauge1, gauge2);
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2, Gauge gauge3)
        {
            LogMetric(gauge1, gauge2, gauge3);
        }

        /// <inheritdoc />
        public void LogGauge<TGauges>(TGauges gauges)
            where TGauges : IEnumerable<Gauge>
        {
            if (gauges == null)
            {
                throw new ArgumentNullException(nameof(gauges));
            }

            foreach (var gauge in gauges)
            {
                if (_writer.ContainsData)
                {
                    _writer.WriteSeparator();
                }
                WriteMetric(gauge);
                if (_writer.MtuExceeded)
                {
                    Send();
                }
            }
            if (_writer.ContainsData)
            {
                Send();
            }
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing)
        {
            LogMetric(timing);
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2)
        {
            LogMetric(timing1, timing2);
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2, Timing timing3)
        {
            LogMetric(timing1, timing2, timing3);
        }

        /// <inheritdoc />
        public void LogTiming<TTimings>(TTimings timings)
            where TTimings : IEnumerable<Timing>
        {
            if (timings == null)
            {
                throw new ArgumentNullException(nameof(timings));
            }

            foreach (var timing in timings)
            {
                if (_writer.ContainsData)
                {
                    _writer.WriteSeparator();
                }
                WriteMetric(timing);
                if (_writer.MtuExceeded)
                {
                    Send();
                }
            }
            if (_writer.ContainsData)
            {
                Send();
            }
        }

        private void Send()
        {
            _channel.Send(_writer.Buffer, _writer.Position);
            _writer.Reset();
        }

        private void LogMetric(Metric metric)
        {
            WriteMetric(metric);
            Send();
        }

        private void LogMetric(Metric metric1, Metric metric2)
        {
            WriteMetric(metric1);
            _writer.WriteSeparator();
            WriteMetric(metric2);

            Send();
        }

        /// <inheritdoc />
        private void LogMetric(Metric metric1, Metric metric2, Metric metric3)
        {
            WriteMetric(metric1);
            _writer.WriteSeparator();
            WriteMetric(metric2);
            _writer.WriteSeparator();
            WriteMetric(metric3);

            Send();
        }

        private void WriteMetric(Metric metric)
        {
            _writer.WriteName(metric.Name);
            _writer.WriteValue(metric.Value);
            _writer.WritePostfix(metric.Postfix);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            (_channel as IDisposable)?.Dispose();
        }
    }
}