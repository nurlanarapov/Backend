using System;
using System.Globalization;
using System.Net;

namespace Identity.API.Middlewares.Exceptions
{
    public class ApplicationException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ApplicationException(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
            : base()
        {
            StatusCode = httpStatusCode;
        }

        public ApplicationException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = httpStatusCode;
        }

        public ApplicationException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
            StatusCode = httpStatusCode;
        }
    }
}