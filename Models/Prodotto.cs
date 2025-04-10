using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Models
{
    public class Prodotto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal Prezzo { get; set; }

        [Display(Name = "Immagine")]
        public string ImmagineUrl { get; set; } = string.Empty;

        [Display(Name = "Disponibilità")]
        public int QuantitaDisponibile { get; set; }

        public string Categoria { get; set; } = "Generica";
    }
}