using System;
using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Models
{
    public class Ordine
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataOrdine { get; set; } = DateTime.UtcNow; // Inizializza con la data attuale

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Il totale non può essere negativo.")]
        public decimal Totale { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public required Cliente Cliente { get; set; }

        [Required]
        public StatoOrdine Stato { get; set; } 

        public enum StatoOrdine
        {
            InElaborazione,
            Spedito,
            Consegnato,
            Annullato
        }
    }
}
