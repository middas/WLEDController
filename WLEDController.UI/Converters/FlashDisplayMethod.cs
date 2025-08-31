using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLEDController.UI.Extensions;
using WLEDController.UI.Models;

namespace WLEDController.UI.Converters
{
    internal class FlashDisplayMethod : IDisplayMethod
    {
        private LED[] leds = [];
        private LightColorMap[] binaryColorMaps = [];
        private long loopCount;
        private int delay;

        public void ConfigureMappings(IEnumerable<WordMap> wordMaps, BitArray start, int numberOfLights, int delay)
        {
            leds = new LED[numberOfLights];

            for (int i = 0; i < leds.Length; i++)
            {
                leds[i] = new(i, Color.Black);
            }

            binaryColorMaps = [.. start.Select(x => new LightColorMap(Color.Black, x)), .. wordMaps.SelectMany(x => x.GetLightColorMaps())];

            loopCount = binaryColorMaps.Length;
            this.delay = delay;
        }

        public async Task SetLights(WLEDClient client, CancellationToken cancellationToken)
        {
            for (int i = 0; i < loopCount; i++)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                for (int x = 0; x < leds.Length; x++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    leds[x].Red = binaryColorMaps[i].OnValue ? binaryColorMaps[i].Color.R : (byte)0;
                    leds[x].Green = binaryColorMaps[i].OnValue ? binaryColorMaps[i].Color.G : (byte)0;
                    leds[x].Blue = binaryColorMaps[i].OnValue ? binaryColorMaps[i].Color.B : (byte)0;
                }

                client.Send(leds);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
