using System;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a statsd gauge metric value
    /// </summary>
    public struct Gauge
    {
        /// <summary>
        /// Name of the metric
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Current level of something
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Creates new instance of <see cref="Gauge"/> structure
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="value">Current level of something</param>
        public Gauge(string name, int value)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"Must be neither null nor empty string but was '{name}'", nameof(name))
                : name;
            Value = value;
        }
    }
}