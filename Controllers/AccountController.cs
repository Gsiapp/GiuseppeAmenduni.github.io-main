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
    public class AccountController : Controller
    {
        private readonly ICarrelloService _carrelloService;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly ICredentialsStore _credentialsStore;

        public AccountController(
            ICarrelloService carrelloService, 
            AppDbContext context, 
            ILogger<AccountController> logger,
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
                if (_credentialsStore.VerifyAdminCredentials(username, password))
                {
                    Console.WriteLine("Password admin verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Admin");
                    
                    // Debug session
                    var testRole = HttpContext.Session.GetString("UserRole");
                    Console.WriteLine($"Valore sessione admin: {testRole}");
                    
                    return RedirectToAction("Dashboard", "Admin");
                }
                // Controlla cliente
                else if (await _credentialsStore.VerifyClienteCredentialsAsync(username, password))
                {
                    Console.WriteLine("Password cliente verificata correttamente");
                    HttpContext.Session.SetString("UserRole", "Customer");

                    // Recupera carrello esistente o creane uno nuovo
                    var carrello = await _carrelloService.GetCarrelloUtenteAsync(username);
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
            return RedirectToAction("Login", "Account");
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
            try
            {
                _logger.LogInformation("Inizio processo di registrazione");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modello non valido. Errori di validazione:");
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning($"- {modelError.ErrorMessage}");
                    }
                    return View(model);
                }

                _logger.LogInformation($"Verifica esistenza email: {model.Email}");
                
                // Verifica se l'email è già registrata
                if (await _credentialsStore.EmailExistsAsync(model.Email))
                {
                    _logger.LogWarning($"Email già registrata: {model.Email}");
                    ModelState.AddModelError("Email", "Questa email è già registrata.");
                    return View(model);
                }

                _logger.LogInformation("Creazione nuovo cliente");

                // Crea un nuovo cliente
                var cliente = new Cliente
                {
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    Indirizzo = model.Indirizzo,
                    Citta = model.Citta,
                    Provincia = model.Provincia,
                    CAP = model.CAP,
                    DataRegistrazione = DateTime.Now,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _logger.LogInformation("Tentativo di aggiunta cliente nel database");

                // Aggiungi il cliente usando il CredentialsStore
                await _credentialsStore.AddClienteAsync(cliente);

                _logger.LogInformation("Cliente aggiunto con successo. Impostazione sessione");

                // Imposta il ruolo e l'ID utente nella sessione
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("UserId", model.Email);
                HttpContext.Session.SetInt32("ClienteId", cliente.Id);

                _logger.LogInformation($"Registrazione completata con successo per: {model.Email}");

                return RedirectToAction("Index", "Store");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERRORE DETTAGLIATO durante la registrazione del cliente {model.Email}:");
                _logger.LogError($"Tipo di errore: {ex.GetType().Name}");
                _logger.LogError($"Messaggio: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                    _logger.LogError($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }

                ViewBag.Error = "Si è verificato un errore durante la registrazione. Riprova più tardi.";
                return View(model);
            }
        }
    }
}
