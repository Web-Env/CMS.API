namespace CMS.API.Infrastructure.Exceptions
{
    public class EmailDoesNotExistException : ExceptionBase
    {
        public EmailDoesNotExistException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
