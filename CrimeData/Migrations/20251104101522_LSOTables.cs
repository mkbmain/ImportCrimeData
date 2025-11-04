using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class LSOTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LSOAcodeId",
                table: "ImportCrimeData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LSOAnameId",
                table: "ImportCrimeData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LSOAcode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LSOAcode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LSOAName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LSOAName", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_LSOAcodeId",
                table: "ImportCrimeData",
                column: "LSOAcodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_LSOAnameId",
                table: "ImportCrimeData",
                column: "LSOAnameId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportCrimeData_LSOAName_LSOAnameId",
                table: "ImportCrimeData",
                column: "LSOAnameId",
                principalTable: "LSOAName",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportCrimeData_LSOAcode_LSOAcodeId",
                table: "ImportCrimeData",
                column: "LSOAcodeId",
                principalTable: "LSOAcode",
                principalColumn: "Id");

            migrationBuilder.Sql("insert into LSOAcode (Value)\nselect  distinct (LSOAcode) from ImportCrimeData");
            migrationBuilder.Sql("insert into LSOAName (Value)\nselect  distinct (LSOAName) from ImportCrimeData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportCrimeData_LSOAName_LSOAnameId",
                table: "ImportCrimeData");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportCrimeData_LSOAcode_LSOAcodeId",
                table: "ImportCrimeData");

            migrationBuilder.DropTable(
                name: "LSOAcode");

            migrationBuilder.DropTable(
                name: "LSOAName");

            migrationBuilder.DropIndex(
                name: "IX_ImportCrimeData_LSOAcodeId",
                table: "ImportCrimeData");

            migrationBuilder.DropIndex(
                name: "IX_ImportCrimeData_LSOAnameId",
                table: "ImportCrimeData");

            migrationBuilder.DropColumn(
                name: "LSOAcodeId",
                table: "ImportCrimeData");

            migrationBuilder.DropColumn(
                name: "LSOAnameId",
                table: "ImportCrimeData");
        }
    }
}
