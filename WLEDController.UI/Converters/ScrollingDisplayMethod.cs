using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WLEDController.UI.Extensions;
using WLEDController.UI.Models;

namespace WLEDController.UI.Converters
{
    internal class ScrollingDisplayMethod : IDisplayMethod
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

            binaryColorMaps = [.. start.Select(x => new LightColorMap(Color.Teal, x)), .. wordMaps.SelectMany(x => x.GetLightColorMaps())];

            loopCount = binaryColorMaps.Length + numberOfLights;
            this.delay = delay;
        }

        public async Task SetLights(WLEDClient client, CancellationToken cancellationToken)
        {
            for (int offset = 0; offset < loopCount; offset++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                int binaryColorIndex = 0;

                for (int i = offset; i >= 0; i--)
                {
                    try
                    {
                        if (binaryColorIndex >= binaryColorMaps.Length || cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (i >= leds.Length)
                        {
                            binaryColorIndex++;
                            continue;
                        }

                        leds[i].Red = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.R : (byte)0;
                        leds[i].Green = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.G : (byte)0;
                        leds[i].Blue = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.B : (byte)0;

                        binaryColorIndex++;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                _ = client.Send(leds);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}

