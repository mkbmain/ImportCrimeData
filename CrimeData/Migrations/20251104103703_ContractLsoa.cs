using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ContractLsoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LSOAcode",
                table: "ImportCrimeData");

            migrationBuilder.DropColumn(
                name: "LSOAname",
                table: "ImportCrimeData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LSOAcode",
                table: "ImportCrimeData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LSOAname",
                table: "ImportCrimeData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
