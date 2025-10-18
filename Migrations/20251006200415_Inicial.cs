using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediSchedule_Prototipo.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PuestosSalud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CorreoContacto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EstadoActivo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuestosSalud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoActivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    PuestoSaludId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_PuestosSalud_PuestoSaludId",
                        column: x => x.PuestoSaludId,
                        principalTable: "PuestosSalud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PuestoSaludId",
                table: "Usuarios",
                column: "PuestoSaludId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "PuestosSalud");
        }
    }
}
