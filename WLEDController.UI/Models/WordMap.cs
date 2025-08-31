using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLEDController.UI.Converters;

namespace WLEDController.UI.Models
{
    internal class WordMap(string word, Color color, BitArray lightValues)
    {
        public Color Color { get; } = color;

        public BitArray LightValues { get; } = lightValues;

        public string Word { get; } = word;

        public IEnumerable<LightColorMap> GetLightColorMaps()
        {
            foreach (bool b in LightValues)
            {
                yield return new(Color, b);
            }
        }

        public override string ToString()
        {
            return $"{Word} ({Color})";
        }
    }

    internal readonly struct LightColorMap(Color color, bool onValue)
    {
        public Color Color { get; } = color;

        public bool OnValue { get; } = onValue;

        public override string ToString()
        {
            return $"({Color}) {(OnValue ? "On" : "Off")}";
        }
    }
}
