using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestioneOrdini.Pages.Admin
{
    [Authorize(Policy = "Admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            // Pagina riservata agli amministratori
        }
    }
}