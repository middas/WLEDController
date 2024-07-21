namespace WLEDController.Exceptions
{
    [Serializable]
    public sealed class WLEDClientException : Exception
    {
        public WLEDClientException(string message) : base(message)
        {
        }

        public WLEDClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}