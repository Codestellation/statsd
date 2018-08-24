using System;
using System.Diagnostics;

namespace Codestellation.Statsd
{
    /// <summary>
    /// Represent analogue to <see cref="Stopwatch"/>, but do not produce garbage.
    /// <remarks>
    /// Because <see cref="LeanStopwatch"/> is an immutable value type, it's impossible to create methods Stop method like <see cref="Stopwatch"/> does. 
    /// So just call to <see cref="Elapsed"/> or <see cref="Elapsed(string)"/> to get time elapsed from the <see cref="StartNew"/> call.
    /// </remarks>
    /// </summary>
    public readonly struct LeanStopwatch
    {
        private readonly long _startedAt;

        private LeanStopwatch(long startedAt)
        {
            _startedAt = startedAt;
        }

        /// <summary>
        /// Returns interval from the call to <see cref="StartNew"/> to the current moment
        /// </summary>
        public TimeSpan ElapsedTimeSpan
        {
            get => TimeSpan.FromTicks(Stopwatch.GetTimestamp() - _startedAt);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LeanStopwatch"/> structure. 
        /// </summary>
        public static LeanStopwatch StartNew() => new LeanStopwatch(Stopwatch.GetTimestamp());

        /// <summary>
        /// Returns an <see cref="Timing"/> from the call to <see cref="StartNew"/> to the current moment.
        /// </summary>
        public Timing Elapsed(string name) => new Timing(name, Convert.ToInt32(ElapsedTimeSpan.TotalMilliseconds));
    }
}