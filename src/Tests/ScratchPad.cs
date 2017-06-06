using Codestellation.Statsd.Internals;
using Xunit;

namespace Codestellation.Statsd.Tests
{
    public class ScratchPad
    {
        [Fact]
        public void TestBenchMark()
        {
            for (int i = 0; i < 1000; i++)
            {
                _writer.WriteName(_metric1.Name);
                _writer.WriteValue(_metric1.Value);
                _writer.WritePostfix(_metric1.Postfix);

                _writer.WriteName(_metric2.Name);
                _writer.WriteValue(_metric2.Value);
                _writer.WritePostfix(_metric2.Postfix);

                _writer.WriteName(_metric3.Name);
                _writer.WriteValue(_metric3.Value);
                _writer.WritePostfix(_metric3.Postfix);

                _writer.WriteName(_metric4.Name);
                _writer.WriteValue(_metric4.Value);
                _writer.WritePostfix(_metric4.Postfix);

                _writer.WriteName(_metric5.Name);
                _writer.WriteValue(_metric5.Value);
                _writer.WritePostfix(_metric5.Postfix);

                _writer.WriteName(_metric6.Name);
                _writer.WriteValue(_metric6.Value);
                _writer.WritePostfix(_metric6.Postfix);

                _writer.WriteName(_metric7.Name);
                _writer.WriteValue(_metric7.Value);
                _writer.WritePostfix(_metric7.Postfix);

                _writer.WriteName(_metric8.Name);
                _writer.WriteValue(_metric8.Value);
                _writer.WritePostfix(_metric8.Postfix);

                _writer.Reset();
            }
        }

        private readonly StatsdWriter _writer;
        private readonly Metric _metric1;
        private readonly Metric _metric2;
        private readonly Metric _metric3;
        private readonly Metric _metric4;
        private readonly Metric _metric5;
        private readonly Metric _metric6;
        private readonly Metric _metric7;
        private readonly Metric _metric8;

        public ScratchPad()
        {
            _writer = new StatsdWriter("test");

            _metric1 = new Metric(new Timing("time", 1));
            _metric2 = new Metric(new Timing("time", 12));
            _metric3 = new Metric(new Timing("time", 123));
            _metric4 = new Metric(new Timing("time", 1234));
            _metric5 = new Metric(new Timing("time", 12345));
            _metric6 = new Metric(new Timing("time", 123456));
            _metric7 = new Metric(new Timing("time", 1234567));
            _metric8 = new Metric(new Timing("time", 12345678));
        }
    }
}