using System;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a statsd gauge metric value
    /// </summary>
    public struct Gauge
    {
        internal string _name;
        internal int _value;

        /// <summary>
        /// Name of the metric
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Current level of something
        /// </summary>
        public int Value => _value;

        /// <summary>
        /// Creates new instance of <see cref="Gauge"/> structure
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="value">Current level of something</param>
        public Gauge(string name, int value)
        {
            _name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"Must be neither null nor empty string but was '{name}'", nameof(name))
                : name;
            _value = value;
        }
    }
}