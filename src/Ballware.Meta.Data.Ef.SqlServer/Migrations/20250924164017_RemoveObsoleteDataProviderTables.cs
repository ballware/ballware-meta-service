using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveObsoleteDataProviderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characteristic");

            migrationBuilder.DropTable(
                name: "CharacteristicAssociation");

            migrationBuilder.DropTable(
                name: "CharacteristicGroup");

            migrationBuilder.DropTable(
                name: "TenantDatabaseObject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characteristic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LookupDisplayMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LookupValueMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Multi = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicAssociation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    CharacteristicGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CharacteristicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: true),
                    Readonly = table.Column<bool>(type: "bit", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicAssociation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicGroup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegisterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantDatabaseObject",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Sql = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDatabaseObject", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characteristic_TenantId",
                table: "Characteristic",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristic_TenantId_Identifier",
                table: "Characteristic",
                columns: new[] { "TenantId", "Identifier" },
                unique: true,
                filter: "[Identifier] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristic_TenantId_Uuid",
                table: "Characteristic",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicAssociation_TenantId_Entity",
                table: "CharacteristicAssociation",
                columns: new[] { "TenantId", "Entity" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicAssociation_TenantId_Entity_CharacteristicId",
                table: "CharacteristicAssociation",
                columns: new[] { "TenantId", "Entity", "CharacteristicId" },
                unique: true,
                filter: "[Entity] IS NOT NULL AND [CharacteristicId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicAssociation_TenantId_Uuid",
                table: "CharacteristicAssociation",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicGroup_TenantId_Entity",
                table: "CharacteristicGroup",
                columns: new[] { "TenantId", "Entity" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicGroup_TenantId_Entity_Name",
                table: "CharacteristicGroup",
                columns: new[] { "TenantId", "Entity", "Name" },
                unique: true,
                filter: "[Entity] IS NOT NULL AND [Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicGroup_TenantId_Uuid",
                table: "CharacteristicGroup",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

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
    }
}
