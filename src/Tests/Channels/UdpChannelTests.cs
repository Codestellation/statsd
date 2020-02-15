using System.Net;
using System.Net.Sockets;
using System.Text;
using Codestellation.Statsd.Channels;
using FluentAssertions;
using Xunit;

namespace Codestellation.Statsd.Tests.Channels
{
    public class UdpChannelTests
    {
        [Theory]
        [InlineData("localhost", AddressFamily.InterNetwork)]
        [InlineData("127.0.0.1", AddressFamily.InterNetwork)]
        [InlineData("localhost", AddressFamily.InterNetworkV6)]
        [InlineData("::1", AddressFamily.InterNetworkV6)]
        public void Should_send_bytes_over_the_network(string statsdHost, AddressFamily family)
        {
            using var server = new Socket(family, SocketType.Dgram, ProtocolType.Udp) { ReceiveTimeout = 3000 };
            server.Bind(new IPEndPoint(family == AddressFamily.InterNetwork ? IPAddress.Loopback : IPAddress.IPv6Loopback, 8125));

            byte[] message = Encoding.UTF8.GetBytes("Hello server!");

            var settings = new UdpChannelSettings
            {
                Host = statsdHost,
                Port = 8125,
                AddressFamily = family
            };

            using var channel = new UdpChannel(settings);
            channel.Send(message, message.Length);
            var receiveBuffer = new byte[message.Length];

            server.Receive(receiveBuffer);

            receiveBuffer.ShouldBeEquivalentTo(message);
        }
    }
}