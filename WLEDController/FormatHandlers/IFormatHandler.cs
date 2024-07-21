using System.Net;

namespace WLEDController.FormatHandlers
{
    internal interface IFormatHandler : IDisposable
    {
        int DefaultPort { get; }

        void Close();

        void Connect(IPEndPoint endPoint);

        int SendLEDs(LED[] leds, byte? time);
    }
}