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
        private readonly ICredentialsStore _credentialsStore;

        public UserAccountController(
            ICarrelloService carrelloService, 
            AppDbContext context, 
            ILogger<UserAccountController> logger,
            ICredentialsStore credentialsStore)
        {
            _carrelloService = carrelloService ?? throw new ArgumentNullException(nameof(carrelloService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _credentialsStore = credentialsStore ?? throw new ArgumentNullException(nameof(credentialsStore));
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
                if (_credentialsStore.VerifyAdminCredentials(lowerUsername, password))
                {
                    Console.WriteLine("Password admin verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Admin");
                    
                    // Debug session
                    var testRole = HttpContext.Session.GetString("UserRole");
                    Console.WriteLine($"Valore sessione admin: {testRole}");
                    
                    return RedirectToAction("Dashboard", "Admin");
                }
                // Controlla cliente
                else if (await _credentialsStore.VerifyClienteCredentialsAsync(lowerUsername, password))
                {
                    Console.WriteLine("Password cliente verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Customer");
                    HttpContext.Session.SetString("UserId", lowerUsername);

                    // Recupera carrello esistente o creane uno nuovo
                    var carrello = await _carrelloService.GetCarrelloUtenteAsync(lowerUsername);
                    HttpContext.Session.SetInt32("CarrelloCount", carrello?.Items.Count ?? 0);

                    // Debug session
                    var testRole = HttpContext.Session.GetString("UserRole");
                    Console.WriteLine($"Valore sessione cliente: {testRole}");
                    
                    return RedirectToAction("Index", "Store"); 
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
        public async Task<IActionResult> Profile()
        {
            // Verifica se l'utente è autenticato
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            // Recupera il cliente dal database
            var cliente = await _context.Clienti.FirstOrDefaultAsync(c => c.Email == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            // Aggiorna l'ultimo accesso
            cliente.UltimoAccesso = DateTime.Now;
            await _context.SaveChangesAsync();

            // Crea il modello per la vista
            var model = new ProfileViewModel
            {
                Nome = cliente.Nome,
                Cognome = cliente.Cognome,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Indirizzo = cliente.Indirizzo,
                Citta = cliente.Citta,
                Provincia = cliente.Provincia,
                CAP = cliente.CAP,
                DataRegistrazione = cliente.DataRegistrazione,
                UltimoAccesso = cliente.UltimoAccesso
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verifica se l'utente è autenticato
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login");
                }

                // Recupera il cliente dal database
                var cliente = await _context.Clienti.FirstOrDefaultAsync(c => c.Email == userId);
                if (cliente == null)
                {
                    return NotFound();
                }

                // Aggiorna i dati del cliente
                cliente.Nome = model.Nome;
                cliente.Cognome = model.Cognome;
                cliente.Telefono = model.Telefono;
                cliente.Indirizzo = model.Indirizzo;
                cliente.Citta = model.Citta;
                cliente.Provincia = model.Provincia;
                cliente.CAP = model.CAP;

                // Salva le modifiche
                await _context.SaveChangesAsync();
                
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
            if (!ModelState.IsValid)
                return View(model);

            if (await _credentialsStore.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email già registrata");
                return View(model);
            }

            var cliente = new Cliente
            {
                Nome = model.Nome,
                Cognome = model.Cognome,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                DataRegistrazione = DateTime.Now
            };

            await _credentialsStore.AddClienteAsync(cliente);

            return RedirectToAction("Login");
        }
    }
} 