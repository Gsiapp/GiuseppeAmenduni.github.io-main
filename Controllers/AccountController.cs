using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GestioneOrdini.Models;
using GestioneOrdini.Services;



namespace GestioneOrdini.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICarrelloService _carrelloService;

        public AccountController(ICarrelloService carrelloService)
        {
            _carrelloService = carrelloService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
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

                        // Recupera carrello esistente o creane uno nuovo
                        var carrello = await _carrelloService.GetCarrelloUtenteAsync(clienteKey);
                        HttpContext.Session.SetInt32("CarrelloCount", carrello?.Items.Count ?? 0);

                        // Debug session
                        var testRole = HttpContext.Session.GetString("UserRole");
                        Console.WriteLine($"Valore sessione cliente: {testRole}");
                        
                        return RedirectToAction("Index", "Store"); 
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            
            var model = new ProfileViewModel
            {
                Nome = "Mario",
                Cognome = "Rossi",
                Email = "mario.rossi@example.com",
                Telefono = "3331234567",
                Indirizzo = "Via Roma 123",
                Citta = "Milano",
                Provincia = "MI",
                CAP = "20100",
                DataRegistrazione = DateTime.Now.AddMonths(-6),
                UltimoAccesso = DateTime.Now.AddHours(-2)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                TempData["SuccessMessage"] = "Profilo aggiornato con successo!";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }
    }
}
