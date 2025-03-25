using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestioneOrdini.Pages.Cliente
{
    [Authorize(Policy = "Cliente")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Pagina riservata ai clienti
        }
    }
}