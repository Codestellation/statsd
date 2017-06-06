using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Codestellation.Statsd.Bench
{
    [ClrJob]
    [CoreJob]
    [LegacyJitX64Job]
    [RyuJitX64Job]
    public class ArrayCopyBenchmark
    {
        private readonly byte[] _source;
        private readonly byte[] _target;

        [Params(5, 11, 19, 35, 67, 131, 259)]
        public int Length { get; set; }

        public ArrayCopyBenchmark()
        {
            _source = new byte[512];
            _target = new byte[512];
        }

        [Benchmark(Baseline = true)]
        public void Array_copy()
        {
            Array.Copy(_source, 0, _target, 0, Length);
        }

        [Benchmark]
        public void Buffer_block_copy()
        {
            Buffer.BlockCopy(_source, 0, _target, 0, Length);
        }

        [Benchmark]
        public void For_loop()
        {
            for (int i = 0; i < Length; i++)
            {
                _target[i] = _source[i];
            }
        }

        [Benchmark]
        public unsafe void For_loop_unsafe()
        {
            fixed (byte* s = _source)
            fixed (byte* t = _target)
                for (int i = 0; i < Length; i++)
                {
                    t[i] = s[i];
                }
        }

        [Benchmark]
        public unsafe void For_loop_fast_unsafe()
        {
            if (Length < 8)
            {
                for (int i = 0; i < Length; i++)
                {
                    _target[i] = _source[i];
                }
                return;
            }

            var copied = 0;
            var maxLong = Length / sizeof(long);
            fixed (byte* s = _source)
            fixed (byte* t = _target)
            {
                long* lsp = (long*)s;
                long* tsp = (long*)t;

                for (int i = 0; i < maxLong; i++)
                {
                    tsp[i] = lsp[i];
                    copied += sizeof(long);
                }
                //copy remainder
                for (int i = copied; i < Length; i++)
                {
                    t[i] = s[i];
                }
            }
        }
    }
}