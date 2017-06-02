using System.Globalization;
using Codestellation.Statsd;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class NumberSerialzationTest
    {
        private readonly LoggingChannel _channel;
        private readonly StatsdClient _client;

        public NumberSerialzationTest()
        {
            _channel = new LoggingChannel();
            _client = new StatsdClient(_channel);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(43)]
        [InlineData(667)]
        [InlineData(9374)]
        [InlineData(31251)]
        [InlineData(258641)]
        [InlineData(1258641)]
        [InlineData(31258641)]
        [InlineData(457621762)]
        [InlineData(int.MaxValue)]
        public void Should_log_single_count_correctly(int value)
        {
            //arrange
            var count = new Count("fresh.bananas", value);

            //act
            _client.LogCount(count);

            //assert
            var actual = _channel.Last;
            var expected = $"fresh.bananas:{value.ToString(CultureInfo.InvariantCulture)}|c";

            actual.Should().Be(expected);
        }
    }
}