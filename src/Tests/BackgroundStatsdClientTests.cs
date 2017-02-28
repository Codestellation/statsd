using System.Linq;
using Codestellation.Statsd;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class BackgroundStatsdClientTests
    {
        private readonly LoggingChannel _channel;
        private readonly BackgroundStatsdClient _client;

        public BackgroundStatsdClientTests()
        {
            _channel = new LoggingChannel();
            _client = new BackgroundStatsdClient(_channel);
        }

        [Fact]
        public void Should_log_single_count_correctly()
        {
            //arrange
            var count = new Count("fresh.bananas", 10);

            //act
            _client.LogCount(count);

            //assert
            AssertMessageIs("fresh.bananas:10|c");
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
            AssertMessageIs(@"fresh.bananas:13|c\nrotten.tomato:44|c");
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
            AssertMessageIs(@"fresh.bananas:13|c\nrotten.tomato:44|c\npoisoned.apples:0|c");
        }

        [Fact]
        public void Should_log_multiple_count_correctly()
        {
            //arrange
            var counts = Enumerable.Range(1, 20).Select(x => new Count("banana", x)).ToList();

            //act
            _client.LogCount(counts);

            //assert
            var expected = string.Join(@"\n", counts.Select(x => $"{x.Name}:{x.Value}|c"));
            AssertMessageIs(expected);
        }

        [Fact]
        public void Should_log_single_gauge_correctly()
        {
            //arrange
            var gauge = new Gauge("fresh.bananas", 10);

            //act
            _client.LogGauge(gauge);

            //assert
            AssertMessageIs("fresh.bananas:10|g");
        }

        [Fact]
        public void Should_log_double_gauge_correctly()
        {
            //arrange
            var gauge1 = new Gauge("fresh.bananas", 13);
            var gauge2 = new Gauge("rotten.tomato", 44);
            //act
            _client.LogGauge(gauge1, gauge2);

            //assert
            AssertMessageIs(@"fresh.bananas:13|g\nrotten.tomato:44|g");
        }

        [Fact]
        public void Should_log_triple_gauge_correctly()
        {
            //arrange
            var gauge1 = new Gauge("fresh.bananas", 13);
            var gauge2 = new Gauge("rotten.tomato", 44);
            var gauge3 = new Gauge("poisoned.apples", 0);

            //act
            _client.LogGauge(gauge1, gauge2, gauge3);

            //assert
            AssertMessageIs(@"fresh.bananas:13|g\nrotten.tomato:44|g\npoisoned.apples:0|g");
        }

        [Fact]
        public void Should_log_multiple_gauge_correctly()
        {
            //arrange
            var gauges = Enumerable.Range(1, 20).Select(x => new Gauge("banana", x)).ToList();

            //act
            _client.LogGauge(gauges);

            //assert
            var expected = string.Join(@"\n", gauges.Select(x => $"{x.Name}:{x.Value}|g"));
            AssertMessageIs(expected);
        }

        [Fact]
        public void Should_log_single_timing_correctly()
        {
            //arrange
            var timing = new Timing("fresh.bananas", 10);

            //act
            _client.LogTiming(timing);

            //assert
            AssertMessageIs("fresh.bananas:10|ms");
        }

        [Fact]
        public void Should_log_double_timing_correctly()
        {
            //arrange
            var timing1 = new Timing("fresh.bananas", 13);
            var timing2 = new Timing("rotten.tomato", 44);
            //act
            _client.LogTiming(timing1, timing2);

            //assert
            AssertMessageIs(@"fresh.bananas:13|ms\nrotten.tomato:44|ms");
        }

        [Fact]
        public void Should_log_triple_timing_correctly()
        {
            //arrange
            var timing1 = new Timing("fresh.bananas", 13);
            var timing2 = new Timing("rotten.tomato", 44);
            var timing3 = new Timing("poisoned.apples", 0);

            //act
            _client.LogTiming(timing1, timing2, timing3);

            //assert
            AssertMessageIs(@"fresh.bananas:13|ms\nrotten.tomato:44|ms\npoisoned.apples:0|ms");
        }

        [Fact]
        public void Should_log_multiple_timings_correctly()
        {
            //arrange
            var timings = Enumerable.Range(1, 20).Select(x => new Timing("banana", x)).ToList();

            //act
            _client.LogTiming(timings);

            //assert
            var expected = string.Join(@"\n", timings.Select(x => $"{x.Name}:{x.Value}|ms"));
            AssertMessageIs(expected);
        }

        private void AssertMessageIs(string expected)
        {
            for (int i = 0; i < 100; i++)
            {
                if (_channel.WaitForMessage())
                {
                    break;
                }
            }
            var actual = _channel.Last;
            actual.Should().Be(expected);
        }
    }
}