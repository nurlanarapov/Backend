using System.Net;

namespace Identity.API.Models.Request.Dto.Response
{
    public class Response
    {
        public HttpStatusCode httpStatusCode { get; set; }
        public string message { get; set; }
    }
}