using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faturamento.API.Migrations
{
    /// <inheritdoc />
    public partial class ComplementItensNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Preco",
                table: "ItensNotaFiscal",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProdutoNome",
                table: "ItensNotaFiscal",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preco",
                table: "ItensNotaFiscal");

            migrationBuilder.DropColumn(
                name: "ProdutoNome",
                table: "ItensNotaFiscal");
        }
    }
}
