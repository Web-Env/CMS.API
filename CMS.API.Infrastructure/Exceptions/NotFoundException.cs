namespace CMS.API.Infrastructure.Exceptions
{
    public class NotFoundException : ExceptionBase
    {
        public NotFoundException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
