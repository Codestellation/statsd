using System;
using Codestellation.Statsd.Channels;

namespace Codestellation.Statsd.Builder
{
    /// <summary>
    /// Provides a few factory methods to create an instance of <see cref="IStatsdClient"/> based on uri string configuration
    /// </summary>
    public static class BuildStatsd
    {
        /// <summary>
        /// Creates an instance of <see cref="IStatsdClient"/> using provided uri query parameters
        /// </summary>
        /// <example>
        /// <remarks>Default values for parameters: prefix=null, background=true, ignore_exceptions=true</remarks>
        /// <code>
        /// // the following strings are equivalent
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background&amp;ignore_exceptions"
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background=true&amp;ignore_exceptions=true"
        /// </code>
        /// </example>
        /// <param name="uri">Well formed uri string</param>
        public static IStatsdClient From(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                const string message = "Uri string should have format 'udp://host:port?prefix=my_prefix&background&ignore_exceptions'";
                throw new ArgumentException(message, nameof(uri));
            }
            return From(new Uri(uri));
        }

        /// <summary>
        /// Creates an instance of <see cref="IStatsdClient"/> using provided uri query parameters
        /// </summary>
        /// <example>
        /// <remarks>Default values for parameters: prefix=null, background=true, ignore_exceptions=true</remarks>
        /// <code>
        /// // the following strings are equivalent
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background&amp;ignore_exceptions"
        /// var uri = "udp://host:port?prefix=my_prefix&amp;background=true&amp;ignore_exceptions=true"
        /// </code>
        /// </example>
        /// <param name="uri">Well formed uri string</param>
        public static IStatsdClient From(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var channel = BuildChannel(uri);
            var values = uri.GetQueryValues();
            var background = values.ParseOrDefault(UriParseExtensions.Background, onDefault: true);
            var prefix = values.ParseOrDefault(UriParseExtensions.Prefix, null);

            if (background)
            {
                return new BackgroundStatsdClient(channel, prefix);
            }

            return new StatsdClient(channel, prefix);
        }

        private static IChannel BuildChannel(Uri uri)
        {
            if (uri.Scheme == "udp")
            {
                var settings = UdpChannelSettings.Parse(uri);

                return new UdpChannel(settings);
            }

            string message = $"Expected uri scheme to be either 'udp' or 'tcp' but received '{uri.Scheme}'";
            throw new ArgumentException(message, nameof(uri));
        }
    }
}