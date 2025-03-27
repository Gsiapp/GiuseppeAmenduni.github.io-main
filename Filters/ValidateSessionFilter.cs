using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

public class ValidateSessionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var excludedControllers = new[] { "Account", "Session" };

        if (!excludedControllers.Contains(controllerName))
        {
            var session = context.HttpContext.Session;
            if (!session.IsAvailable || !session.Keys.Contains("UserRole"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) 
    {
    }
}