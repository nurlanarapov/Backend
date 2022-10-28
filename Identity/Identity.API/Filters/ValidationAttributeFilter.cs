using Identity.API.Middlewares.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Identity.API.Filters
{
    public class ValidationAttributeFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new ApplicationException(context.ModelState.Values.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage ?? "Error not found");
            }
        }
    }
}