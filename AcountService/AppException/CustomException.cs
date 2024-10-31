namespace AcountService.AppException
{
    public class CustomException : Exception
    {
        public int ErrorCode { get; }
        public string CustomMessage { get; }

        public CustomException(string message, int errorCode) : base(message)
        {
            ErrorCode = 400;
            CustomMessage = message;
        }
    }
}
