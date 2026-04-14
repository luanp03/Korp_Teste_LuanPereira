using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faturamento.API.Migrations
{
    /// <inheritdoc />
    public partial class attMySQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "ItensNotaFiscal");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "NotasFiscais",
                newName: "QuantidadeTotal");

            migrationBuilder.AddColumn<string>(
                name: "ProdutoCodigo",
                table: "ItensNotaFiscal",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProdutoCodigo",
                table: "ItensNotaFiscal");

            migrationBuilder.RenameColumn(
                name: "QuantidadeTotal",
                table: "NotasFiscais",
                newName: "Numero");

            migrationBuilder.AddColumn<Guid>(
                name: "ProdutoId",
                table: "ItensNotaFiscal",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }
    }
}
