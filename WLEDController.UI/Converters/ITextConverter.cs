using System.Collections;
using WLEDController.UI.Models;

namespace WLEDController.UI.Converters
{
    internal interface ITextConverter
    {
        IEnumerable<WordMap> GetWordMaps(string value);

        BitArray ConvertWord(string value);

        BitArray Start();

        public IDisplayMethod DisplayMethod { get; }
    }
}