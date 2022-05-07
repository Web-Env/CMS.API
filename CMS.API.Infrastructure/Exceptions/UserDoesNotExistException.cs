namespace CMS.API.Infrastructure.Exceptions
{
    public class UserDoesNotExistException : ExceptionBase
    {
        public UserDoesNotExistException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
