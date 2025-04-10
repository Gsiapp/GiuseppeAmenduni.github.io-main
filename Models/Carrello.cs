using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace GestioneOrdini.Models
{
    public class Carrello
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string UserId { get; set; }
        
        public DateTime DataCreazione { get; set; }
        
        public virtual ICollection<CarrelloItem> Items { get; set; } = new List<CarrelloItem>();
    }
}
