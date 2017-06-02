using System;
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
            var uri = new Uri("udp://my-host:8085?prefix=the.service&background=false&ignore_exceptions");

            var settings = UdpChannelSettings.Parse(uri);

            var expected = new UdpChannelSettings
            {
                Host = "my-host",
                IgnoreSocketExceptions = true,
                Port = 8085
            };

            settings.ShouldBeEquivalentTo(expected);
        }
    }
}