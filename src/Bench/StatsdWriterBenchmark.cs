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
        private readonly byte[] _postfix;
        private readonly int _intPostfix;

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
            _postfix = new[] { (byte)'|', (byte)'m', (byte)'s' };

            _intPostfix = ((byte)'|') << 24 + ((byte)'m') << 16 + (byte)'s' << 8 + 3;
        }

        [Benchmark(Baseline = true)]
        public void Postfix_aggressive_inlining()
        {
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');
            _writer.WritePostfix('m', 's');

            _writer.Reset();
        }

        [Benchmark]
        public void Postfix_non_aggressive_inlining()
        {
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');
            _writer.WritePostfixPassive('m', 's');

            _writer.Reset();
        }

        [Benchmark]
        public void Postfix_array_cast_pointers_from_int()
        {
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.WritePostfixArrayCastPointer(_intPostfix);
            _writer.Reset();
        }

        [Benchmark]
        public void Postfix_aggressive_by_type()
        {
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);
            _writer.WritePostfix(Type.Timing);

            _writer.Reset();
        }
    }
}