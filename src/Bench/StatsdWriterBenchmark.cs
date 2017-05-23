using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd.Bench
{
    [ClrJob, CoreJob, LegacyJitX64Job, RyuJitX64Job]
    public class StatsdWriterBenchmark
    {
        private readonly StatsdWriter _writer;
        private Metric _metric;

        public StatsdWriterBenchmark()
        {
            _writer = new StatsdWriter("test");
            _metric = new Metric(new Timing("time", 814519243));
        }

        [Benchmark]
        public void Write_metric()
        {
            const string metricName = "time";

            _writer.WriteName(metricName);
            _writer.WriteValue(1);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(12);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(123);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(1234);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(12345);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(123456);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(1234567);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(metricName);
            _writer.WriteValue(12345678);
            _writer.WritePostfix('m', 's');

            _writer.Reset();
        }
    }
}