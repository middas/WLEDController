using System;
using System.Collections;
using System.Drawing;
using System.Text;
using WLEDController.UI.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WLEDController.UI.Converters
{
    internal class BinaryTextConverter : ITextConverter
    {
        private IDisplayMethod? displayMethod;

        public IDisplayMethod DisplayMethod
        {
            get
            {
                if (displayMethod is null)
                {
                    displayMethod = new ScrollingDisplayMethod();
                }

                return displayMethod;
            }
        }

        public BitArray ConvertWord(string value)
        {
            if (value == " ")
            {
                return new BitArray(8, false);
            }

            string binaryString = Encoding.UTF8.GetBytes(value).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Aggregate("", (cur, next) => cur += next);
            bool[] binaryValue = new bool[value.Length * 8];

            for (int i = 0; i < binaryValue.Length; i++)
            {
                binaryValue[i] = binaryString[i] == '1';
            }

            return new BitArray(binaryValue);
        }

        public IEnumerable<WordMap> GetWordMaps(string value)
        {
            string[] words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<WordMap> wordMaps = [];
            Random random = new();

            foreach (string word in words)
            {
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                wordMaps.Add(new(word, color, ConvertWord(word)));

                wordMaps.Add(new(" ", Color.Black, ConvertWord(" ")));
            }

            return wordMaps;
        }

        public BitArray Start()
        {
            return new BitArray(8, true);
        }
    }
}