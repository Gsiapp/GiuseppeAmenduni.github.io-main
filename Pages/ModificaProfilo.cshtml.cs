using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestioneOrdini.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GestioneOrdini.Data;
using System.Linq;

namespace GestioneOrdini.Pages
{
    public class ModificaProfiloModel : PageModel
    {
        private readonly AppDbContext _context;

        [BindProperty]
        public Models.Cliente Cliente { get; set; }

        public ModificaProfiloModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cliente = await _context.Clienti.FirstOrDefaultAsync(m => m.Id == id);

            if (Cliente == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(Cliente.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clienti.Any(e => e.Id == id);
        }
    }
}