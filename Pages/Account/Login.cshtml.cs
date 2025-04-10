using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GestioneOrdini.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = string.Empty;

        public void OnGet()
        {
            // Inizializza la pagina
            

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verifica le credenziali
            var lowerUsername = Username?.ToLower();    
            if (CredentialsStore.Admins.Keys.Any(k => k.ToLower() == lowerUsername))
            {
                var adminKey = CredentialsStore.Admins.Keys.First(k => k.ToLower() == lowerUsername);
                if (BCrypt.Net.BCrypt.Verify(Password, CredentialsStore.Admins[adminKey]))
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToPage("/Admin/Dashboard");
                }
            }
            else if (CredentialsStore.Clienti.Keys.Any(k => k.ToLower() == lowerUsername))
            {
                var clienteKey = CredentialsStore.Clienti.Keys.First(k => k.ToLower() == lowerUsername);
                if (BCrypt.Net.BCrypt.Verify(Password, CredentialsStore.Clienti[clienteKey]))
                {
                    HttpContext.Session.SetString("UserRole", "Cliente");
                    return RedirectToPage("/Client/Dashboard");
                }
            }
            
            return RedirectToPage("/Index");
        }
    }
} 