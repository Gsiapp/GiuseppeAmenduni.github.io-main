using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestioneOrdini.Data;
using GestioneOrdini.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GestioneOrdini.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Statistiche
            ViewBag.TotaleOrdini = await _context.Ordini.CountAsync();
            ViewBag.TotaleClienti = await _context.Clienti.CountAsync();
            ViewBag.TotaleProdotti = await _context.Prodotti.CountAsync();
            ViewBag.FatturatoTotale = await _context.Ordini.SumAsync(o => o.Totale);

            // Ultimi ordini
            var ultimiOrdini = await _context.Ordini
                .Include(o => o.Cliente)
                .OrderByDescending(o => o.DataOrdine)
                .Take(5)
                .Select(o => new
                {
                    o.Id,
                    Cliente = o.Cliente.Nome + " " + o.Cliente.Cognome,
                    Data = o.DataOrdine.ToString("dd/MM/yyyy"),
                    o.Totale,
                    StatoEnum = o.Stato,
                    Stato = o.Stato.ToString()
                })
                .ToListAsync();

            
            var ordiniConColore = ultimiOrdini.Select(o => new
            {
                o.Id,
                o.Cliente,
                o.Data,
                o.Totale,
                o.Stato,
                StatoColore = o.StatoEnum switch
                {
                    Ordine.StatoOrdine.InElaborazione => "warning",
                    Ordine.StatoOrdine.Spedito => "info",
                    Ordine.StatoOrdine.Consegnato => "success",
                    Ordine.StatoOrdine.Annullato => "danger",
                    _ => "secondary"
                }
            }).ToList();

            ViewBag.UltimiOrdini = ordiniConColore;

            return View();
        }

        // GET: Home/OrdineDetails/5
        public async Task<IActionResult> OrdineDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini
                .Include(o => o.Cliente) // Include i dettagli del cliente associato
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordine == null)
            {
                return NotFound();
            }

            return View(ordine);
        }

        // GET: Home/CreateOrder
        public IActionResult CreateOrder()
        {
            // Passa la lista dei clienti alla vista per selezionare il cliente associato
            ViewBag.Clienti = _context.Clienti.ToList();
            return View();
        }

        // POST: Home/CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("Id,DataOrdine,Totale,Stato,ClienteId")] Ordine ordine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se il modello non è valido, ricarica la lista dei clienti
            ViewBag.Clienti = _context.Clienti.ToList();
            return View(ordine);
        }

        // GET: Home/EditOrder/5
        public async Task<IActionResult> EditOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine == null)
            {
                return NotFound();
            }

            // Passa la lista dei clienti alla vista per selezionare il cliente associato
            ViewBag.Clienti = _context.Clienti.ToList();
            return View(ordine);
        }

        // POST: Home/EditOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(int id, [Bind("Id,DataOrdine,Totale,Stato,ClienteId")] Ordine ordine)
        {
            if (id != ordine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineExists(ordine.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Se il modello non è valido, ricarica la lista dei clienti
            ViewBag.Clienti = _context.Clienti.ToList();
            return View(ordine);
        }

        // GET: Home/DeleteOrder/5
        public async Task<IActionResult> DeleteOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordine = await _context.Ordini
                .Include(o => o.Cliente) // Include i dettagli del cliente associato
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordine == null)
            {
                return NotFound();
            }

            return View(ordine);
        }

        // POST: Home/DeleteOrder/5
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrderConfirmed(int id)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine != null)
            {
                _context.Ordini.Remove(ordine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrdineExists(int id)
        {
            return _context.Ordini.Any(e => e.Id == id);
        }
    }
}
