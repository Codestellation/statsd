using System;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a statsd timing metric value
    /// </summary>
    public struct Timing
    {
        /// <summary>
        /// Name of the metric
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Time interval in milliseconds
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Timing"/> structure
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="value">Time interval in milliseconds</param>
        public Timing(string name, int value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Must be neither null nor empty string but was '{name}'", nameof(name));
            }
            if (value < 0)
            {
                throw new ArgumentException($"Must be non-negative value but was {value}", nameof(value));
            }
            Name = name;
            Value = value;
        }
    }
}