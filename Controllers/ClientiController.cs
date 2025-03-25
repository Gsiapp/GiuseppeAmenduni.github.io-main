using Microsoft.AspNetCore.Mvc;
using GestioneOrdini.Data;
using GestioneOrdini.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GestioneOrdini.Controllers
{
    public class ClientiController : Controller
    {
        private readonly AppDbContext _context;

        public ClientiController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Clienti/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Clienti/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            // Salva il cliente nel database
            _context.Clienti.Add(cliente);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }
        // GET: Clienti
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.SurnameSortParam = sortOrder == "surname" ? "surname_desc" : "surname";

            var clienti = _context.Clienti.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.SearchString = searchString;
                clienti = clienti.Where(c => c.Nome.Contains(searchString) || c.Cognome.Contains(searchString));
            }

            clienti = sortOrder switch
            {
                "name_desc" => clienti.OrderByDescending(c => c.Nome),
                "surname" => clienti.OrderBy(c => c.Cognome),
                "surname_desc" => clienti.OrderByDescending(c => c.Cognome),
                _ => clienti.OrderBy(c => c.Nome),
            };

            int totalRecords = await clienti.CountAsync();
            var clientiPaginati = await clienti.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(clientiPaginati);
        }

        // GET: Clienti/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clienti/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cognome,Email,Telefono, Indirizzo")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                // Recupera l'entità originale dal database
                var originalCliente = await _context.Clienti.FirstOrDefaultAsync(c => c.Id == id);
                if (originalCliente == null)
                {
                    return NotFound();
                }

                // Aggiorna manualmente i campi modificabili
                originalCliente.Nome = cliente.Nome;
                originalCliente.Cognome = cliente.Cognome;
                originalCliente.Email = cliente.Email;
                originalCliente.Telefono = cliente.Telefono;

                // Le modifiche vengono salvate automaticamente
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clienti.Any(e => e.Id == cliente.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Errore di concorrenza: Qualcuno ha già modificato questo cliente. Riprova.");
                    return View(cliente);
                }
            }
        }



        // GET: Clienti/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clienti.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clienti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente != null)
            {
                _context.Clienti.Remove(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}