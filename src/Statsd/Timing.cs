using System;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a statsd timing metric value
    /// </summary>
    public struct Timing
    {
        internal string _name;
        internal int _value;

        /// <summary>
        /// Name of the metric
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Time interval in milliseconds
        /// </summary>
        public int Value => _value;

        /// <summary>
        /// Initializes a new instance of <see cref="Timing"/> structure
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="value">Time interval in milliseconds</param>
        public Timing(string name, int value)
        {
            _name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"Must be neither null nor empty string but was '{name}'", nameof(name))
                : name;
            _value = value < 0
                ? throw new ArgumentException($"Must be non-negative value but was {value}", nameof(value))
                : value;
        }
    }
}