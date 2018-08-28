using System;
using System.Net.Sockets;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// A UDP channel to send data to a statsd server. This is a recommended channel.
    /// </summary>
    public class UdpChannel : IChannel, IDisposable
    {
        private readonly UdpChannelSettings _settings;
        private readonly UdpClient _udpClient;

        /// <summary>
        /// Creates a new instance of <see cref="UdpChannel"/> class
        /// </summary>
        /// <param name="settings">Udp channel settings</param>
        public UdpChannel(UdpChannelSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            settings.Validate();
            _settings = settings;

            _udpClient = new UdpClient();
        }

        /// <summary>
        /// Creates a new instance of <see cref="UdpChannel"/> class
        /// </summary>
        /// <param name="hostname">Statsd server host. Must be either valid ip address or dns name</param>
        /// <param name="port">Port a statsd server listens to</param>
        public UdpChannel(string hostname, int port)
            : this(new UdpChannelSettings { Host = hostname, Port = port })
        {
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
            
            if (TryConnect())
            {
                _udpClient.Client.Send(buffer, count, SocketFlags.None);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
#if NET40 || NET45
            _udpClient?.Close();
#else
            _udpClient?.Dispose();
#endif
        }

        private bool TryConnect()
        {
            if (_udpClient.Client.Connected)
            {
                return true;
            }
            try
            {
                _udpClient.Client.Connect(_settings.Host, _settings.Port);
                return true;
            }
            catch (SocketException e)
            {
                var message = $"UdpChannel was not able to send data to 'udp://{_settings.Host}:{_settings.Port}'. " +
                    "Possible reasons are invalid IP address or unresolvable DNS name.";
                if (!_settings.IgnoreSocketExceptions)
                {
                    throw new InvalidOperationException(message, e);
                }
            }

            return false;
        }
    }
}