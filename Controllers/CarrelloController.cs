using Microsoft.AspNetCore.Mvc;
using GestioneOrdini.Data;
using GestioneOrdini.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace GestioneOrdini.Controllers
{
    public class CarrelloController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CarrelloService _carrelloService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrelloController(
            AppDbContext context, 
            CarrelloService carrelloService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _carrelloService = carrelloService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Se l'utente non è autenticato, crea un ID temporaneo basato sulla sessione
                userId = _httpContextAccessor.HttpContext?.Session.Id;
            }
            return userId;
        }

        public IActionResult Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var carrello = _carrelloService.GetCarrelloUtente(userId);
            return View(carrello);
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
                    return Json(new { success = false, message = "Devi effettuare l'accesso per aggiungere prodotti al carrello" });
                }

                await _carrelloService.AggiungiProdottoAsync(userId, model.ProdottoId, model.Quantita);
                return Json(new { success = true, message = "Prodotto aggiunto al carrello" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Si è verificato un errore durante l'aggiunta al carrello" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RimuoviDalCarrello(int prodottoId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Devi effettuare l'accesso per rimuovere prodotti dal carrello" });
            }

            await _carrelloService.RimuoviProdottoAsync(userId, prodottoId);
            return Json(new { success = true, message = "Prodotto rimosso dal carrello" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AggiornaQuantita(int prodottoId, int quantita)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Devi effettuare l'accesso per aggiornare il carrello" });
            }

            var successo = await _carrelloService.AggiornaQuantitaAsync(userId, prodottoId, quantita);
            return Json(new { 
                success = successo, 
                message = successo ? "Quantità aggiornata" : "Prodotto non trovato" 
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetContatore()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Json(0);
            }

            var totale = await _carrelloService.GetConteggioCarrelloAsync(userId);
            return Json(totale);
        }

        public class AggiungiAlCarrelloModel
        {
            public int ProdottoId { get; set; }
            public int Quantita { get; set; }
        }
    }
}