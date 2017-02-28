using System;
using System.Net.Sockets;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// A UDP channel to send data to a statsd server. This is a recommended channel.
    /// </summary>
    public class UdpChannel : IChannel, IDisposable
    {
        private readonly UdpClient _updClient;

        /// <summary>
        /// Creates a new instance of <see cref="UdpChannel"/> class
        /// </summary>
        /// <param name="hostname">Statsd server host. Must be either valid ip address or dns name</param>
        /// <param name="port">Port a statsd server listens to</param>
        public UdpChannel(string hostname, int port)
        {
            if (string.IsNullOrWhiteSpace(hostname))
            {
                throw new ArgumentException($"Expected a valid host name, but received '{hostname}'", nameof(hostname));
            }

            if (port <= 0 || UInt16.MaxValue < port)
            {
                throw new ArgumentOutOfRangeException(nameof(port), $"Expected a number between 1 and {UInt16.MaxValue} but received {port}");
            }

            _updClient = new UdpClient();
            _updClient.Client.Connect(hostname, port);
        }

        /// <inheritdoc />
        public void Send(byte[] buffer, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (count == 0)
            {
                return;
            }

            _updClient.Client.Send(buffer, count, SocketFlags.None);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _updClient?.Dispose();
        }
    }
}