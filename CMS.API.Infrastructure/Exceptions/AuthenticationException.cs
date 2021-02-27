using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Infrastructure.Exceptions
{
    public class AuthenticationException : ExceptionBase
    {
        public AuthenticationException (string errorMessage, string errorData) : base(errorMessage, errorData) { }
    }
}
