using Microsoft.AspNetCore.Mvc;
using GestioneOrdini.Data;
using GestioneOrdini.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace GestioneOrdini.Controllers
{
    public class CarrelloController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CarrelloService _carrelloService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CarrelloController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IHostEnvironment _environment;

        public CarrelloController(
            AppDbContext context, 
            CarrelloService carrelloService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CarrelloController> logger,
            IAntiforgery antiforgery,
            IHostEnvironment environment)
        {
            _context = context;
            _carrelloService = carrelloService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _antiforgery = antiforgery;
            _environment = environment;
        }

        private string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = _httpContextAccessor.HttpContext?.Session.Id;
                _httpContextAccessor.HttpContext?.Session.SetString("TempUserId", userId);
            }
            return userId;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AggiungiAlCarrello([FromBody] AggiungiAlCarrelloModel model)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Accesso richiesto" });
                }

                await _carrelloService.AggiungiProdottoAsync(userId, model.ProdottoId, model.Quantita);
                
                // Aggiorna il contatore nella sessione
                var count = await _carrelloService.GetConteggioCarrelloAsync(userId);
                _httpContextAccessor.HttpContext?.Session?.SetInt32("CarrelloCount", count);
                
                return Json(new { success = true, message = "Prodotto aggiunto al carrello" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore aggiunta al carrello");
                return Json(new { 
                    success = false, 
                    message = _environment.IsDevelopment() ? ex.Message : "Errore durante l'operazione" 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RimuoviDalCarrello([FromBody] RimuoviDalCarrelloModel model)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Accesso richiesto" });
                }

                if (model == null || model.ProdottoId <= 0)
                {
                    return Json(new { success = false, message = "ID prodotto non valido" });
                }

                await _carrelloService.RimuoviProdottoAsync(userId, model.ProdottoId);
                
                // Aggiorna il contatore nella sessione
                var count = await _carrelloService.GetConteggioCarrelloAsync(userId);
                _httpContextAccessor.HttpContext?.Session?.SetInt32("CarrelloCount", count);
                
                return Json(new { success = true, message = "Prodotto rimosso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore rimozione dal carrello");
                return Json(new { 
                    success = false, 
                    message = _environment.IsDevelopment() ? ex.Message : "Errore durante l'operazione" 
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AggiornaQuantita([FromBody] AggiornaQuantitaModel model)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Accesso richiesto" });
                }

                var successo = await _carrelloService.AggiornaQuantitaAsync(userId, model.ProdottoId, model.Quantita);
                return Json(new { 
                    success = successo, 
                    message = successo ? "Quantità aggiornata" : "Prodotto non trovato" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore aggiornamento quantità");
                return Json(new { 
                    success = false, 
                    message = _environment.IsDevelopment() ? ex.Message : "Errore durante l'operazione" 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetContatore()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Json(0);
                }

                var count = await _carrelloService.GetConteggioCarrelloAsync(userId);
                
                // Aggiorna anche la sessione
                _httpContextAccessor.HttpContext?.Session?.SetInt32("CarrelloCount", count);
                
                return Json(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del contatore del carrello");
                return Json(0);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var items = await _carrelloService.GetCarrelloItemsAsync(userId);
            return View(items);
        }

        // Modelli
        public class AggiungiAlCarrelloModel
        {
            public int ProdottoId { get; set; }
            public int Quantita { get; set; }
        }

        public class AggiornaQuantitaModel
        {
            public int ProdottoId { get; set; }
            public int Quantita { get; set; }
        }

        public class RimuoviDalCarrelloModel
        {
            public int ProdottoId { get; set; }
        }
    }
}