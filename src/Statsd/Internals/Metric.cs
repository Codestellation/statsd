namespace Codestellation.Statsd.Internals
{
    internal struct Metric
    {
        public string Name;
        public int Value;
        private Postfix _postfix;

        public Metric(Count count)
            : this(count.Name, count.Value, Postfix.Count)
        {
        }

        public Metric(Gauge gauge)
            : this(gauge.Name, gauge.Value, Postfix.Gauge)
        {
        }

        public Metric(Timing timing)
            : this(timing.Name, timing.Value, Postfix.Timing)
        {
        }

        private Metric(string name, int value, Postfix postfix)
        {
            Name = name;
            Value = value;
            _postfix = postfix;
        }

        public void WritePostfix(ref byte[] buffer, ref int position)
        {
            _postfix.Write(ref buffer, ref position);
        }
    }
}