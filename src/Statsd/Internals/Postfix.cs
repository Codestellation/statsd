using System;

namespace Codestellation.Statsd.Internals
{
    internal unsafe struct Postfix
    {
        public static Postfix Count = new Postfix('c');
        public static Postfix Gauge = new Postfix('g');
        public static Postfix Timing = new Postfix('m', 's');

        private fixed byte _postfix[4];
        private readonly byte _length;

        private Postfix(char c1)
        {
            _length = 2;
            fixed (byte* p = _postfix)
            {
                p[0] = (byte)'|';
                p[1] = (byte)c1;
            }
        }

        private Postfix(char c1, char c2)
        {
            fixed (byte* p = _postfix)
            {
                p[0] = (byte)'|';
                p[1] = (byte)c1;
                p[2] = (byte)c2;
            }
            _length = 3;
        }

        private Postfix(char c1, char c2, char c3)
        {
            fixed (byte* p = _postfix)
            {
                p[0] = (byte)'|';
                p[1] = (byte)c1;
                p[2] = (byte)c2;
                p[3] = (byte)c3;
            }
            _length = 4;
        }

        public void Write(ref byte[] buffer, ref int position)
        {
#if NETSTANDARD1_6 || NET46
            fixed (byte* source = _postfix)
            fixed (byte* dest = &buffer[position])
            {
                Buffer.MemoryCopy(source, dest, Math.Min(buffer.Length - position, _length), _length);
            }
#else
            fixed (byte* source = _postfix)
            fixed (byte* dest = &buffer[position])
            {
                for(int i = 0; i < _length; i++)    
                {
                    dest[i] = source[i];
                }

            }
#endif
            position += _length;
        }
    }
}