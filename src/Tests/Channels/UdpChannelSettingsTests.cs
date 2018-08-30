using System;
using System.Net.Sockets;
using Codestellation.Statsd.Channels;
using FluentAssertions;
using Xunit;

namespace Codestellation.Statsd.Tests.Channels
{
    public class UdpChannelSettingsTests
    {
        [Fact]
        public void Should_parse_uri_properly()
        {
            var uri = new Uri("udp://my-host:8085?prefix=the.service&background=false&ignore_exceptions&dns_update_period=13&ipv6");

            var settings = UdpChannelSettings.Parse(uri);

            var expected = new UdpChannelSettings
            {
                Host = "my-host",
                IgnoreSocketExceptions = true,
                Port = 8085,
                DnsUpdatePeriod = 13,
                AddressFamily = AddressFamily.InterNetworkV6
            };

            settings.ShouldBeEquivalentTo(expected);
        }
    }
}