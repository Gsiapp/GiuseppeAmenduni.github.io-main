using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System;

namespace GestioneOrdini.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Il campo username è obbligatorio")]
        public string Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Il campo password è obbligatorio")]
        public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Errore nei dati del form.");
                return Page();
            }

            try
            {
                // Login per Admin
                if (CredentialsStore.Admins.TryGetValue(Username, out var hashedPassword) &&
                    BCrypt.Net.BCrypt.Verify(Password, hashedPassword))
                {
                    Console.WriteLine($"Login riuscito per Admin: {Username}");
                    await SignInUser(Username, "Admin");
                    return RedirectToPage("/Admin/Index");
                }

                // Login per Cliente
                if (CredentialsStore.Clienti.TryGetValue(Username, out hashedPassword) &&
                    BCrypt.Net.BCrypt.Verify(Password, hashedPassword))
                {
                    Console.WriteLine($"Login riuscito per Cliente: {Username}");
                    await SignInUser(Username, "Cliente");
                    return RedirectToPage("/Cliente/Index");
                }

                // Se le credenziali non sono valide
                Console.WriteLine("Credenziali errate.");
                ModelState.AddModelError("", "Credenziali non valide");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il login: {ex.Message}");
            }

            return Page();
        }

        private async Task SignInUser(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
