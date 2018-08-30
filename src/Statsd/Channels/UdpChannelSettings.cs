using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Codestellation.Statsd.Builder;

namespace Codestellation.Statsd.Channels
{
    /// <summary>
    /// Contains specific settings for <see cref="UdpChannel"/>
    /// </summary>
    public class UdpChannelSettings
    {
        private AddressFamily _addressFamily;

        /// <summary>
        /// Statsd server host. Must be either valid ip address or dns name
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port a statsd server listens to. Typical value is 8125.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// <see cref="UdpChannel"/> will throw error if any <see cref="SocketException"/> happens.
        /// </summary>
        public bool IgnoreSocketExceptions { get; set; }
        /// <summary>
        /// How often to check for dns changes in minutes. 
        /// </summary>
        public int DnsUpdatePeriod { get; set; }

        /// <summary>
        /// Gets of sets current family for the underlying socket.  
        /// </summary>
        public AddressFamily AddressFamily
        {
            get => _addressFamily;
            set
            {
                if (value == AddressFamily.InterNetwork || value == AddressFamily.InterNetworkV6)
                {
                    _addressFamily = value;
                    return;
                }
                var message = $"Must be either {AddressFamily.InterNetworkV6.ToString()} or {AddressFamily.InterNetwork.ToString()} but was {value.ToString()}";
                throw new ArgumentException(message);
            }
        }
        /// <summary>
        /// Initializes a new instance of <see cref="UdpChannelSettings"/>
        /// </summary>
        public UdpChannelSettings()
        {
            DnsUpdatePeriod = 10;
            AddressFamily = AddressFamily.InterNetwork;
        }
        
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

            Dictionary<string, string> values = uri.GetQueryValues();

            return new UdpChannelSettings
            {
                Host = uri.Host,
                Port = uri.Port,
                IgnoreSocketExceptions = values.ParseOrDefault(UriParseExtensions.IgnoreExceptions, onDefault: true),
                DnsUpdatePeriod = values.ParseOrDefault(UriParseExtensions.DnsUpdatePeriod, onDefault: 10),
                AddressFamily = values.ContainsKey("IPv6") ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork
            };
        }
    }
}