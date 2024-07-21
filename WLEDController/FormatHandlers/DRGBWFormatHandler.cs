namespace WLEDController.FormatHandlers
{
    internal sealed class DRGBWFormatHandler : UdpClientBase
    {
        private const int maxLeds = 367;
        private const int port = 21324;
        private const byte protocolByte = 0x03;

        public override int DefaultPort => port;

        protected override int Send(LED[] leds, byte time)
        {
            if (leds.Length > maxLeds || leds.Max(x => x.Index) > maxLeds)
            {
                throw new Exception();
            }

            byte[] data = new byte[(leds.Max(x => x.Index + 1) * 4) + 5];
            data[0] = protocolByte;
            data[1] = time;

            foreach (LED led in leds)
            {
                int n = led.Index * 4;

                data[n + 2] = led.Red;
                data[n + 3] = led.Green;
                data[n + 4] = led.Blue;
                data[n + 5] = led.White;
            }

            return SendData(data);
        }
    }
}