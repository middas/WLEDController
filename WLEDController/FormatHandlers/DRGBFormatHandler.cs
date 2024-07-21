using WLEDController.Exceptions;

namespace WLEDController.FormatHandlers
{
    internal sealed class DRGBFormatHandler : UdpClientBase
    {
        private const int maxLeds = 490;
        private const int port = 21324;
        private const byte protocolByte = 0x02;

        public override int DefaultPort => port;

        protected override int Send(LED[] leds, byte time)
        {
            if (leds.Length > maxLeds || leds.Max(x => x.Index) > maxLeds)
            {
                throw new WLEDClientException($"The number of LEDs ({Math.Max(leds.Max(x => x.Index), leds.Length)}) is greater than the maximum number allowed for this format ({maxLeds}).");
            }

            byte[] data = new byte[(leds.Max(x => x.Index) * 3) + 4];
            data[0] = protocolByte;
            data[1] = time;

            foreach (LED led in leds)
            {
                int n = led.Index * 3;

                data[n + 2] = led.Red;
                data[n + 3] = led.Green;
                data[n + 4] = led.Blue;
            }

            return SendData(data);
        }
    }
}