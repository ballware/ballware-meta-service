using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    public partial class RemoveTenantSessionInitializationSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionInitializationSql",
                table: "Tenant");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionInitializationSql",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
