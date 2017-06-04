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

        public static implicit operator Metric(Count count)
        {
            return new Metric(count);
        }

        public static implicit operator Metric(Gauge gauge)
        {
            return new Metric(gauge);
        }

        public static implicit operator Metric(Timing timing)
        {
            return new Metric(timing);
        }
    }
}