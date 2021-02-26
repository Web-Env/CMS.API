namespace CMS.API.Exceptions
{
    public class EmailDoesNotExistException : ExceptionBase
    {
        public EmailDoesNotExistException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
