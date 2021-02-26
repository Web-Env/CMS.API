namespace CMS.API.Exceptions
{
    public class ExceptionBase
    {
        public string ErrorMessage { get; private set; }
        public string ErrorData { get; private set; }

        public ExceptionBase(string errorMessage, string errorData)
        {
            ErrorMessage = errorMessage;
            ErrorData = errorData;
        }
    }
}
