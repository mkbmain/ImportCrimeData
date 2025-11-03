using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorities",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrimeType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrimeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Month",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Month", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportCrimeData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrimeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthId = table.Column<short>(type: "smallint", nullable: true),
                    ReportedById = table.Column<short>(type: "smallint", nullable: true),
                    FallswithinId = table.Column<short>(type: "smallint", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LSOAcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LSOAname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrimeTypeId = table.Column<short>(type: "smallint", nullable: true),
                    LastOutcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportCrimeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportCrimeData_Authorities_FallswithinId",
                        column: x => x.FallswithinId,
                        principalTable: "Authorities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportCrimeData_Authorities_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "Authorities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportCrimeData_CrimeType_CrimeTypeId",
                        column: x => x.CrimeTypeId,
                        principalTable: "CrimeType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImportCrimeData_Month_MonthId",
                        column: x => x.MonthId,
                        principalTable: "Month",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_CrimeTypeId",
                table: "ImportCrimeData",
                column: "CrimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_FallswithinId",
                table: "ImportCrimeData",
                column: "FallswithinId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_MonthId",
                table: "ImportCrimeData",
                column: "MonthId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_ReportedById",
                table: "ImportCrimeData",
                column: "ReportedById");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW if exists CrimeDataReadable");
            
            migrationBuilder.DropTable(
                name: "ImportCrimeData");

            migrationBuilder.DropTable(
                name: "Authorities");

            migrationBuilder.DropTable(
                name: "CrimeType");

            migrationBuilder.DropTable(
                name: "Month");
        }
    }
}
