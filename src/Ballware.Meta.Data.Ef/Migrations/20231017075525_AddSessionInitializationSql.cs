﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.Migrations
{
    public partial class AddSessionInitializationSql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionInitializationSql",
                table: "Tenant",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionInitializationSql",
                table: "Tenant");
        }
    }
}
