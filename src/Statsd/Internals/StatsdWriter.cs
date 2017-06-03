using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, byte[]> _stringCache;

        public byte[] Buffer => _buffer;

        public int Position => _position;

        public bool MtuExceeded => _position >= MaxMtu;
        public bool ContainsData => _position > 0;

        public StatsdWriter(string prefix)
        {
            _prefix = prefix;
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
            //TODO: Check out a better way to copy name from cache
            Array.Copy(utf8Array, 0, _buffer, _position, utf8Array.Length);
            _position += utf8Array.Length;
        }

        public void WriteValue(int value)
        {
            const int offset = '0';

            if (value < 10)
            {
                _buffer[_position++] = (byte)(offset + value);
                return;
            }

            int length;
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

            for (int i = _position + length - 1; i >= _position; --i)
            {
                _buffer[i] = (byte)(offset + value % 10);
                value /= 10;
            }

            _position += length;
        }

        public void WriteSeparator()
        {
            _buffer[_position++] = (byte)'\n';
        }

        public void Reset()
        {
            _position = 0;
        }

        public unsafe void WritePostfix(int postfix)
        {
            const byte separator = (byte)'|';
            fixed (byte* d = &_buffer[_position])
            {
                switch (((byte*)&postfix)[0])
                {
                    case 1:
                        d[0] = separator;
                        d[1] = ((byte*)&postfix)[1];
                        _position += 2;
                        break;
                    case 2:
                        d[0] = separator;
                        d[1] = ((byte*)&postfix)[1];
                        d[2] = ((byte*)&postfix)[2];
                        _position += 3;
                        break;
                }
            }
        }
    }
}