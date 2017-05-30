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
    }
}