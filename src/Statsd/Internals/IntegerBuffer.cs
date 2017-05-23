using System;

namespace Codestellation.Statsd.Internals
{
    internal unsafe struct IntegerBuffer
    {
        private const int DigitSymbolOffset = '0';
        private const int BufferSize = 11;

        ///Maximum integer number length in decimal representation is 10, plus a byte for a sign
        private fixed byte _numberBuffer[BufferSize];

        //The method is not supposed to change parameters, but position (ref is simple faster than normal parameter)
        public void WriteNumber(ref byte[] buffer, ref int position, ref int value)
        {
            fixed (byte* nbp = _numberBuffer)
            {
                int numberBufferOffset = BufferSize;
                for (int quotient = value; quotient != 0; quotient /= 10)
                {
                    nbp[--numberBufferOffset] = (byte)(DigitSymbolOffset + quotient % 10);
                }

                var length = BufferSize - numberBufferOffset;
#if NETSTANDARD1_6 || NET46
                fixed (byte* buf = &buffer[position])
                {
                    Buffer.MemoryCopy(nbp + numberBufferOffset, buf, Math.Min(buffer.Length - position, length), length);
                }
#else
                for (int i = numberBufferOffset; i < numberBufferOffset + length; i++)
                {
                    buffer[position++] = nbp[i];
                }

#endif
                position += length;
            }
        }
    }
}