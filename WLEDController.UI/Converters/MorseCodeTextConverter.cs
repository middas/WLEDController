namespace WLEDController.UI.Converters
{
    internal class MorseCodeTextConverter : ITextConverter
    {
        private static readonly bool[] dash = [.. dash];
        private static readonly bool dot = true;

        private static readonly Dictionary<char, bool[]> morseCodeLookup = new()
        {
            {'A', [dot, s, ..dash] },
            {'B', [..dash, s, dot, s, dot, s, dot] },
            {'C', [..dash, s, dot, s, ..dash, s, dot] },
            {'D', [..dash, s, dot, s, dot] },
            {'E', [dot] },
            {'F', [dot, s, dot, s, ..dash, s, dot] },
            {'G', [..dash, s, ..dash, s, dot] },
            {'H', [dot, s, dot, s, dot, s, dot] },
            {'I', [dot, s, dot] },
            {'J', [dot, s, ..dash, s, ..dash, s, ..dash] },
            {'K', [..dash, s, dot, s, ..dash] },
            {'L', [dot, s, ..dash, s, dot, s, dot] },
            {'M', [..dash, s, ..dash] },
            {'N', [..dash, s, dot] },
            {'O', [..dash, s, ..dash, s, ..dash] },
            {'P', [dot, s, ..dash, s, ..dash, s, dot] },
            {'Q', [..dash, s, ..dash, s, dot, s, ..dash] },
            {'R', [dot, s, ..dash, s, dot] },
            {'S', [dot, s, dot, s, dot] },
            {'T', [..dash] },
            {'U', [dot, s, dot, s, ..dash] },
            {'V', [dot, s, dot, s, dot, s, ..dash] },
            {'W', [dot, s, ..dash, s, ..dash] },
            {'X', [..dash, s, dot, s, dot, s, ..dash] },
            {'Y', [..dash, s, dot, s, ..dash, s, ..dash] },
            {'Z', [..dash, s, ..dash, s, dot, s, dot] },
            {'0', [..dash, s, ..dash, s, ..dash, s, ..dash, s, ..dash] },
            {'1', [dot, s, ..dash, s, ..dash, s, ..dash, s, ..dash] },
            {'2', [dot, s, dot, s, ..dash, s, ..dash, s, ..dash] },
            {'3', [dot, s, dot, s, dot, s, ..dash, s, ..dash] },
            {'4', [dot, s, dot, s, dot, s, dot, s, ..dash] },
            {'5', [dot, s, dot, s, dot, s, dot, s, dot] },
            {'6', [..dash, s, dot, s, dot, s, dot, s, dot] },
            {'7', [..dash, s, ..dash, s, dot, s, dot, s, dot] },
            {'8', [..dash, s, ..dash, s, ..dash, s, dot, s, dot] },
            {'9', [..dash, s, ..dash, s, ..dash, s, ..dash, s, dot] },
            {' ', [s, s, s, s, s, s, s] },
            {',', [..dash, s, ..dash, s, dot, s, dot, s, ..dash, s, ..dash] },
            {'?', [dot, s, dot, s, ..dash, s, ..dash, s, dot, s, dot] },
            {':', [..dash, s, ..dash, s, ..dash, s, dot, s, dot, s, dot] },
            {'-', [..dash, s, dot, s, dot, s, dot, s, dot, s, ..dash] },
            {'"', [dot, s, ..dash, s, dot, s, dot, s, ..dash, s, dot] },
            {'(', [..dash, s, dot, s, ..dash, s, ..dash, s, dot] },
            {'=', [..dash, s, dot, s, dot, s, dot, s, ..dash] },
            {'.', [dot, s, ..dash, s, dot, s, ..dash, s, dot, s, ..dash] },
            {'/', [..dash, s, dot, s, dot, s, ..dash, s, dot] },
            {'\'', [dot, s, ..dash, s, ..dash, s, ..dash, s, ..dash, s, dot] },
            {'_', [dot, s, dot, s, ..dash, s, ..dash, s, dot, s, ..dash] },
            {')', [..dash, s, dot, s, ..dash, s, ..dash, s, dot, s, ..dash] },
            {'+', [dot, s, ..dash, s, dot, s, ..dash, s, dot] },
            {'@', [dot, s, ..dash, s, ..dash, s, dot, s, ..dash, s, dot] }
        };

        private static readonly bool s = false;
        private static bool[] letterSpacing = [s, s, s];

        public bool[] ConvertText(string value)
        {
            if (value[0] == (char)255)
            {
                return [..dash, s, dot, s, ..dash, s, dot, s, ..dash];
            }

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