using System;
using Codestellation.Statsd.Builder;
using Codestellation.Statsd.Internals;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Default static helpers methods to log metrics
    /// </summary>
    public static class GlobalStatsd
    {
        private static IStatsdClient _statsdClient = new StubClient();

        /// <summary>
        /// Gets underlying instance of <see cref="IStatsdClient"/>
        /// <remarks>Call <see cref="Configure(string)"/> or <see cref="Configure(Uri)"/> before using the property. By default it returns stub implementation which simply swallows all supplied metrics.</remarks>
        /// </summary>
        public static IStatsdClient Client => _statsdClient;

        /// <summary>
        /// Configures the property <see cref="Client"/> with appropriate implementation of <see cref="IStatsdClient"/>
        /// <remarks>Do not use synchronous <see cref="IStatsdClient"/> to avoid threading issues.</remarks>
        /// <remarks>Default values for parameters: prefix=null, background=true, ignore_exceptions=true</remarks>
        /// <code>
        /// // the following strings are equivalent
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background&amp;ignore_exceptions"
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background=true&amp;ignore_exceptions=true"
        /// </code>
        /// </summary>
        /// <param name="uri">Parameter of the statsd client </param>
        public static void Configure(string uri)
        {
            Configure(new Uri(uri));
        }

        /// <summary>
        /// Configures the property <see cref="Client"/> with appropriate implementation of <see cref="IStatsdClient"/>
        /// <remarks>Do not use synchronous <see cref="IStatsdClient"/> to avoid threading issues.</remarks>
        /// <remarks>Default values for parameters: prefix=null, background=true, ignore_exceptions=true</remarks>
        /// <code>
        /// // the following strings are equivalent
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background&amp;ignore_exceptions"
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background=true&amp;ignore_exceptions=true"
        /// </code>
        /// /// </summary>
        /// <param name="uri">Parameter of the statsd client </param>
        public static void Configure(Uri uri)
        {
            _statsdClient = BuildStatsd.From(uri);
        }

        /// <summary>
        /// Sends count metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Number of times something happened</param>    
        public static void LogCount(string name, int value)
        {
            _statsdClient.LogCount(name, value);
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Current level of something</param>
        public static void LogGauge(string name, int value)
        {
            _statsdClient.LogGauge(name, value);
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="name">Name of a metric</param>
        /// <param name="value">Time interval in milliseconds</param>
        public static void LogTiming(string name, int value)
        {
            _statsdClient.LogGauge(name, value);
        }

        /// <summary>
        /// Sends gauge metric into a <see cref="IStatsdClient"/>
        /// </summary>
        /// <param name="name">Name of a metric</param>
        /// <param name="stopwatch">An instance of <see cref="LeanStopwatch"/> which will be used to determine time interval</param>
        public static void LogTiming(string name, LeanStopwatch stopwatch)
        {
            _statsdClient.LogTiming(stopwatch.Elapsed(name));
        }
    }
}