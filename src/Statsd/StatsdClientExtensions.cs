namespace Codestellation.Statsd
{
    /// <summary>
    /// Provides a few metrics to simplify metrics logging and enable log methods chaining
    /// </summary>
    public static class StatsdClientExtensions
    {
        /// <summary>
        /// Sends count metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="self">An instance of the <see cref="IStatsdClient"/></param>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Number of times something happened</param>
        public static IStatsdClient LogCount(this IStatsdClient self, string name, int value)
        {
            self.LogCount(new Count(name, value));
            return self;
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="self">An instance of the <see cref="IStatsdClient"/></param>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Current level of something</param>
        public static IStatsdClient LogGauge(this IStatsdClient self, string name, int value)
        {
            self.LogGauge(new Gauge(name, value));
            return self;
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="self">An instance of the <see cref="IStatsdClient"/></param>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Time interval in milliseconds</param>
        public static IStatsdClient LogTiming(this IStatsdClient self, string name, int value)
        {
            self.LogTiming(new Timing(name, value));
            return self;
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="self">An instance of the <see cref="IStatsdClient"/></param>
        /// <param name="name">Name of a metric</param>
        /// <param name="stopwatch">An instance of <see cref="LeanStopwatch"/> which will be used to determine time interval</param>
        public static IStatsdClient LogTiming(this IStatsdClient self, string name, LeanStopwatch stopwatch)
        {
            self.LogTiming(stopwatch.Elapsed(name));
            return self;
        }
    }
}