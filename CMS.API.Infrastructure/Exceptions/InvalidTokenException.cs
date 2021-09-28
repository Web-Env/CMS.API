using CMS.API.Infrastructure.Enums;

namespace CMS.API.Infrastructure.Exceptions
{
    public class InvalidTokenException : ExceptionBase
    {
        public InvalidTokenType InvalidTokenType { get; }
        public InvalidTokenException(InvalidTokenType invalidTokenType, string errorMessage) : base(errorMessage, "The provided token is invalid")
        {
            InvalidTokenType = invalidTokenType;
            ErrorType = nameof(InvalidTokenException);
        }
    }
}
