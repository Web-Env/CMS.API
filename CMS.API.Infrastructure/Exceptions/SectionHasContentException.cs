namespace CMS.API.Infrastructure.Exceptions
{
    public class SectionHasContentException : ExceptionBase
    {
        public SectionHasContentException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
