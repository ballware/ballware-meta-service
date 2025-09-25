using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ballware.Meta.Data.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveObsoleteDataProviderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characteristic");

            migrationBuilder.DropTable(
                name: "characteristic_association");

            migrationBuilder.DropTable(
                name: "characteristic_group");

            migrationBuilder.DropTable(
                name: "tenant_database_object");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characteristic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lookup_display_member = table.Column<string>(type: "text", nullable: true),
                    lookup_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lookup_value_member = table.Column<string>(type: "text", nullable: true),
                    multi = table.Column<bool>(type: "boolean", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characteristic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characteristic_association",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    active = table.Column<bool>(type: "boolean", nullable: true),
                    characteristic_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    characteristic_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    length = table.Column<int>(type: "integer", nullable: true),
                    @readonly = table.Column<bool>(name: "readonly", type: "boolean", nullable: true),
                    required = table.Column<bool>(type: "boolean", nullable: true),
                    sorting = table.Column<int>(type: "integer", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characteristic_association", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characteristic_group",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    register_name = table.Column<string>(type: "text", nullable: true),
                    sorting = table.Column<int>(type: "integer", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characteristic_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_database_object",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    sql = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_database_object", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_tenant_id",
                table: "characteristic",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_tenant_id_identifier",
                table: "characteristic",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_tenant_id_uuid",
                table: "characteristic",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_association_tenant_id_entity",
                table: "characteristic_association",
                columns: new[] { "tenant_id", "entity" });

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_association_tenant_id_entity_characteristic_",
                table: "characteristic_association",
                columns: new[] { "tenant_id", "entity", "characteristic_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_association_tenant_id_uuid",
                table: "characteristic_association",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_group_tenant_id_entity",
                table: "characteristic_group",
                columns: new[] { "tenant_id", "entity" });

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_group_tenant_id_entity_name",
                table: "characteristic_group",
                columns: new[] { "tenant_id", "entity", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characteristic_group_tenant_id_uuid",
                table: "characteristic_group",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_database_object_tenant_id_type_name",
                table: "tenant_database_object",
                columns: new[] { "tenant_id", "type", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_database_object_tenant_id_uuid",
                table: "tenant_database_object",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);
        }
    }
}
