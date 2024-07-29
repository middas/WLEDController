using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLEDController.UI.Converters
{
    internal interface ITextConverter
    {
        bool[] ConvertText(string value);
    }
}
