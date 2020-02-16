using System;
using System.Net;
using System.Net.Sockets;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// A UDP channel to send data to a statsd server. This is a recommended channel.
    /// </summary>
    public class UdpChannel : IChannel, IDisposable
    {
        private readonly UdpChannelSettings _settings;
        private IPAddress[] _addresses;

        private long _nextDnsCheckAt;
        private Socket _udpSocket;

        /// <summary>
        /// Creates a new instance of <see cref="UdpChannel" /> class
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
            _addresses = new IPAddress[0];
        }

        /// <summary>
        /// Creates a new instance of <see cref="UdpChannel" /> class
        /// </summary>
        /// <param name="hostname">Statsd server host. Must be either valid ip address or dns name</param>
        /// <param name="port">Port a statsd server listens to</param>
        public UdpChannel(string hostname, int port)
            : this(new UdpChannelSettings
            {
                Host = hostname,
                Port = port,
                DnsUpdatePeriod = 10
            })
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
                _udpSocket.Send(buffer, count, SocketFlags.None);
            }
        }

        private bool TryConnect()
        {
            try
            {
                if (CheckDnsChanges())
                {
                    InitSocket();
                }

                return _udpSocket?.Connected ?? false;
            }
            catch (SocketException e)
            {
                HandleException(e);
            }

            return false;
        }

        private bool CheckDnsChanges()
        {
            if (Environment.TickCount < _nextDnsCheckAt)
            {
                return false;
            }

            if (IPAddress.TryParse(_settings.Host, out IPAddress ipAddress))
            {
                _nextDnsCheckAt = long.MaxValue; //it's already ip address, don't have to resolve dns ever
                _addresses = new[] { ipAddress };
                return true;
            }

            int minutes = _settings.DnsUpdatePeriod * 60 * 1000; //minutes * seconds_per_minute * ms_per_second
            _nextDnsCheckAt = Environment.TickCount + minutes;

            try
            {
                // Throw SocketException with 'no such host is known' enveloped in AggregateException
#if NETSTANDARD1_6
                IPAddress[] addresses = Dns.GetHostAddressesAsync(_settings.Host).Result;
#else
                IPAddress[] addresses = Dns.GetHostAddresses(_settings.Host);
#endif
                if (AddressesAreSame(addresses))
                {
                    return false;
                }

                _addresses = addresses;
                return true;
            }
            catch (Exception e)
            {
                HandleException(e);
            }

            return false;
        }

        private bool AddressesAreSame(IPAddress[] addresses)
        {
            if (_addresses.Length != addresses.Length)
            {
                return false;
            }

            foreach (IPAddress ipAddress in addresses)
            {
                if (Array.IndexOf(_addresses, ipAddress) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void InitSocket()
        {
            _udpSocket?.Dispose();
            _udpSocket = new Socket(_settings.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.Connect(_addresses, _settings.Port);
        }

        private void HandleException(Exception e)
        {
            string message = $"UdpChannel was not able to send data to 'udp://{_settings.Host}:{_settings.Port}'. " +
                             "Possible reasons are invalid IP address or unresolvable DNS name.";
            if (!_settings.IgnoreSocketExceptions)
            {
                throw new InvalidOperationException(message, e);
            }
        }

        /// <inheritdoc />
        public void Dispose() => _udpSocket?.Dispose();
    }
}