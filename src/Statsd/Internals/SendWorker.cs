using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codestellation.Statsd.Internals
{
    internal class SendWorker : IDisposable
    {
        private readonly StatsdWriter _writer;

        private readonly MetricsQueue _queue;
        private readonly IChannel _channel;
        private readonly Metric[] _batch;
        private readonly Task _task;
        private readonly CancellationTokenSource _source;

        public SendWorker(MetricsQueue queue, IChannel channel, string prefix)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _writer = new StatsdWriter(prefix);

            _source = new CancellationTokenSource();
#if NET40
            _task = new Task((Action)ProcessQueue, _source.Token);
            _task.Start();
#else
            _task = Task.Run((Action)ProcessQueue, _source.Token);
#endif


            _batch = new Metric[50];
        }

        private void ProcessQueue()
        {
            while (!_source.IsCancellationRequested)
            {
                int batchSize = _queue.DequeueInto(_batch);

                if (batchSize > 0)
                {
                    SendBatch(batchSize);
                }
            }
        }

        private void SendBatch(int batchSize)
        {
            for (int i = 0; i < batchSize; i++)
            {
                
                if (_writer.ContainsData)
                {
                    _writer.WriteSeparator();
                }

                _writer.Write(ref _batch[i]);

                if (_writer.MtuExceeded)
                {
                    _channel.Send(_writer.Buffer, _writer.Position);
                    _writer.Reset();
                }
            }
            if (_writer.ContainsData)
            {
                _channel.Send(_writer.Buffer, _writer.Position);
            }
        }

        public void Dispose()
        {
            _source.Cancel();
            _task.Wait();
        }
    }
}