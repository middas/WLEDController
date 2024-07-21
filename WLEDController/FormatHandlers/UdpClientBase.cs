using System.Net;
using System.Net.Sockets;

namespace WLEDController.FormatHandlers
{
    internal abstract class UdpClientBase : IFormatHandler
    {
        private bool disposedValue;
        private UdpClient? udpClient;

        public abstract int DefaultPort { get; }

        public void Close()
        {
            udpClient?.Close();
        }

        public void Connect(IPEndPoint endPoint)
        {
            if (udpClient is not null)
            {
                udpClient.Dispose();
            }

            udpClient = new();
            udpClient.Connect(endPoint);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int SendLEDs(LED[] leds, byte? time)
        {
            return Send(leds, time ?? 0x02);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    udpClient?.Dispose();
                }

                udpClient = null;

                disposedValue = true;
            }
        }

        protected abstract int Send(LED[] leds, byte time);

        protected int Send(ReadOnlySpan<byte> data)
        {
            if (udpClient is null)
            {
                throw new Exception();
            }

            if (!udpClient.Client.Connected)
            {
                throw new Exception();
            }

            return udpClient.Send(data);
        }

        protected int SendData(ReadOnlySpan<byte> data)
        {
            if (udpClient is null || !udpClient.Client.Connected)
            {
                throw new Exception();
            }

            return udpClient.Send(data);
        }
    }
}