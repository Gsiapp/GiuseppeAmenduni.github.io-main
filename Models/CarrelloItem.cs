using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GestioneOrdini.Models
{
    public class CarrelloItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        public int ProdottoId { get; set; }
        
        [Display(Name = "Nome Prodotto")]
        public string NomeProdotto { get; set; } = string.Empty;

        
        [Display(Name = "Quantità")]
        public int Quantita { get; set; }
        
        [Display(Name = "Prezzo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prezzo { get; set; }
        
        [NotMapped]
        public decimal Totale => Prezzo * Quantita;
        
        [ForeignKey("ProdottoId")]
        public virtual Prodotto Prodotto { get; set; } = null!;
    }
}