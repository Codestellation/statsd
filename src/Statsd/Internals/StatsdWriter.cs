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

        private readonly byte[] _buffer;
        private int _position;
        private readonly byte[] _intBuffer;
        private readonly Dictionary<string, byte[]> _stringCache;

        public byte[] Buffer => _buffer;

        public int Position => _position;

        public bool MtuExceeded => _position >= MaxMtu;
        public bool ContainsData => _position > 0;

        public StatsdWriter(string prefix)
        {
            _prefix = prefix;
            _intBuffer = new byte[10];
            _buffer = new byte[1024];
            _position = 0;
            _stringCache = new Dictionary<string, byte[]>(StringComparer.Ordinal);
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
            int digitCount = 0;
            for (int quotient = value; quotient != 0; quotient /= 10)
            {
                _intBuffer[digitCount++] = (byte)(offset + quotient % 10);
            }

            for (int i = digitCount - 1; i >= 0; i--)
            {
                _buffer[_position++] = _intBuffer[i];
            }
        }

        public void WritePostfix(Type type)
        {
            switch (type)
            {
                case Type.Count:
                    WritePostfix('c');
                    return;
                case Type.Gauge:
                    WritePostfix('g');
                    return;
                case Type.Timing:
                    WritePostfix('m', 's');
                    return;
            }
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