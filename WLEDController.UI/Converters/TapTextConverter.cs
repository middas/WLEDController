using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLEDController.UI.Models;

namespace WLEDController.UI.Converters
{
    internal class TapTextConverter : ITextConverter
    {
        private readonly Dictionary<char, Tuple<int, int>> tapLookup = new()
        {
            {'A', new(1,1) },
            {'B', new(1,2) },
            {'C', new(1,3) },
            {'D', new(1,4) },
            {'E', new(1,5) },
            {'F', new(1,6) },
            {'G', new(2,1) },
            {'H', new(2,2) },
            {'I', new(2,3) },
            {'J', new(2,4) },
            {'K', new(2,5) },
            {'L', new(2,6) },
            {'M', new(3,1) },
            {'N', new(3,2) },
            {'O', new(3,3) },
            {'P', new(3,4) },
            {'Q', new(3,5) },
            {'R', new(3,6) },
            {'S', new(4,1) },
            {'T', new(4,2) },
            {'U', new(4,3) },
            {'V', new(4,4) },
            {'W', new(4,5) },
            {'X', new(4,6) },
            {'Y', new(5,1) },
            {'Z', new(5,2) },
            {'0', new(5,3) },
            {'1', new(5,4) },
            {'2', new(5,5) },
            {'3', new(5,6) },
            {'4', new(6,1) },
            {'5', new(6,2) },
            {'6', new(6,3) },
            {'7', new(6,4) },
            {'8', new(6,5) },
            {'9', new(6,6) },
            {' ', new(-1,-1) }
        };

        private const bool blank = false;

        private bool isColumn = false;

        private IDisplayMethod? displayMethod;

        public IDisplayMethod DisplayMethod
        {
            get
            {
                if(displayMethod is null)
                {
                    displayMethod = new FlashDisplayMethod();
                }

                return displayMethod;
            }
        }

        public BitArray ConvertWord(string value)
        {
            return new BitArray(value.ToUpperInvariant().Select(x =>
            {
                if (tapLookup.TryGetValue(x, out Tuple<int, int>? value))
                {
                    if (value.Item1 < 0)
                    {
                        return [blank];
                    }

                    List<bool> result = [];
                    if (!isColumn)
                    {
                        for (int i = 0; i < value.Item1; i++)
                        {
                            result.Add(true);
                            result.Add(false);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < value.Item2; i++)
                        {
                            result.Add(true);
                            result.Add(false);
                        }
                    }

                    return result.ToArray();
                }
                else
                {
                    return [];
                }
            }).Aggregate(Array.Empty<bool>(), (cur, next) =>
            {
                List<bool> list = [.. cur];
                if (list.Count > 0 && next.Length > 0)
                {
                    list.Add(blank);
                }

                if (next.Length > 0)
                {
                    list.AddRange(next);
                }
                return [.. list];
            }));
        }

        public BitArray Start()
        {
            return new BitArray(4, false);
        }

        public IEnumerable<WordMap> GetWordMaps(string value)
        {
            string[] words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<WordMap> wordMaps = [];
            Random random = new();

            foreach (string word in words)
            {
                Color colorRow = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                Color colorColumn = Color.FromArgb(255 - colorRow.R, 255 - colorRow.G, 255 - colorRow.B);

                wordMaps.AddRange(word.SelectMany(c =>
                {
                    WordMap[] result = new WordMap[2];
                    isColumn = false;
                    result[0] = new(c.ToString(), colorRow, ConvertWord(c.ToString()));
                    isColumn = true;
                    result[1] = new(c.ToString(), colorColumn, ConvertWord(c.ToString()));

                    return result;
                }));

                wordMaps.Add(new(" ", Color.Black, ConvertWord(" ")));
            }

            return wordMaps;
        }
    }
}
