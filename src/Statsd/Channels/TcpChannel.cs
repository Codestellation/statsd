using System;
using System.Net.Sockets;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// A channel to send statsd data using a tcp connection
    /// </summary>
    public class TcpChannel : IChannel, IDisposable
    {
        private readonly TcpClient _tcpClient;

        /// <summary>
        /// Creates a new instance of <see cref="TcpChannel"/> class
        /// </summary>
        /// <param name="hostname">Statsd server host. Must be either valid ip address or dns name</param>
        /// <param name="port">Port a statsd server listens to</param>
        public TcpChannel(string hostname, int port)
        {
            if (string.IsNullOrWhiteSpace(hostname))
            {
                throw new ArgumentException($"Expected a valid host name, but received '{hostname}'", nameof(hostname));
            }

            if (port <= 0 || UInt16.MaxValue < port)
            {
                throw new ArgumentOutOfRangeException(nameof(port), $"Expected a number between 1 and {UInt16.MaxValue} but received {port}");
            }

            _tcpClient = new TcpClient();
            _tcpClient.Client.Connect(hostname, port);
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

            _tcpClient.Client.Send(buffer, count, SocketFlags.None);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}