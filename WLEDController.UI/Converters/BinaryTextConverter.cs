using System.Collections;
using System.Text;

namespace WLEDController.UI.Converters
{
    internal class BinaryTextConverter : ITextConverter
    {
        public BitArray ConvertText(string value)
        {
            if (value == " ")
            {
                return new BitArray(8, false);
            }

            if (value[0] == 255)
            {
                return new BitArray(8, true);
            }

            string binaryString = Encoding.UTF8.GetBytes(value).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Aggregate("", (cur, next) => cur += next);
            bool[] binaryValue = new bool[value.Length * 8];

            for (int i = 0; i < binaryValue.Length; i++)
            {
                binaryValue[i] = binaryString[i] == '1';
            }

            return new BitArray(binaryValue);
        }
    }
}