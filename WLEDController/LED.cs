using System.Drawing;

namespace WLEDController
{
    public sealed class LED
    {
        public LED(int index, Color color) : this(index, color.R, color.G, color.B)
        {
        }

        public LED(int index, byte red, byte green, byte blue) : this(index, red, green, blue, 0)
        {
        }

        public LED(int index, byte red, byte green, byte blue, byte white)
        {
            Index = index;
            Red = red;
            Green = green;
            Blue = blue;
            White = white;
        }

        public byte Blue { get; set; }

        public byte Green { get; set; }

        public int Index { get; set; }

        public byte Red { get; set; }

        public byte White { get; set; }
    }
}