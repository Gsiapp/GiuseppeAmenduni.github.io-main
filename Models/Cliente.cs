using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestioneOrdini.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri.")]
        public required string Cognome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Il telefono è obbligatorio.")]
        [Phone(ErrorMessage = "Inserisci un numero di telefono valido.")]
        public required string Telefono { get; set; }

        [Required(ErrorMessage = "L'indirizzo è obbligatorio.")]
        [StringLength(100, ErrorMessage = "L'indirizzo non può superare i 100 caratteri.")]
        public required string Indirizzo { get; set; }

        public ICollection<Ordine> Ordini { get; set; } = new List<Ordine>();
    }
}