using System;
using System.Globalization;
using Codestellation.Statsd;
using Codestellation.Statsd.Internals;
using Xunit;

namespace Tests
{
    public class PerformanceTests
    {
        private readonly StatsdWriter _writer;
        private Metric _metric;

        public const int Iterations = 100_000_000;

        public PerformanceTests()
        {
            _writer = new StatsdWriter("test");
            _metric = new Metric(new Timing("time", 814519243));
        }

        [Fact]
        public void Should_serialize_data_fast()
        {
            _writer.Write(ref _metric);
            _writer.Reset();
            var sw = LeanStopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                _writer.Write(ref _metric);
                _writer.Reset();
            }
            var elapsed = sw.ElapsedTimeSpan;

            var opms = Iterations / elapsed.TotalMilliseconds;
            var message = $"Took {elapsed.TotalMilliseconds.ToString("N3", CultureInfo.InvariantCulture)}; {opms.ToString("N3", CultureInfo.InvariantCulture)} op/millisecond";
            throw new Exception(message);
        }
    }
}