using Codestellation.Statsd;
using Codestellation.Statsd.Builder;
using FluentAssertions;
using Xunit;

namespace Tests.Builder
{
    public class BuildStatsdTests
    {
        [Fact]
        public void Should_create_background_client_from_uri()
        {
            var uri = "udp://localhost:8181?prefix=test";

            var actual = BuildStatsd.From(uri);


            actual.Should().BeOfType<BackgroundStatsdClient>();
        }

        [Fact]
        public void Should_create_synchronous_client_from_uri()
        {
            var uri = "udp://localhost:8181?prefix=test&background=false";

            var actual = BuildStatsd.From(uri);


            actual.Should().BeOfType<StatsdClient>();
        }
    }
}