namespace CMS.API.Infrastructure.Exceptions
{
    public class ExceptionBase : System.Exception
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
