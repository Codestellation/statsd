using System;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a statsd count value
    /// </summary>
    public struct Count
    {
        internal string _name;
        internal int _value;

        /// <summary>
        /// Name of the metric
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Number of times something happened
        /// </summary>
        public int Value => _value;

        /// <summary>
        /// Initializes a new instance of <see cref="Count"/> structure
        /// </summary>
        /// <param name="name">Name of the metric</param>
        /// <param name="value">Number of times something happened</param>
        public Count(string name, int value)
        {
            _name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException($"Must be neither null nor empty string but was '{name}'", nameof(name))
                : name;
            _value = value;
        }
    }
}