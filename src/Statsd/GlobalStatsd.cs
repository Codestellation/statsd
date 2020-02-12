using System;
using System.Threading;
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
        /// Gets underlying instance of <see cref="IStatsdClient"/>. Avoid caching the property value anywhere else. 
        /// <remarks>
        /// Call <see cref="Configure(string)"/> or <see cref="Configure(System.Uri,System.Action{System.Exception})"/> before using the property. By default it returns stub implementation which simply swallows all supplied metrics.
        /// You can call <see cref="Dispose"/> to stop current implementation of <see cref="IStatsdClient"/> to prevent further metrics sending
        /// </remarks>
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
        /// <param name="exceptionHandler">Action to perform on an unhandled exception. Most commoly it's used for logging</param>
        public static void Configure(string uri, Action<Exception> exceptionHandler = null)
        {
            Configure(new Uri(uri), exceptionHandler);
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
        /// <param name="exceptionHandler">Action to perform on an unhandled exception. Most commoly it's used for logging</param>
        public static void Configure(Uri uri, Action<Exception> exceptionHandler = null)
        {
            _statsdClient = BuildStatsd.From(uri, exceptionHandler);
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
            _statsdClient.LogTiming(name, value);
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

        /// <summary>
        /// Disposes current instance of <see cref="Client"/> and replaces it with stub implementation
        /// </summary>
        public static void Dispose()
        {
#if NET40
            var client = _statsdClient;
            Thread.MemoryBarrier();
#else
            var client = Volatile.Read(ref _statsdClient);
#endif
            _statsdClient = new StubClient();
            (client as IDisposable)?.Dispose();
        }
    }
}