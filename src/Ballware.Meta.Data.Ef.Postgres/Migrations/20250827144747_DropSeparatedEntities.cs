using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ballware.Meta.Data.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class DropSeparatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "ml_model");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "notification_trigger");

            migrationBuilder.DropTable(
                name: "subscription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    report_binary = table.Column<byte[]>(type: "bytea", nullable: true),
                    report_parameter = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ml_model",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    options = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    train_result = table.Column<string>(type: "text", nullable: true),
                    train_sql = table.Column<string>(type: "text", nullable: true),
                    train_state = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ml_model", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    identifier = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    @params = table.Column<string>(name: "params", type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_trigger",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    finished = table.Column<bool>(type: "boolean", nullable: false),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    @params = table.Column<string>(name: "params", type: "text", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_trigger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    attachment = table.Column<bool>(type: "boolean", nullable: false),
                    attachment_file_name = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<string>(type: "text", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    frequency = table.Column<int>(type: "integer", nullable: false),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    last_send_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    mail = table.Column<string>(type: "text", nullable: true),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_document_tenant_id",
                table: "document",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_document_tenant_id_uuid",
                table: "document",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ml_model_tenant_id",
                table: "ml_model",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ml_model_tenant_id_identifier",
                table: "ml_model",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ml_model_tenant_id_uuid",
                table: "ml_model",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_tenant_id",
                table: "notification",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_tenant_id_identifier",
                table: "notification",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_tenant_id_uuid",
                table: "notification",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notification_trigger_tenant_id",
                table: "notification_trigger",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_notification_trigger_tenant_id_uuid",
                table: "notification_trigger",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_frequency",
                table: "subscription",
                column: "frequency");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_tenant_id",
                table: "subscription",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_tenant_id_uuid",
                table: "subscription",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);
        }
    }
}
