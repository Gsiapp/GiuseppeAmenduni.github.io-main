using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestioneOrdini.Data;
using GestioneOrdini.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestioneOrdini.Controllers
{
    public class OrdiniController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10;

        public OrdiniController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ordini 
       public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1)
{
    // Aggiunti parametri per l'ordinamento dello Stato
    ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";
    ViewBag.TotalSortParam = sortOrder == "total_asc" ? "total_desc" : "total_asc";
    ViewBag.StatoSortParam = sortOrder == "stato_asc" ? "stato_desc" : "stato_asc"; // Nuovo
    ViewBag.CurrentSort = sortOrder;
    ViewBag.SearchString = searchString;

    var ordiniQuery = _context.Ordini.Include(o => o.Cliente).AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
    {
        ordiniQuery = ordiniQuery.Where(o =>
            o.Cliente.Nome.Contains(searchString) || 
            o.Cliente.Cognome.Contains(searchString));
    }

    // Aggiunti casi per l'ordinamento dello Stato
    switch (sortOrder)
    {
        case "date_desc":
            ordiniQuery = ordiniQuery.OrderByDescending(o => o.DataOrdine);
            break;
        case "date_asc":
            ordiniQuery = ordiniQuery.OrderBy(o => o.DataOrdine);
            break;
        case "total_asc":
            ordiniQuery = ordiniQuery.OrderBy(o => o.Totale);
            break;
        case "total_desc":
            ordiniQuery = ordiniQuery.OrderByDescending(o => o.Totale);
            break;
        case "stato_asc":
            ordiniQuery = ordiniQuery.OrderBy(o => o.Stato);
            break;
        case "stato_desc":
            ordiniQuery = ordiniQuery.OrderByDescending(o => o.Stato);
            break;
        default:
            ordiniQuery = ordiniQuery.OrderByDescending(o => o.DataOrdine);
            break;
    }

    // Calcolo pagine e paginazione
    int totalRecords = await ordiniQuery.CountAsync();
    ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / PageSize);
    ViewBag.CurrentPage = page;

    var ordini = await ordiniQuery
        .Skip((page - 1) * PageSize)
        .Take(PageSize)
        .ToListAsync();

    return View(ordini);
}
        // Verifica se un ordine esiste
        private bool OrdineExists(int id)
        {
            return _context.Ordini.Any(o => o.Id == id);
        }

        // GET: Ordini/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ordine = await _context.Ordini
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordine == null) return NotFound();

            return View(ordine);
        }

        // GET: Ordini/Create
        public IActionResult Create()
        {
            ViewBag.Clienti = _context.Clienti.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Nome} {c.Cognome}"
            }).ToList();

            return View();
        }

        // POST: Ordini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataOrdine,Totale,Stato,ClienteId")] Ordine ordine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clienti = _context.Clienti.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Nome} {c.Cognome}"
            }).ToList();

            return View(ordine);
        }

        // GET: Ordini/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine == null) return NotFound();

            ViewBag.Clienti = _context.Clienti.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Nome} {c.Cognome}"
            }).ToList();

            return View(ordine);
        }

        // POST: Ordini/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataOrdine,Totale,Stato,ClienteId")] Ordine ordine)
        {
            if (id != ordine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineExists(ordine.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clienti = _context.Clienti.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Nome} {c.Cognome}"
            }).ToList();

            return View(ordine);
        }

        // GET: Ordini/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ordine = await _context.Ordini
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ordine == null) return NotFound();

            return View(ordine);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine != null)
            {
                _context.Ordini.Remove(ordine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
