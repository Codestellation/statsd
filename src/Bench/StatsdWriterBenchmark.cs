using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd.Bench
{
    [ClrJob, CoreJob, LegacyJitX64Job, RyuJitX64Job]
    public class StatsdWriterBenchmark
    {
        private readonly StatsdWriter _writer;
        private Metric _metric1;
        private Metric _metric2;
        private Metric _metric3;
        private Metric _metric4;
        private Metric _metric5;
        private Metric _metric6;
        private Metric _metric7;
        private Metric _metric8;

        public StatsdWriterBenchmark()
        {
            _writer = new StatsdWriter("test");
            _metric1 = new Metric(new Timing("time", 1));
            _metric2 = new Metric(new Timing("time", 12));
            _metric3 = new Metric(new Timing("time", 123));
            _metric4 = new Metric(new Timing("time", 1234));
            _metric5 = new Metric(new Timing("time", 12345));
            _metric6 = new Metric(new Timing("time", 123456));
            _metric7 = new Metric(new Timing("time", 1234567));
            _metric8 = new Metric(new Timing("time", 12345678));
        }

        [Benchmark(Baseline = true)]
        public void Write_metric_three_calls()
        {
            _writer.WriteName(_metric1.Name);
            _writer.WriteValue(_metric1.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric2.Name);
            _writer.WriteValue(_metric3.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric3.Name);
            _writer.WriteValue(_metric4.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric4.Name);
            _writer.WriteValue(_metric4.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric5.Name);
            _writer.WriteValue(_metric5.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric6.Name);
            _writer.WriteValue(_metric6.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric7.Name);
            _writer.WriteValue(_metric7.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric8.Name);
            _writer.WriteValue(_metric8.Value);
            _writer.WritePostfix('m', 's');

            _writer.Reset();
        }

        [Benchmark]
        public void Write_metric_three_calls2()
        {
            _writer.WriteName(_metric1.Name);
            _writer.WriteValue2(_metric1.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric2.Name);
            _writer.WriteValue2(_metric3.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric3.Name);
            _writer.WriteValue2(_metric4.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric4.Name);
            _writer.WriteValue2(_metric4.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric5.Name);
            _writer.WriteValue2(_metric5.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric6.Name);
            _writer.WriteValue2(_metric6.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric7.Name);
            _writer.WriteValue2(_metric7.Value);
            _writer.WritePostfix('m', 's');

            _writer.WriteName(_metric8.Name);
            _writer.WriteValue2(_metric8.Value);
            _writer.WritePostfix('m', 's');

            _writer.Reset();
        }

        [Benchmark]
        public void Write_metric_by_ref()
        {
            _writer.Write(ref _metric1);
            _writer.Write(ref _metric2);
            _writer.Write(ref _metric3);
            _writer.Write(ref _metric4);
            _writer.Write(ref _metric5);
            _writer.Write(ref _metric6);
            _writer.Write(ref _metric7);
            _writer.Write(ref _metric8);

            _writer.Reset();
        }
    }
}