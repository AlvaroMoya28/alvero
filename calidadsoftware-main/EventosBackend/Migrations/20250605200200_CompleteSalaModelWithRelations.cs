using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosBackend.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSalaModelWithRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RESERVA_USUARIO_UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA");

            migrationBuilder.DropIndex(
                name: "IX_RESERVA_UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA");

            migrationBuilder.AlterColumn<string>(
                name: "SALT",
                schema: "EVENTOS",
                table: "USUARIO",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "RAW(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CONTRASENA_HASH",
                schema: "EVENTOS",
                table: "USUARIO",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "RAW(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "SALA",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_SALA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOMBRE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPCION_CORTA = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    PRECIO_BASE = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PRECIO_ALTA = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PRECIO_BAJA = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CAPACIDAD = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UBICACION = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IMAGEN_PRINCIPAL = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    HORARIO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ESPACIO = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    DETALLES_PAGO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DESCUENTO = table.Column<int>(type: "NUMBER(10)", nullable: true, defaultValue: 0),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false, defaultValue: "DISPONIBLE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALA", x => x.ID_SALA);
                });

            migrationBuilder.CreateTable(
                name: "SALA_CATERING",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_CATERING = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_SALA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOMBRE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALA_CATERING", x => x.ID_CATERING);
                    table.ForeignKey(
                        name: "FK_CATERING_SALA",
                        column: x => x.ID_SALA,
                        principalSchema: "EVENTOS",
                        principalTable: "SALA",
                        principalColumn: "ID_SALA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALA_EQUIPAMIENTO",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_EQUIPAMIENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_SALA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOMBRE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CANTIDAD = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALA_EQUIPAMIENTO", x => x.ID_EQUIPAMIENTO);
                    table.ForeignKey(
                        name: "FK_EQUIPAMIENTO_SALA",
                        column: x => x.ID_SALA,
                        principalSchema: "EVENTOS",
                        principalTable: "SALA",
                        principalColumn: "ID_SALA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALA_EVENTO",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_EVENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_SALA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOMBRE_EVENTO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALA_EVENTO", x => x.ID_EVENTO);
                    table.ForeignKey(
                        name: "FK_EVENTO_SALA",
                        column: x => x.ID_SALA,
                        principalSchema: "EVENTOS",
                        principalTable: "SALA",
                        principalColumn: "ID_SALA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALA_GALERIA",
                schema: "EVENTOS",
                columns: table => new
                {
                    ID_GALERIA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_SALA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    URL_IMAGEN = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    ORDEN = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALA_GALERIA", x => x.ID_GALERIA);
                    table.ForeignKey(
                        name: "FK_GALERIA_SALA",
                        column: x => x.ID_SALA,
                        principalSchema: "EVENTOS",
                        principalTable: "SALA",
                        principalColumn: "ID_SALA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SALA_CATERING_ID_SALA",
                schema: "EVENTOS",
                table: "SALA_CATERING",
                column: "ID_SALA");

            migrationBuilder.CreateIndex(
                name: "IX_SALA_EQUIPAMIENTO_ID_SALA",
                schema: "EVENTOS",
                table: "SALA_EQUIPAMIENTO",
                column: "ID_SALA");

            migrationBuilder.CreateIndex(
                name: "IX_SALA_EVENTO_ID_SALA",
                schema: "EVENTOS",
                table: "SALA_EVENTO",
                column: "ID_SALA");

            migrationBuilder.CreateIndex(
                name: "IX_SALA_GALERIA_ID_SALA",
                schema: "EVENTOS",
                table: "SALA_GALERIA",
                column: "ID_SALA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SALA_CATERING",
                schema: "EVENTOS");

            migrationBuilder.DropTable(
                name: "SALA_EQUIPAMIENTO",
                schema: "EVENTOS");

            migrationBuilder.DropTable(
                name: "SALA_EVENTO",
                schema: "EVENTOS");

            migrationBuilder.DropTable(
                name: "SALA_GALERIA",
                schema: "EVENTOS");

            migrationBuilder.DropTable(
                name: "SALA",
                schema: "EVENTOS");

            migrationBuilder.AlterColumn<byte[]>(
                name: "SALT",
                schema: "EVENTOS",
                table: "USUARIO",
                type: "RAW(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<byte[]>(
                name: "CONTRASENA_HASH",
                schema: "EVENTOS",
                table: "USUARIO",
                type: "RAW(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RESERVA_UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_RESERVA_USUARIO_UsuarioIdUsuario",
                schema: "EVENTOS",
                table: "RESERVA",
                column: "UsuarioIdUsuario",
                principalSchema: "EVENTOS",
                principalTable: "USUARIO",
                principalColumn: "ID_USUARIO");
        }
    }
}
