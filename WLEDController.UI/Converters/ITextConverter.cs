using System.Collections;

namespace WLEDController.UI.Converters
{
    internal interface ITextConverter
    {
        BitArray ConvertText(string value);
    }
}
