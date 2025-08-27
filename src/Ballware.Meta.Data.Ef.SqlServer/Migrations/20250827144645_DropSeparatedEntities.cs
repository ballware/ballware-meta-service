using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DropSeparatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "MlModel");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "NotificationTrigger");

            migrationBuilder.DropTable(
                name: "Subscription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportBinary = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ReportParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainSql = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainState = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Params = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTrigger",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Finished = table.Column<bool>(type: "bit", nullable: false),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Params = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTrigger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Attachment = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSendStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_TenantId",
                table: "Document",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_TenantId_Uuid",
                table: "Document",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MlModel_TenantId",
                table: "MlModel",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MlModel_TenantId_Identifier",
                table: "MlModel",
                columns: new[] { "TenantId", "Identifier" },
                unique: true,
                filter: "[Identifier] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MlModel_TenantId_Uuid",
                table: "MlModel",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TenantId",
                table: "Notification",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TenantId_Identifier",
                table: "Notification",
                columns: new[] { "TenantId", "Identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TenantId_Uuid",
                table: "Notification",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTrigger_TenantId",
                table: "NotificationTrigger",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTrigger_TenantId_Uuid",
                table: "NotificationTrigger",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Frequency",
                table: "Subscription",
                column: "Frequency");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_TenantId",
                table: "Subscription",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_TenantId_Uuid",
                table: "Subscription",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);
        }
    }
}
