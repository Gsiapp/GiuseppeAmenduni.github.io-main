using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(string username, string password)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        try
        {
            var lowerUsername = username?.ToLower();
            Console.WriteLine($"Tentativo di login per username: {lowerUsername}");

            // Controlla admin
            if (CredentialsStore.Admins.Keys.Any(k => k.ToLower() == lowerUsername))
            {
                var adminKey = CredentialsStore.Admins.Keys.First(k => k.ToLower() == lowerUsername);
                Console.WriteLine($"Trovato admin: {adminKey}");
                
                if (BCrypt.Net.BCrypt.Verify(password, CredentialsStore.Admins[adminKey]))
                {
                    Console.WriteLine("Password admin verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Admin");
                    
                    // Debug session
                    var testRole = HttpContext.Session.GetString("UserRole");
                    Console.WriteLine($"Valore sessione admin: {testRole}");
                    
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            // Controlla cliente
            else if (CredentialsStore.Clienti.Keys.Any(k => k.ToLower() == lowerUsername))
            {
                var clienteKey = CredentialsStore.Clienti.Keys.First(k => k.ToLower() == lowerUsername);
                Console.WriteLine($"Trovato cliente: {clienteKey}");
                
                if (BCrypt.Net.BCrypt.Verify(password, CredentialsStore.Clienti[clienteKey]))
                {
                    Console.WriteLine("Password cliente verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Customer");
                    
                    // Debug session
                    var testRole = HttpContext.Session.GetString("UserRole");
                    Console.WriteLine($"Valore sessione cliente: {testRole}");
                    
                    return RedirectToAction("Index", "Store"); // Modificato da Home a Store
                }
            }

            ViewBag.Error = "Credenziali errate";
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRORE DETTAGLIATO - Username: {username} - Errore: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ViewBag.Error = "Errore temporaneo. Riprova più tardi.";
            return View();
        }
    }
}