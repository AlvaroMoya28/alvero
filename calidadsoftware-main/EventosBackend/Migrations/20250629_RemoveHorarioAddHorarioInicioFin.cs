using Microsoft.EntityFrameworkCore.Migrations;

namespace EventosBackend.Migrations
{
    public partial class RemoveHorarioAddHorarioInicioFin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HORARIO",
                table: "SALA");

            migrationBuilder.AddColumn<DateTime>(
                name: "HORARIO_INICIO",
                table: "SALA",
                type: "TIMESTAMP",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HORARIO_FIN",
                table: "SALA",
                type: "TIMESTAMP",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HORARIO_INICIO",
                table: "SALA");

            migrationBuilder.DropColumn(
                name: "HORARIO_FIN",
                table: "SALA");

            migrationBuilder.AddColumn<string>(
                name: "HORARIO",
                table: "SALA",
                type: "NVARCHAR2(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
