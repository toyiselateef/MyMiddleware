public class BadRequestException : Exception
    {
        public int StatusCode = 400;
        public BadRequestException(string message)
        : base(message)
        {

        }
    }