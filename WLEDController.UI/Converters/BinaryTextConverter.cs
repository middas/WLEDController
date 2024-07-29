using System.Text;

namespace WLEDController.UI.Converters
{
    internal class BinaryTextConverter : ITextConverter
    {
        public bool[] ConvertText(string value)
        {
            if (value == " ")
            {
                return [false, false, false, false, false, false, false, false];
            }

            string binaryString = Encoding.UTF8.GetBytes(value).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Aggregate("", (cur, next) => cur += next);
            bool[] binaryValue = new bool[value.Length * 8];

            for (int i = 0; i < binaryValue.Length; i++)
            {
                binaryValue[i] = binaryString[i] == '1';
            }

            return binaryValue;
        }
    }
}