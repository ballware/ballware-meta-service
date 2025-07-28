using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ballware.Meta.Data.Ef.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characteristic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Multi = table.Column<bool>(type: "bit", nullable: true),
                    LookupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LookupValueMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LookupDisplayMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<int>(type: "int", nullable: true),
                    CharacteristicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CharacteristicGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: true),
                    Readonly = table.Column<bool>(type: "bit", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    RegisterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ReportBinary = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ReportParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documentation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Meta = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedSchema = table.Column<bool>(type: "bit", nullable: false),
                    NoIdentity = table.Column<bool>(type: "bit", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemMappingScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemReverseMappingScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByIdQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScalarValueQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaveStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemoveStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemovePreliminaryCheckScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemoveScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByIdScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeforeSaveScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaveScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lookups = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Picklists = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomScripts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GridLayout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditLayout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomFunctions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateReasonColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateAllowedScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indices = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityRight",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Container = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRight", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Export",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Export", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lookup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Meta = table.Column<bool>(type: "bit", nullable: false),
                    HasParam = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByIdQuery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Params = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Params = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Finished = table.Column<bool>(type: "bit", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTrigger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Layout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lookups = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Picklists = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomScripts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pickvalue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sorting = table.Column<int>(type: "int", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickvalue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Successors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordFinished = table.Column<bool>(type: "bit", nullable: false),
                    RecordLocked = table.Column<bool>(type: "bit", nullable: false),
                    ReasonRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistic",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MappingScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomScripts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FetchSql = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Layout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta = table.Column<bool>(type: "bit", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment = table.Column<bool>(type: "bit", nullable: false),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    LastSendStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Navigation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RightsCheckScript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerScriptDefinitions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagedDatabase = table.Column<bool>(type: "bit", nullable: false),
                    Server = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Catalog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportSchemaDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastChangeStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
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
                name: "IX_Document_TenantId",
                table: "Document",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_TenantId_Uuid",
                table: "Document",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documentation_TenantId",
                table: "Documentation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentation_TenantId_Entity_Field",
                table: "Documentation",
                columns: new[] { "TenantId", "Entity", "Field" },
                unique: true,
                filter: "[Entity] IS NOT NULL AND [Field] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Documentation_TenantId_Uuid",
                table: "Documentation",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entity_TenantId",
                table: "Entity",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_TenantId_Uuid",
                table: "Entity",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityRight_TenantId_Entity",
                table: "EntityRight",
                columns: new[] { "TenantId", "Entity" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityRight_TenantId_Uuid",
                table: "EntityRight",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Export_TenantId",
                table: "Export",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Export_TenantId_Uuid",
                table: "Export",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lookup_TenantId",
                table: "Lookup",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lookup_TenantId_Identifier",
                table: "Lookup",
                columns: new[] { "TenantId", "Identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lookup_TenantId_Uuid",
                table: "Lookup",
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
                name: "IX_Page_TenantId",
                table: "Page",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_TenantId_Identifier",
                table: "Page",
                columns: new[] { "TenantId", "Identifier" },
                unique: true,
                filter: "[Identifier] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Page_TenantId_Uuid",
                table: "Page",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pickvalue_TenantId_Entity",
                table: "Pickvalue",
                columns: new[] { "TenantId", "Entity" });

            migrationBuilder.CreateIndex(
                name: "IX_Pickvalue_TenantId_Uuid",
                table: "Pickvalue",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingState_TenantId_Entity",
                table: "ProcessingState",
                columns: new[] { "TenantId", "Entity" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingState_TenantId_Uuid",
                table: "ProcessingState",
                columns: new[] { "TenantId", "Uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_TenantId",
                table: "Statistic",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_TenantId_Identifier",
                table: "Statistic",
                columns: new[] { "TenantId", "Identifier" },
                unique: true,
                filter: "[Identifier] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Statistic_TenantId_Uuid",
                table: "Statistic",
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

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Name",
                table: "Tenant",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Uuid",
                table: "Tenant",
                column: "Uuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characteristic");

            migrationBuilder.DropTable(
                name: "CharacteristicAssociation");

            migrationBuilder.DropTable(
                name: "CharacteristicGroup");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "Documentation");

            migrationBuilder.DropTable(
                name: "Entity");

            migrationBuilder.DropTable(
                name: "EntityRight");

            migrationBuilder.DropTable(
                name: "Export");

            migrationBuilder.DropTable(
                name: "Lookup");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "NotificationTrigger");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Pickvalue");

            migrationBuilder.DropTable(
                name: "ProcessingState");

            migrationBuilder.DropTable(
                name: "Statistic");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
