namespace Codestellation.Statsd
{
    /// <summary>
    /// Represents a channel that is used to send data to a statsd server
    /// <remarks>Implementations are not supposed to be thread safe</remarks>
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Sends data to a statsd server
        /// </summary>
        /// <param name="buffer">Buffer which contains data to send</param>
        /// <param name="count">Number of bytes to send from the buffer</param>
        void Send(byte[] buffer, int count);
    }
}