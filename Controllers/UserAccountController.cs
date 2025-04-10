using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GestioneOrdini.Models;
using GestioneOrdini.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GestioneOrdini.Data;

namespace GestioneOrdini.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly ICarrelloService _carrelloService;
        private readonly AppDbContext _context;
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(
            ICarrelloService carrelloService, 
            AppDbContext context, 
            ILogger<UserAccountController> logger)
        {
            _carrelloService = carrelloService ?? throw new ArgumentNullException(nameof(carrelloService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            return RedirectToAction("Login", "UserAccount");
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica se l'email è già registrata
                    var emailEsistente = await _context.Clienti.AnyAsync(c => c.Email.ToLower() == model.Email.ToLower());
                    if (emailEsistente)
                    {
                        ModelState.AddModelError("Email", "Questa email è già registrata.");
                        return View(model);
                    }

                    // Crea un nuovo cliente
                    var cliente = new Cliente
                    {
                        Nome = model.Nome,
                        Cognome = model.Cognome,
                        Email = model.Email,
                        Telefono = model.Telefono,
                        Indirizzo = model.Indirizzo
                    };

                    // Aggiungi il cliente al database
                    _context.Clienti.Add(cliente);
                    await _context.SaveChangesAsync();

                    // Salva le credenziali
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    CredentialsStore.Clienti[model.Email] = hashedPassword;

                    // Imposta il ruolo e l'ID utente nella sessione
                    HttpContext.Session.SetString("UserRole", "Customer");
                    HttpContext.Session.SetString("UserId", model.Email);

                    // Reindirizza alla pagina dello store
                    return RedirectToAction("Index", "Store");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la registrazione");
                    ViewBag.Error = "Si è verificato un errore durante la registrazione. Riprova più tardi.";
                    return View(model);
                }
            }

            return View(model);
        }
    }
} 