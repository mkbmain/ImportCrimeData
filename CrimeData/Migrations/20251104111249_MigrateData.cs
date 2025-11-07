using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrateData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("insert  into LastOutComes (Value)\nselect distinct (LastOutcome) from ImportCrimeData");
            migrationBuilder.Sql("update  icd set icd.LastOutcomeId = loc.Id   from ImportCrimeData icd inner join LastOutComes loc on loc.Value = icd.LastOutcome");
            migrationBuilder.Sql(@"DROP VIEW if exists CrimeDataReadable 
go
create view CrimeDataReadable as
select  icd.CrimeID,ct.Value as CrimeType,icd.Longitude,icd.Latitude,loc.Value as location,fw.Value as FallsWithin,rb.Value as ReportedBy,m.Value as Month,lc.Value as lsoaCode,ln.Value as lsoaName,lod.Value as LastOutCome from ImportCrimeData icd
    left outer join Authorities rb on rb.Id = icd.ReportedById
    left outer join Location loc on loc.id = icd.LocationId
        left outer join Authorities fw on fw.Id = icd.FallswithinId
    left outer join LSOAcode lc on lc.id = icd.LSOAcodeId
    left outer join LSOAName ln on ln.id = icd.LSOAnameId
    left outer join LastOutComes lod on lod.id = icd.LastOutcomeId
    inner join month m on m.Id = icd.MonthId
           left outer join CrimeType ct on ct.Id  = icd.CrimeTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP VIEW if exists CrimeDataReadable 
go
create view CrimeDataReadable as
select  icd.CrimeID,ct.Value as CrimeType,icd.Longitude,icd.Latitude,loc.Value as location,fw.Value as FallsWithin,rb.Value as ReportedBy,m.Value as Month,lc.Value as lsoaCode,ln.Value as lsoaName from ImportCrimeData icd
    left outer join Authorities rb on rb.Id = icd.ReportedById
    left outer join Location loc on loc.id = icd.LocationId
        left outer join Authorities fw on fw.Id = icd.FallswithinId
    left outer join LSOAcode lc on lc.id = icd.LSOAcodeId
    left outer join LSOAName ln on ln.id = icd.LSOAnameId
    inner join month m on m.Id = icd.MonthId
           left outer join CrimeType ct on ct.Id  = icd.CrimeTypeId");
        }
    }
}
