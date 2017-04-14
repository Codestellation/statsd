using System;
using System.Net.Sockets;
using Codestellation.Statsd.Builder;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// Contains specific settings for <see cref="UdpChannel"/>
    /// </summary>
    public class UdpChannelSettings
    {
        /// <summary>
        /// Statsd server host. Must be either valid ip address or dns name
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port a statsd server listens to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// <see cref="UdpChannel"/> will throw error if any <see cref="SocketException"/> happens.
        /// </summary>
        public bool IgnoreSocketExceptions { get; set; }

        /// <summary>
        /// Validates settings and throws an exception in case of invalid settings
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                throw new ArgumentException($"Expected a valid host name, but received '{Host ?? "<null>"}'", nameof(Host));
            }

            if (Port <= 0 || UInt16.MaxValue < Port)
            {
                throw new ArgumentOutOfRangeException(nameof(Port), $"Expected a number between 1 and {UInt16.MaxValue} but received {Port}");
            }
        }

        /// <summary>
        /// Created and instance of <see cref="UdpChannelSettings"/> class using uri string parameters
        /// </summary>
        /// <param name="uri">Uri encoded parameters</param>
        public static UdpChannelSettings Parse(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException();
            }
            if (uri.Scheme != "udp")
            {
                throw new ArgumentException($"Uri scheme must be udp but was {uri.Scheme}");
            }

            var values = uri.GetQueryValues();

            return new UdpChannelSettings
            {
                Host = uri.Host,
                Port = uri.Port,
                IgnoreSocketExceptions = values.ParseOrDefault(UriParseExtensions.IgnoreExceptions, onDefault: true)
            };
        }
    }
}