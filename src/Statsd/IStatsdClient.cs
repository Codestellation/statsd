using System.Collections.Generic;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Sends metrics to an instance of statsd server
    /// </summary>
    public interface IStatsdClient
    {
        /// <summary>
        /// Sends signle count metric into the channel
        /// </summary>
        void LogCount(Count count1);

        /// <summary>
        /// Sends two count metrics into the channel
        /// </summary>
        void LogCount(Count count1, Count count2);

        /// <summary>
        /// Sends three count metrics into the channel
        /// </summary>
        void LogCount(Count count1, Count count2, Count count3);

        /// <summary>
        /// Sends a batch of count metrics into the channel
        /// </summary>
        void LogCount<TCounts>(TCounts counts) where TCounts : IEnumerable<Count>;

        /// <summary>
        /// Sends signle gauge metric into the channel
        /// </summary>
        void LogGauge(Gauge gauge);

        /// <summary>
        /// Sends two gauge metrics into the channel
        /// </summary>
        void LogGauge(Gauge gauge1, Gauge gauge2);

        /// <summary>
        /// Sends three gauge metrics into the channel
        /// </summary>
        void LogGauge(Gauge gauge1, Gauge gauge2, Gauge gauge3);

        /// <summary>
        /// Sends a batch of gauge metrics into the channel
        /// </summary>
        void LogGauge<TGauges>(TGauges gauges) where TGauges : IEnumerable<Gauge>;

        /// <summary>
        /// Sends signle timing metric into the channel
        /// </summary>
        void LogTiming(Timing timing);

        /// <summary>
        /// Sends two timing metrics into the channel
        /// </summary>
        void LogTiming(Timing timing1, Timing timing2);

        /// <summary>
        /// Sends three timing metrics into the channel
        /// </summary>
        void LogTiming(Timing timing1, Timing timing2, Timing timing3);

        /// <summary>
        /// Sends a batch of timing metrics into the channel
        /// </summary>
        void LogTiming<TTimings>(TTimings timings) where TTimings : IEnumerable<Timing>;
    }
}