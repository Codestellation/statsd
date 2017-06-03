namespace Codestellation.Statsd.Internals
{
    internal struct Metric
    {
        public readonly string Name;
        public readonly int Value;
        public readonly int Postfix;

        public Metric(Count count)
            : this(count.Name, count.Value, Statsd.Postfix.Count)
        {
        }

        public Metric(Gauge gauge)
            : this(gauge.Name, gauge.Value, Statsd.Postfix.Gauge)
        {
        }

        public Metric(Timing timing)
            : this(timing.Name, timing.Value, Statsd.Postfix.Timing)
        {
        }

        private Metric(string name, int value, int postfix)
        {
            Postfix = postfix;
            Name = name;
            Value = value;
        }
    }
}