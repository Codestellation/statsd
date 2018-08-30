using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Codestellation.Statsd.Channels;
using FluentAssertions;
using Xunit;

namespace Codestellation.Statsd.Tests.Channels
{
    public class UdpChannelTests : IDisposable 
    {
        private Socket _server;
        private UdpChannel _channel;
        private byte[] _message;

        
        [Theory]
        [InlineData("localhost", AddressFamily.InterNetwork)]
        [InlineData("127.0.0.1", AddressFamily.InterNetwork)]
        [InlineData("localhost", AddressFamily.InterNetworkV6)]
        [InlineData("::1", AddressFamily.InterNetworkV6)]
        public void Should_send_bytes_over_the_network(string statsdHost, AddressFamily family)
        {
            _server = new Socket(family, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 3000
            };
            _server.Bind(new IPEndPoint(family == AddressFamily.InterNetwork ?  IPAddress.Loopback : IPAddress.IPv6Loopback, 8125));

            _message = Encoding.UTF8.GetBytes("Hello server!");
            
            var settings = new UdpChannelSettings
            {
                Host = statsdHost, 
                Port = 8125,
                AddressFamily = family
            };
            
            _channel = new UdpChannel(settings);
            _channel.Send(_message, _message.Length);
            var receiveBuffer = new byte[_message.Length];

            _server.Receive(receiveBuffer);
            
            receiveBuffer.ShouldBeEquivalentTo(_message);
        }
        
        public void Dispose()
        {
            _server?.Dispose();
            _channel.Dispose();
        }
    }
}