using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "Ingressos");

            migrationBuilder.DropColumn(
                name: "FilmeId",
                table: "Ingressos");

            migrationBuilder.AddColumn<string>(
                name: "CinemaNome",
                table: "Ingressos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilmeData",
                table: "Ingressos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilmeNome",
                table: "Ingressos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sala",
                table: "Ingressos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CinemaNome",
                table: "Ingressos");

            migrationBuilder.DropColumn(
                name: "FilmeData",
                table: "Ingressos");

            migrationBuilder.DropColumn(
                name: "FilmeNome",
                table: "Ingressos");

            migrationBuilder.DropColumn(
                name: "Sala",
                table: "Ingressos");

            migrationBuilder.AddColumn<int>(
                name: "CinemaId",
                table: "Ingressos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilmeId",
                table: "Ingressos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
