using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestioneOrdini.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        public required string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        public required string Cognome { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public required string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Inserisci un numero di telefono valido.")]
        public string? Telefono { get; set; }

        [StringLength(100, ErrorMessage = "L'indirizzo non può superare i 100 caratteri.")]
        public string? Indirizzo { get; set; }

        [StringLength(50, ErrorMessage = "La città non può superare i 50 caratteri.")]
        public string? Citta { get; set; }

        [StringLength(2, ErrorMessage = "La provincia deve essere di 2 caratteri.")]
        public string? Provincia { get; set; }

        [StringLength(5, ErrorMessage = "Il CAP deve essere di 5 caratteri.")]
        public string? CAP { get; set; }

        public DateTime DataRegistrazione { get; set; } = DateTime.Now;

        public DateTime? UltimoAccesso { get; set; }

        public string? PasswordHash { get; set; }

        public ICollection<Ordine> Ordini { get; set; } = new List<Ordine>();
    }
}