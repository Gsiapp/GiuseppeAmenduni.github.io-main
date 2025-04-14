using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestioneOrdini.Services;

namespace GestioneOrdini.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ICredentialsStore _credentialsStore;

        public LoginModel(ICredentialsStore credentialsStore)
        {
            _credentialsStore = credentialsStore;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Inizializza la pagina
            

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Inserisci username e password";
                return Page();
            }

            var lowerUsername = Username.ToLower();

            // Verifica credenziali admin
            if (_credentialsStore.VerifyAdminCredentials(lowerUsername, Password))
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToPage("/Admin/Dashboard");
            }

            // Verifica credenziali cliente
            if (await _credentialsStore.VerifyClienteCredentialsAsync(lowerUsername, Password))
            {
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("UserId", lowerUsername);
                return RedirectToPage("/Store/Index");
            }

            ErrorMessage = "Credenziali non valide";
            return Page();
        }
    }
} 