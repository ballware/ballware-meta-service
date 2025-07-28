using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    public partial class AddTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Templates",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Templates",
                table: "Entity",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Templates",
                table: "Tenant");

            migrationBuilder.DropColumn(
                name: "Templates",
                table: "Entity");
        }
    }
}
