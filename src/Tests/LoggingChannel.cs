using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Codestellation.Statsd.Tests
{
    public class LoggingChannel : IChannel
    {
        private readonly List<string> _messages;
        private readonly AutoResetEvent _arrived;

        public string Last
        {
            get
            {
                lock (_messages)
                {
                    return _messages.Last();
                }
            }
        }

        public IReadOnlyCollection<string> Messages
        {
            get
            {
                lock (_messages)
                {
                    return new ReadOnlyCollection<string>(_messages);
                }
            }
        }

        public LoggingChannel()
        {
            _messages = new List<string>();
            _arrived = new AutoResetEvent(false);
        }

        public void Send(byte[] buffer, int count)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, count).Replace("\n", "\\n");
            lock (_messages)
            {
                _messages.Add(message);
                _arrived.Set();
            }
        }

        public bool WaitForMessage()
        {
            return _arrived.WaitOne(100);
        }
    }
}