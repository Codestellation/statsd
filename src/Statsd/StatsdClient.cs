using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Codestellation.Statsd.Channels;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Sends metrics in a synchrounous manner. <remarks>This class is not thread safe. See <see cref="BackgroundStatsdClient"/> for thread safe</remarks>
    /// </summary>
    public class StatsdClient : IStatsdClient
    {
        private readonly StatsdWriter _writer;
        private readonly IChannel _channel;

        /// <summary>
        /// Creates a new intance of <see cref="StatsdClient"/> class.
        /// </summary>
        /// <param name="channel">A channel to send data to statsd server. Possible implementations are <see cref="UdpChannel"/> and <see cref="TcpChannel"/></param>
        /// <param name="prefix">Prefix which is will be for every metric.</param>
        public StatsdClient(IChannel channel, string prefix = null)
        {
            if (channel == null)
            {
                throw new ArgumentNullException(nameof(channel));
            }
            if (prefix != null && (string.IsNullOrWhiteSpace(prefix) || prefix.EndsWith(".")))
            {
                const string message = "Must be either null or period delemited string and not end with '.'." +
                    " For instance, 'my.favourite.prefix' is a right one.";
                throw new ArgumentException(message, nameof(prefix));
            }

            _writer = new StatsdWriter(prefix);
            _channel = channel;
        }

        /// <inheritdoc />
        public void LogCount(Count count1)
        {
            Write(count1);
            Send();
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2)
        {
            Write(count1);
            _writer.WriteSeparator();
            Write(count2);

            Send();
        }

        /// <inheritdoc />
        public void LogCount(Count count1, Count count2, Count count3)
        {
            Write(count1);
            _writer.WriteSeparator();
            Write(count2);
            _writer.WriteSeparator();
            Write(count3);

            Send();
        }

        /// <inheritdoc />
        public void LogCount<TCounts>(TCounts counts) where TCounts : IEnumerable<Count>
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
                Write(count);
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
            Write(gauge);
            Send();
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2)
        {
            Write(gauge1);
            _writer.WriteSeparator();
            Write(gauge2);

            Send();
        }

        /// <inheritdoc />
        public void LogGauge(Gauge gauge1, Gauge gauge2, Gauge gauge3)
        {
            Write(gauge1);
            _writer.WriteSeparator();
            Write(gauge2);
            _writer.WriteSeparator();
            Write(gauge3);

            Send();
        }

        /// <inheritdoc />
        public void LogGauge<TGauges>(TGauges gauges) where TGauges : IEnumerable<Gauge>
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
                Write(gauge);
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
            Write(timing);
            Send();
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2)
        {
            Write(timing1);
            _writer.WriteSeparator();
            Write(timing2);

            Send();
        }

        /// <inheritdoc />
        public void LogTiming(Timing timing1, Timing timing2, Timing timing3)
        {
            Write(timing1);
            _writer.WriteSeparator();
            Write(timing2);
            _writer.WriteSeparator();
            Write(timing3);

            Send();
        }

        /// <inheritdoc />
        public void LogTiming<TTimings>(TTimings timings) where TTimings : IEnumerable<Timing>
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
                Write(timing);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(Timing timing)
        {
            _writer.WriteName(timing.Name);
            _writer.WriteValue(timing.Value);
            _writer.WritePostfix('m', 's');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(Gauge gauge)
        {
            _writer.WriteName(gauge.Name);
            _writer.WriteValue(gauge.Value);
            _writer.WritePostfix('g');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Send()
        {
            _channel.Send(_writer.Buffer, _writer.Position);
            _writer.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(Count count)
        {
            _writer.WriteName(count.Name);
            _writer.WriteValue(count.Value);
            _writer.WritePostfix('c');
        }
    }
}