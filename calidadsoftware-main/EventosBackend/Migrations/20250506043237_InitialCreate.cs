using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EVENTOS");

            migrationBuilder.CreateTable(
                name: "USUARIO",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOMBRE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    APELLIDO1 = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    APELLIDO2 = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEFONO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    TIPO_USUARIO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "CLIENTE"),
                    CONTRASENA_HASH = table.Column<byte[]>(type: "RAW(255)", maxLength: 255, nullable: false),
                    SALT = table.Column<byte[]>(type: "RAW(100)", maxLength: 100, nullable: false),
                    FECHA_REGISTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ULTIMO_LOGIN = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false, defaultValue: "ACTIVO"),
                    FECHA_NACIMIENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "RESERVA",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IDUSUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IDSALA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVAS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RESERVA_USUARIO",
                        column: x => x.IDUSUARIO,
                        principalSchema: "EVENTOS",
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RESERVA_USUARIO_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalSchema: "EVENTOS",
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RESERVA_IDUSUARIO",
                schema: "EVENTOS",
                table: "RESERVA",
                column: "IDUSUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVA_UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA",
                column: "UsuarioIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RESERVA",
                schema: "EVENTOS");

            migrationBuilder.DropTable(
                name: "USUARIO",
                schema: "EVENTOS");
        }
    }
}
