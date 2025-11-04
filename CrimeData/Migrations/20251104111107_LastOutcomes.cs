using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class LastOutcomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "LastOutcomeId",
                table: "ImportCrimeData",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LastOutComes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastOutComes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_LastOutcomeId",
                table: "ImportCrimeData",
                column: "LastOutcomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportCrimeData_LastOutComes_LastOutcomeId",
                table: "ImportCrimeData",
                column: "LastOutcomeId",
                principalTable: "LastOutComes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportCrimeData_LastOutComes_LastOutcomeId",
                table: "ImportCrimeData");

            migrationBuilder.DropTable(
                name: "LastOutComes");

            migrationBuilder.DropIndex(
                name: "IX_ImportCrimeData_LastOutcomeId",
                table: "ImportCrimeData");

            migrationBuilder.DropColumn(
                name: "LastOutcomeId",
                table: "ImportCrimeData");
        }
    }
}
