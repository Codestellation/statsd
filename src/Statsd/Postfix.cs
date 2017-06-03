namespace Codestellation.Statsd
{
    internal static class Postfix
    {
        public const int Count = 1 + ((byte)'c' << 8);
        public const int Gauge = 1 + ((byte)'g' << 8);
        public const int Timing = 2 + ((byte)'m' << 8) + ((byte)'s' << 16);
    }
}