using System;
using System.Globalization;
using System.Net;

namespace Identity.API.Data.System
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public ApiException(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest) 
            : base()
        {
            StatusCode = httpStatusCode;
        }

        public ApiException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = httpStatusCode;
        }

        public ApiException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
            StatusCode = httpStatusCode;
        }
    }
}