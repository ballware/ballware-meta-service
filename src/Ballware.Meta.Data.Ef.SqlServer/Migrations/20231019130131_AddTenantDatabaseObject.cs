using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    public partial class AddTenantDatabaseObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantDatabaseObject",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Sql = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDatabaseObject", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantDatabaseObject_TenantId_Type_Name",
                table: "TenantDatabaseObject",
                columns: new[] { "TenantId", "Type", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TenantDatabaseObject_TenantId_Uuid",
                table: "TenantDatabaseObject",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantDatabaseObject");
        }
    }
}
