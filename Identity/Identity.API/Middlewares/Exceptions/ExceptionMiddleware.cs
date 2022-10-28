using Identity.API.Models.Request.Dto.Response;
using Microsoft.AspNetCore.Http;
using Sentry;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Identity.API.Middlewares.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            Response responseBody = new Response();

            switch (exception)
            {
                case ApplicationException e:
                    responseBody.httpStatusCode = e.StatusCode;
                    responseBody.message = e.Message;
                    // custom application error
                    response.StatusCode = (int)e.StatusCode;
                    break;
                default:
                    responseBody.httpStatusCode = HttpStatusCode.InternalServerError;
                    responseBody.message = "Internal Server Error";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    SentrySdk.CaptureException(exception);
                    break;
            }

            var result = JsonSerializer.Serialize(responseBody);
            await response.WriteAsync(result);
        }
    }
}