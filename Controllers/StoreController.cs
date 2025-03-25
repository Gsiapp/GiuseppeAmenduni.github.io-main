using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestioneOrdini.Data;
using GestioneOrdini.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GestioneOrdini.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppDbContext _context;

        public StoreController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Store
        public async Task<IActionResult> Index(string categoria, string searchString)
        {
            IQueryable<Prodotto> prodottiQuery = _context.Prodotti;

            // Filtri
            if (!string.IsNullOrEmpty(categoria))
            {
                prodottiQuery = prodottiQuery.Where(p => p.Categoria == categoria);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                prodottiQuery = prodottiQuery.Where(p => 
                    p.Nome.Contains(searchString) || 
                    p.Descrizione.Contains(searchString));
            }

            // Categorie per il dropdown
            ViewBag.Categorie = await _context.Prodotti
                .Select(p => p.Categoria)
                .Distinct()
                .ToListAsync();

            return View(await prodottiQuery.ToListAsync());
        }

        // GET: Store/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var prodotto = await _context.Prodotti
                .FirstOrDefaultAsync(m => m.Id == id);

            return prodotto == null ? NotFound() : View(prodotto);
        }
    }
}