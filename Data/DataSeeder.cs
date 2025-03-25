using System;
using System.Linq;
using Bogus;
using GestioneOrdini.Data;
using GestioneOrdini.Models;
using static GestioneOrdini.Models.Ordine;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        try
        {
            if (!context.Clienti.Any())
            {
                var clienteFaker = new Faker<Cliente>("it")
                    .RuleFor(c => c.Nome, f => f.Name.FirstName())
                    .RuleFor(c => c.Cognome, f => f.Name.LastName())
                    .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.Nome, c.Cognome))
                    .RuleFor(c => c.Telefono, f => f.Phone.PhoneNumber("+39##########"))
                    .RuleFor(c => c.Indirizzo, f => f.Address.FullAddress());

                context.Clienti.AddRange(clienteFaker.Generate(50));
                context.SaveChanges();
            }

            if (!context.Prodotti.Any())
            {
                var prodottoFaker = new Faker<Prodotto>("it")
                    .RuleFor(p => p.Nome, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Descrizione, f => f.Commerce.ProductDescription())
                    .RuleFor(p => p.Prezzo, f => f.Random.Decimal(10, 1000))
                    .RuleFor(p => p.QuantitaDisponibile, f => f.Random.Int(1, 100))
                    .RuleFor(p => p.ImmagineUrl, f => f.Image.PicsumUrl());

                context.Prodotti.AddRange(prodottoFaker.Generate(10));
                context.SaveChanges();
            }

            if (!context.Ordini.Any())
            {
                var clientiIds = context.Clienti.Select(c => c.Id).ToList();
                var ordineFaker = new Faker<Ordine>("it")
                    .RuleFor(o => o.DataOrdine, f => f.Date.Past())
                    .RuleFor(o => o.Totale, f => f.Random.Decimal(10, 1000))
                    .RuleFor(o => o.Stato, f => f.PickRandom<StatoOrdine>())
                    .RuleFor(o => o.ClienteId, f => f.PickRandom(clientiIds));

                context.Ordini.AddRange(ordineFaker.Generate(200));
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRORE durante il seed: {ex}");
            throw;
        }
    }
}