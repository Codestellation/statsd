using BenchmarkDotNet.Running;

namespace Codestellation.Statsd.Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ArrayCopyBenchmark>();
        }
    }
}