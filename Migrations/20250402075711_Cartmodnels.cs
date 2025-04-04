using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestioneOrdini.Migrations
{
    /// <inheritdoc />
    public partial class Cartmodnels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantità",
                table: "CarrelloItems",
                newName: "Quantita");

            migrationBuilder.RenameColumn(
                name: "PrezzoUnitario",
                table: "CarrelloItems",
                newName: "Prezzo");

            migrationBuilder.AddColumn<string>(
                name: "NomeProdotto",
                table: "CarrelloItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CarrelloItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeProdotto",
                table: "CarrelloItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CarrelloItems");

            migrationBuilder.RenameColumn(
                name: "Quantita",
                table: "CarrelloItems",
                newName: "Quantità");

            migrationBuilder.RenameColumn(
                name: "Prezzo",
                table: "CarrelloItems",
                newName: "PrezzoUnitario");
        }
    }
}
