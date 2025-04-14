using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il telefono è obbligatorio.")]
        [Phone(ErrorMessage = "Inserisci un numero di telefono valido.")]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'indirizzo è obbligatorio.")]
        [StringLength(100, ErrorMessage = "L'indirizzo non può superare i 100 caratteri.")]
        [Display(Name = "Indirizzo")]
        public string Indirizzo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La città è obbligatoria.")]
        [StringLength(50, ErrorMessage = "La città non può superare i 50 caratteri.")]
        [Display(Name = "Città")]
        public string Citta { get; set; } = string.Empty;

        [Required(ErrorMessage = "La provincia è obbligatoria.")]
        [StringLength(2, ErrorMessage = "La provincia deve essere di 2 caratteri.")]
        [Display(Name = "Provincia")]
        public string Provincia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il CAP è obbligatorio.")]
        [StringLength(5, ErrorMessage = "Il CAP deve essere di 5 caratteri.")]
        [Display(Name = "CAP")]
        public string CAP { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria.")]
        [StringLength(100, ErrorMessage = "La password deve essere di almeno {2} caratteri.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        [Compare("Password", ErrorMessage = "La password e la conferma password non corrispondono.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 