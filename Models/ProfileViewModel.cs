using System;
using System.ComponentModel.DataAnnotations;


namespace GestioneOrdini.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [Display(Name = "Nome")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [Display(Name = "Cognome")]
        public required string Cognome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Inserisci un numero di telefono valido")]
        [Display(Name = "Telefono")]
        public required string Telefono { get; set; }

        [Display(Name = "Indirizzo")]
        public required string Indirizzo { get; set; }

        [Display(Name = "Città")]
        public required string Citta { get; set; }

        [Display(Name = "Provincia")]
        public required string Provincia { get; set; }

        [Display(Name = "CAP")]
        public required string CAP { get; set; }

        [Display(Name = "Data di Registrazione")]
        public DateTime DataRegistrazione { get; set; }

        [Display(Name = "Ultimo Accesso")]
        public DateTime? UltimoAccesso { get; set; }
    }
} 