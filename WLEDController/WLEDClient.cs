using System.Net;
using WLEDController.FormatHandlers;

namespace WLEDController
{
    public enum Format
    {
        DRGBW,
        DRGB
    }

    public sealed class WLEDClient : IDisposable
    {
        private static readonly Dictionary<Format, IFormatHandler> formats = new()
        {
            {Format.DRGBW, new DRGBWFormatHandler() },
            {Format.DRGB, new DRGBFormatHandler() }
        };

        private bool disposedValue;
        private IFormatHandler? formatHandler;

        public void Close()
        {
            formatHandler?.Close();
        }

        public void Connect(Format format, string url)
        {
            Connect(format, url, formats[format].DefaultPort);
        }

        public void Connect(Format format, string url, int port)
        {
            IPAddress ipAddress = Dns.GetHostAddresses(url)[0];
            Connect(format, ipAddress, port);
        }

        public void Connect(Format format, IPAddress ipAddress)
        {
            Connect(format, ipAddress, formats[format].DefaultPort);
        }

        public void Connect(Format format, IPAddress ipAddress, int port)
        {
            if (formatHandler is not null)
            {
                formatHandler.Close();
            }

            formatHandler = formats[format];

            IPEndPoint ipEndPoint = new(ipAddress, port);
            formatHandler.Connect(ipEndPoint);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int Send(LED[] leds)
        {
            return Send(leds, null);
        }

        public int Send(LED[] leds, byte? time)
        {
            if (formatHandler is null)
            {
                throw new Exception();
            }

            return formatHandler.SendLEDs(leds, time);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    formatHandler?.Dispose();
                }

                formatHandler = null;

                disposedValue = true;
            }
        }
    }
}