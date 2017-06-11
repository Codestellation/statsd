using System.Collections.Generic;

namespace Codestellation.Statsd.Internals
{
    internal class StubClient : IStatsdClient
    {
        public void LogCount(Count count1)
        {
        }

        public void LogCount(Count count1, Count count2)
        {
        }

        public void LogCount(Count count1, Count count2, Count count3)
        {
        }

        public void LogCount<TCounts>(TCounts counts)
            where TCounts : IEnumerable<Count>
        {
        }

        public void LogGauge(Gauge gauge)
        {
        }

        public void LogGauge(Gauge gauge1, Gauge gauge2)
        {
        }

        public void LogGauge(Gauge gauge1, Gauge gauge2, Gauge gauge3)
        {
        }

        public void LogGauge<TGauges>(TGauges gauges)
            where TGauges : IEnumerable<Gauge>
        {
        }

        public void LogTiming(Timing timing)
        {
        }

        public void LogTiming(Timing timing1, Timing timing2)
        {
        }

        public void LogTiming(Timing timing1, Timing timing2, Timing timing3)
        {
        }

        public void LogTiming<TTimings>(TTimings timings)
            where TTimings : IEnumerable<Timing>
        {
        }
    }
}