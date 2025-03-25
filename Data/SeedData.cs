using System.Linq;
using GestioneOrdini.Models;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Prodotti.Any())
            {
                context.Prodotti.AddRange(
                    new Prodotto 
                    { 
                        Nome = "Smartphone Premium", 
                        Descrizione = "128GB RAM, 6.7\" Display", 
                        Prezzo = 799.99m,
                        ImmagineUrl = "https://via.placeholder.com/600x400?text=Smartphone",
                        QuantitaDisponibile = 15,
                        Categoria = "Elettronica"
                    },
                    new Prodotto
                    {
                        Nome = "Cuffie Wireless",
                        Descrizione = "Noise Cancelling, 30h battery",
                        Prezzo = 199.99m,
                        ImmagineUrl = "https://via.placeholder.com/600x400?text=Cuffie",
                        QuantitaDisponibile = 8,
                        Categoria = "Elettronica"
                    },
                    new Prodotto
                    {
                        Nome = "Maglietta Basic",
                        Descrizione = "100% Cotone, Taglia Unica",
                        Prezzo = 29.99m,
                        ImmagineUrl = "https://via.placeholder.com/600x400?text=Maglietta",
                        QuantitaDisponibile = 50,
                        Categoria = "Abbigliamento"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}