using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 

[AuthorizeAdmin]
public class AdminController : Controller
{
    public ActionResult Dashboard()
    {
        // Verifica aggiuntiva per sicurezza
        if (HttpContext.Session.GetString("UserRole") != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }
        return View();
    }

    public ActionResult Orders()
    {
        
        return View();
    }

    public ActionResult Clients()
    {
        
        return View();
    }

    public ActionResult Store()
    {
        
        return View();
    }
}