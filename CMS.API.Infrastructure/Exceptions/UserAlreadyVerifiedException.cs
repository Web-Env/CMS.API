﻿namespace CMS.API.Infrastructure.Exceptions
{
    public class UserAlreadyVerifiedException : ExceptionBase
    {
        public UserAlreadyVerifiedException(string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
