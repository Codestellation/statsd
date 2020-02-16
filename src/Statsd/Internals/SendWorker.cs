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
        private readonly Action<Exception> _exceptionHandler;
        private readonly Metric[] _batch;
        private readonly Task _task;
        private readonly CancellationTokenSource _source;

        public SendWorker(MetricsQueue queue, IChannel channel, string prefix, Action<Exception> exceptionHandler = null)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _exceptionHandler = exceptionHandler;
            _writer = new StatsdWriter(prefix);

            _source = new CancellationTokenSource();
#if NET40
            _task = new Task((Action)ProcessQueue, _source.Token);
            _task.Start();
#else
            _task = Task.Run(ProcessQueue, _source.Token);
#endif

            _batch = new Metric[50];
        }

        private void ProcessQueue()
        {
            while (!_source.IsCancellationRequested)
            {
                try
                {
                    int batchSize = _queue.DequeueInto(_batch);

                    if (batchSize > 0)
                    {
                        SendBatch(batchSize);
                    }
                }
                catch (Exception ex)
                {
                    //Socket exception must be handled at channel level. Handle here unexpected one
                    _exceptionHandler?.Invoke(ex);
                }
            }
        }

        private void SendBatch(int batchSize)
        {
            for (var i = 0; i < batchSize; i++)
            {
                Metric metric = _batch[i];
                if (_writer.ContainsData)
                {
                    _writer.WriteSeparator();
                }

                _writer.WriteName(metric.Name);
                _writer.WriteValue(metric.Value);
                _writer.WritePostfix(metric.Postfix);

                if (_writer.MtuExceeded)
                {
                    Send();
                }
            }

            if (_writer.ContainsData)
            {
                Send();
            }
        }

        private void Send()
        {
            _channel.Send(_writer.Buffer, _writer.Position);
            _writer.Reset();
        }

        public void Dispose()
        {
            _source.Cancel();
            _queue.Stop();
            (_channel as IDisposable)?.Dispose();
        }
    }
}