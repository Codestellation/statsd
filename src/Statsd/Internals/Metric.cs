namespace Codestellation.Statsd.Internals
{
    internal struct Metric
    {
        public readonly Type Type;
        public readonly string Name;
        public readonly int Value;

        public Metric(Count count)
            : this(Type.Count, count.Name, count.Value)
        {
        }

        public Metric(Gauge gauge)
            : this(Type.Gauge, gauge.Name, gauge.Value)
        {
        }

        public Metric(Timing timing)
            : this(Type.Timing, timing.Name, timing.Value)
        {
        }

        private Metric(Type type, string name, int value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}