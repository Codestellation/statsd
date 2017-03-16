using System;
using System.Net.Sockets;

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
    }
}