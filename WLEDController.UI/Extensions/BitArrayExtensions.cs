using System.Collections;

namespace WLEDController.UI.Extensions
{
    internal static class BitArrayExtensions
    {
        public static IEnumerable<TResult> Select<TResult>(this BitArray bitArray, Func<bool, TResult> selector)
        {
            foreach (bool bit in bitArray)
            {
                yield return selector(bit);
            }
        }
    }
}
