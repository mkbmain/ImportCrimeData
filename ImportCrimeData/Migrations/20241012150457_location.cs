using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportCrimeData.Migrations
{
    /// <inheritdoc />
    public partial class location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "ImportCrimeData");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "ImportCrimeData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportCrimeData_LocationId",
                table: "ImportCrimeData",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportCrimeData_Location_LocationId",
                table: "ImportCrimeData",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
            
            migrationBuilder.Sql(@"
DROP VIEW if exists CrimeDataReadable
go
create view CrimeDataReadable as
select  icd.CrimeID,ct.Value as CrimeType,icd.Longitude,icd.Latitude,loc.Value as location,fw.Value as FallsWithin,rb.Value as ReportedBy,m.Value as Month from ImportCrimeData icd
    left outer join Authorities rb on rb.Id = icd.ReportedById
    left outer join Location loc on loc.id = icd.LocationId
        left outer join Authorities fw on fw.Id = icd.FallswithinId
    inner join month m on m.Id = icd.MonthId
           left outer join CrimeType ct on ct.Id  = icd.CrimeTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportCrimeData_Location_LocationId",
                table: "ImportCrimeData");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropIndex(
                name: "IX_ImportCrimeData_LocationId",
                table: "ImportCrimeData");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ImportCrimeData");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ImportCrimeData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
