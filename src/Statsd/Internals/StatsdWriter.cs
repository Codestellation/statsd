using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Codestellation.Statsd.Internals
{
    internal class StatsdWriter
    {
        private readonly string _prefix;
        private const int MaxMtu = 3 * 128;
        private static readonly Encoding Encoding = new UTF8Encoding(false);

        private byte[] _buffer;
        private int _position;
        private readonly Dictionary<string, byte[]> _stringCache;
        private IntegerBuffer _intBuffer;

        public byte[] Buffer => _buffer;

        public int Position => _position;

        public bool MtuExceeded => _position >= MaxMtu;
        public bool ContainsData => _position > 0;

        public StatsdWriter(string prefix)
        {
            _prefix = prefix;
            _intBuffer = new IntegerBuffer();
            _buffer = new byte[1024];
            _position = 0;
            _stringCache = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        }

        public void Write(ref Metric metric)
        {
            WriteName(metric.Name);
            WriteValue2(metric.Value);
            metric.WritePostfix(ref _buffer, ref _position);
        }

        public void WriteName(string name)
        {
            byte[] utf8Array;
            if (!_stringCache.TryGetValue(name, out utf8Array))
            {
                var fullname = _prefix == null ? $"{name}:" : $"{_prefix}.{name}:";
                utf8Array = Encoding.GetBytes(fullname);
                _stringCache[name] = utf8Array;
            }
            Array.Copy(utf8Array, 0, _buffer, _position, utf8Array.Length);
            _position += utf8Array.Length;
        }
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void WriteValue(int value)
        {
            const byte offset = (byte)'0';

            if (0 <= value && value < 10)
            {
                _buffer[_position++] = (byte)(offset + (byte)value);
                return;
            }

            //format int to it's string representation
            _position += _intBuffer.WriteNumber(_buffer, _position, value);
        }

        public void WriteValue2(int value)
        {
            const byte offset = (byte)'0';

            if (0 <= value && value < 10)
            {
                _buffer[_position++] = (byte)(offset + (byte)value);
                return;
            }

            var length = 0;
            if (value < 100)
            {
                length = 2;
            }
            else if (value < 1_000)
            {
                length = 3;
            }
            else if (value < 10_000)
            {
                length = 4;
            }
            else if (value < 100_000)
            {
                length = 5;
            }
            else if (value < 1_000_000)
            {
                length = 6;
            }
            else if (value < 10_000_000)
            {
                length = 7;
            }
            else if (value < 100_000_000)
            {
                length = 8;
            }
            else if (value < 1_000_000_000)
            {
                length = 9;
            }
            else
            {
                length = 10;
            }

            for (int i = _position + length - 1; i >= _position; i--)
            {
                _buffer[i] = (byte)('0' + value % 10);
                value /= 10;
            }

            //for (int quotient = value; quotient != 0; quotient /= 10)
            //{
            //    nbp[--numberBufferOffset] = (byte)( + quotient % 10);
            //}

            _position += length;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void WritePostfix(char c)
        {
            _buffer[_position++] = (byte)'|';
            _buffer[_position++] = (byte)c;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void WritePostfix(char c1, char c2)
        {
            _buffer[_position++] = (byte)'|';
            _buffer[_position++] = (byte)c1;
            _buffer[_position++] = (byte)c2;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void WritePostfix(char c1, char c2, char c3)
        {
            _buffer[_position++] = (byte)'|';
            _buffer[_position++] = (byte)c1;
            _buffer[_position++] = (byte)c2;
            _buffer[_position++] = (byte)c3;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void WriteSeparator()
        {
            _buffer[_position++] = (byte)'\n';
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Reset()
        {
            _position = 0;
        }
    }
}