using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLEDController.UI.Converters
{
    internal class MorseCodeTextConverter : ITextConverter
    {
        private static readonly Dictionary<char, bool[]> morseCodeLookup = new()
        {
            {'A', [true, false, true, true, true] },
            {'B', [true, true, true, false, true, false, true, false, true] },
            {'C', [true, true, true, false, true, false, true, true, true, false, true] },
            {'D', [true, true, true, false, true, false, true] },
            {'E', [true] },
            {'F', [true, false, true, false, true, true, true, false, true] },
            {'G', [true, true, true, false, true, true, true, false, true] },
            {'H', [true, false, true, false, true, false, true] },
            {'I', [true, false, true] },
            {'J', [true, false, true, true, true, false, true, true, true, false, true, true, true] },
            {'K', [true, true, true, false, true, false, true, true, true] },
            {'L', [true, false, true, true, true, false, true, false, true] },
            {'M', [true, true, true, false, true, true, true] },
            {'N', [true, true, true, false, true] },
            {'O', [true, true, true, false, true, true, true, false, true, true, true] },
            {'P', [true, false, true, true, true, false, true, true, true, false, true] },
            {'Q', [true, true, true, false, true, true, true, false, true, false, true, true, true] },
            {'R', [true, false, true, true, true, false, true] },
            {'S', [true, false, true, false, true] },
            {'T', [true, true, true] },
            {'U', [true, false, true, false, true, true, true] },
            {'V', [true, false, true, false, true, false, true, true, true] },
            {'W', [true, false, true, true, true, false, true, true, true] },
            {'X', [true, true, true, false, true, false, true, false, true, true, true] },
            {'Y', [true, true, true, false, true, false, true, true, true, false, true, true, true] },
            {'Z', [true, true, true, false, true, true, true, false, true, false, true] },
            {'0', [true, true, true, false, true, true, true, false, true, true, true, false, true, true, true, false, true, true, true] },
            {'1', [true, false, true, true, true, false, true, true, true, false, true, true, true, false, true, true, true] },
            {'2', [true, false, true, false, true, true, true, false, true, true, true, false, true, true, true] },
            {'3', [true, false, true, false, true, false, true, true, true, false, true, true, true] },
            {'4', [true, false, true, false, true, false, true, false, true, true, true] },
            {'5', [true, false, true, false, true, false, true, false, true] },
            {'6', [true, true, true, false, true, false, true, false, true, false, true] },
            {'7', [true, true, true, false, true, true, true, false, true, false, true, false, true] },
            {'8', [true, true, true, false, true, true, true, false, true, true, true, false, true, false, true] },
            {'9', [true, true, true, false, true, true, true, false, true, true, true, false, true, true, true, false, true] },
            {' ', [false, false, false, false, false, false, false] }
        };

        private static bool[] letterSpacing = [false, false, false];

        public bool[] ConvertText(string value)
        {
            return value.ToUpperInvariant().Select(x =>
            {
                if (morseCodeLookup.TryGetValue(x, out bool[]? v))
                {
                    return v;
                }
                return [];
            }).Aggregate(new bool[0], (cur, next) =>
            {
                var list = cur.ToList();
                if (list.Count > 0)
                {
                    list.AddRange(letterSpacing);
                }
                list.AddRange(next);
                return list.ToArray();
            });
        }
    }
}
