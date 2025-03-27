using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

public class AuthorizeAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        
        if (!session.IsAvailable || !session.Keys.Contains("UserRole"))
        {
            // Non loggato -> Redirect al login
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
        }
        else if (session.GetString("UserRole") != "Admin")
        {
            // Loggato come cliente -> Mostra errore
            context.HttpContext.Response.StatusCode = 403;
            context.Result = new ContentResult()
            {
                Content = "<h1 class='text-danger'>ERRORE 403 - NON SEI UN ADMIN</h1>" +
                          "<p>Non hai i permessi per accedere a questa sezione</p>" +
                          "<a href='/Store' class='btn btn-primary'>Torna allo Store</a>",
                ContentType = "text/html"
            };
        }
        
        base.OnActionExecuting(context);
    }
}