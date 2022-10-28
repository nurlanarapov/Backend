using Identity.API.Middlewares.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Identity.API.Filters
{
    public class HeaderFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.AcceptLanguage.Count == 0)
                throw new ApplicationException("Не найден язык", HttpStatusCode.BadRequest);
        }
    }
}