using Codestellation.Statsd;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class PrefixedStatsdClientTests
    {
        private readonly LoggingChannel _channel;
        private readonly StatsdClient _client;

        public PrefixedStatsdClientTests()
        {
            _channel = new LoggingChannel();
            _client = new StatsdClient(_channel, "custom.prefix");
        }

        [Fact]
        public void Should_log_single_count_correctly()
        {
            //arrange
            var count = new Count("fresh.bananas", 10);

            //act
            _client.LogCount(count);

            //assert
            var actual = _channel.Last;
            var expected = "custom.prefix.fresh.bananas:10|c";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_log_double_count_correctly()
        {
            //arrange
            var count1 = new Count("fresh.bananas", 13);
            var count2 = new Count("rotten.tomato", 44);
            //act
            _client.LogCount(count1, count2);

            //assert
            var actual = _channel.Last;
            var expected = @"custom.prefix.fresh.bananas:13|c\ncustom.prefix.rotten.tomato:44|c";

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_log_triple_count_correctly()
        {
            //arrange
            var count1 = new Count("fresh.bananas", 13);
            var count2 = new Count("rotten.tomato", 44);
            var count3 = new Count("poisoned.apples", 0);

            //act
            _client.LogCount(count1, count2, count3);

            //assert
            var actual = _channel.Last;
            var expected = @"custom.prefix.fresh.bananas:13|c\ncustom.prefix.rotten.tomato:44|c\ncustom.prefix.poisoned.apples:0|c";

            actual.Should().Be(expected);
        }
    }
}