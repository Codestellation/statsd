using System;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Codestellation.Statsd.Tests
{
    public class LeanStopwatchTests
    {
        [Fact]
        public void Should_measure_time_correctly()
        {
            var watch = LeanStopwatch.StartNew();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var watchElapsed = watch.ElapsedTimeSpan;

            watchElapsed
                .Should()
                .BeGreaterThan(TimeSpan.FromSeconds(0.97))
                .And
                .BeLessThan(TimeSpan.FromSeconds(1.03));
        }
    }
}