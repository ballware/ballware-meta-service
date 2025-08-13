using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ballware.Meta.Data.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characteristic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    multi = table.Column<bool>(type: "boolean", nullable: true),
                    lookup_id = table.Column<Guid>(type: "uuid", nullable: true),
                    lookup_value_member = table.Column<string>(type: "text", nullable: true),
                    lookup_display_member = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    length = table.Column<int>(type: "integer", nullable: true),
                    characteristic_id = table.Column<Guid>(type: "uuid", nullable: true),
                    characteristic_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: true),
                    required = table.Column<bool>(type: "boolean", nullable: true),
                    @readonly = table.Column<bool>(name: "readonly", type: "boolean", nullable: true),
                    sorting = table.Column<int>(type: "integer", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    sorting = table.Column<int>(type: "integer", nullable: true),
                    register_name = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characteristic_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    report_binary = table.Column<byte[]>(type: "bytea", nullable: true),
                    report_parameter = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documentation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    field = table.Column<string>(type: "text", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documentation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meta = table.Column<bool>(type: "boolean", nullable: false),
                    generated_schema = table.Column<bool>(type: "boolean", nullable: false),
                    no_identity = table.Column<bool>(type: "boolean", nullable: false),
                    application = table.Column<string>(type: "text", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    base_url = table.Column<string>(type: "text", nullable: true),
                    item_mapping_script = table.Column<string>(type: "text", nullable: true),
                    item_reverse_mapping_script = table.Column<string>(type: "text", nullable: true),
                    list_query = table.Column<string>(type: "text", nullable: true),
                    by_id_query = table.Column<string>(type: "text", nullable: true),
                    new_query = table.Column<string>(type: "text", nullable: true),
                    scalar_value_query = table.Column<string>(type: "text", nullable: true),
                    save_statement = table.Column<string>(type: "text", nullable: true),
                    remove_statement = table.Column<string>(type: "text", nullable: true),
                    remove_preliminary_check_script = table.Column<string>(type: "text", nullable: true),
                    list_script = table.Column<string>(type: "text", nullable: true),
                    remove_script = table.Column<string>(type: "text", nullable: true),
                    by_id_script = table.Column<string>(type: "text", nullable: true),
                    before_save_script = table.Column<string>(type: "text", nullable: true),
                    save_script = table.Column<string>(type: "text", nullable: true),
                    lookups = table.Column<string>(type: "text", nullable: true),
                    picklists = table.Column<string>(type: "text", nullable: true),
                    custom_scripts = table.Column<string>(type: "text", nullable: true),
                    grid_layout = table.Column<string>(type: "text", nullable: true),
                    edit_layout = table.Column<string>(type: "text", nullable: true),
                    custom_functions = table.Column<string>(type: "text", nullable: true),
                    state_column = table.Column<string>(type: "text", nullable: true),
                    state_reason_column = table.Column<string>(type: "text", nullable: true),
                    templates = table.Column<string>(type: "text", nullable: true),
                    state_allowed_script = table.Column<string>(type: "text", nullable: true),
                    indices = table.Column<string>(type: "text", nullable: true),
                    provider_model_definition = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_right",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    container = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_right", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "export",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application = table.Column<string>(type: "text", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    query = table.Column<string>(type: "text", nullable: true),
                    expiration_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    media_type = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_export", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "job",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduler = table.Column<string>(type: "text", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    owner = table.Column<Guid>(type: "uuid", nullable: true),
                    options = table.Column<string>(type: "text", nullable: true),
                    result = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lookup",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meta = table.Column<bool>(type: "boolean", nullable: false),
                    has_param = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    list_query = table.Column<string>(type: "text", nullable: true),
                    by_id_query = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lookup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ml_model",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    train_sql = table.Column<string>(type: "text", nullable: true),
                    train_state = table.Column<int>(type: "integer", nullable: false),
                    train_result = table.Column<string>(type: "text", nullable: true),
                    options = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    @params = table.Column<string>(name: "params", type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    @params = table.Column<string>(name: "params", type: "text", nullable: true),
                    finished = table.Column<bool>(type: "boolean", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_trigger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "page",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    layout = table.Column<string>(type: "text", nullable: true),
                    lookups = table.Column<string>(type: "text", nullable: true),
                    picklists = table.Column<string>(type: "text", nullable: true),
                    custom_scripts = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_page", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pickvalue",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    field = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: true),
                    sorting = table.Column<int>(type: "integer", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pickvalue", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processing_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    successors = table.Column<string>(type: "text", nullable: true),
                    record_finished = table.Column<bool>(type: "boolean", nullable: false),
                    record_locked = table.Column<bool>(type: "boolean", nullable: false),
                    reason_required = table.Column<bool>(type: "boolean", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processing_state", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "statistic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity = table.Column<string>(type: "text", nullable: true),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    mapping_script = table.Column<string>(type: "text", nullable: true),
                    custom_scripts = table.Column<string>(type: "text", nullable: true),
                    fetch_sql = table.Column<string>(type: "text", nullable: true),
                    fetch_script = table.Column<string>(type: "text", nullable: true),
                    layout = table.Column<string>(type: "text", nullable: true),
                    meta = table.Column<bool>(type: "boolean", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statistic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mail = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<string>(type: "text", nullable: true),
                    attachment = table.Column<bool>(type: "boolean", nullable: false),
                    attachment_file_name = table.Column<string>(type: "text", nullable: true),
                    notification_id = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    last_send_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    navigation = table.Column<string>(type: "text", nullable: true),
                    rights_check_script = table.Column<string>(type: "text", nullable: true),
                    templates = table.Column<string>(type: "text", nullable: true),
                    server_script_definitions = table.Column<string>(type: "text", nullable: true),
                    managed_database = table.Column<bool>(type: "boolean", nullable: false),
                    provider = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    server = table.Column<string>(type: "text", nullable: true),
                    catalog = table.Column<string>(type: "text", nullable: true),
                    schema = table.Column<string>(type: "text", nullable: true),
                    user = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true),
                    report_schema_definition = table.Column<string>(type: "text", nullable: true),
                    provider_model_definition = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_database_object",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    sql = table.Column<string>(type: "text", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    create_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_changer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_change_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "ix_document_tenant_id",
                table: "document",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_document_tenant_id_uuid",
                table: "document",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_documentation_tenant_id",
                table: "documentation",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_documentation_tenant_id_entity_field",
                table: "documentation",
                columns: new[] { "tenant_id", "entity", "field" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_documentation_tenant_id_uuid",
                table: "documentation",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entity_tenant_id",
                table: "entity",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_entity_tenant_id_uuid",
                table: "entity",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entity_right_tenant_id_entity",
                table: "entity_right",
                columns: new[] { "tenant_id", "entity" });

            migrationBuilder.CreateIndex(
                name: "ix_entity_right_tenant_id_uuid",
                table: "entity_right",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_export_tenant_id",
                table: "export",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_export_tenant_id_uuid",
                table: "export",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_job_tenant_id",
                table: "job",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_tenant_id_owner",
                table: "job",
                columns: new[] { "tenant_id", "owner" });

            migrationBuilder.CreateIndex(
                name: "ix_job_tenant_id_uuid",
                table: "job",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lookup_tenant_id",
                table: "lookup",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_lookup_tenant_id_identifier",
                table: "lookup",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lookup_tenant_id_uuid",
                table: "lookup",
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
                name: "ix_page_tenant_id",
                table: "page",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_page_tenant_id_identifier",
                table: "page",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_page_tenant_id_uuid",
                table: "page",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pickvalue_tenant_id_entity",
                table: "pickvalue",
                columns: new[] { "tenant_id", "entity" });

            migrationBuilder.CreateIndex(
                name: "ix_pickvalue_tenant_id_uuid",
                table: "pickvalue",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_processing_state_tenant_id_entity",
                table: "processing_state",
                columns: new[] { "tenant_id", "entity" });

            migrationBuilder.CreateIndex(
                name: "ix_processing_state_tenant_id_uuid",
                table: "processing_state",
                columns: new[] { "tenant_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statistic_tenant_id",
                table: "statistic",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_statistic_tenant_id_identifier",
                table: "statistic",
                columns: new[] { "tenant_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statistic_tenant_id_uuid",
                table: "statistic",
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

            migrationBuilder.CreateIndex(
                name: "ix_tenant_name",
                table: "tenant",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_uuid",
                table: "tenant",
                column: "uuid",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characteristic");

            migrationBuilder.DropTable(
                name: "characteristic_association");

            migrationBuilder.DropTable(
                name: "characteristic_group");

            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "documentation");

            migrationBuilder.DropTable(
                name: "entity");

            migrationBuilder.DropTable(
                name: "entity_right");

            migrationBuilder.DropTable(
                name: "export");

            migrationBuilder.DropTable(
                name: "job");

            migrationBuilder.DropTable(
                name: "lookup");

            migrationBuilder.DropTable(
                name: "ml_model");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "notification_trigger");

            migrationBuilder.DropTable(
                name: "page");

            migrationBuilder.DropTable(
                name: "pickvalue");

            migrationBuilder.DropTable(
                name: "processing_state");

            migrationBuilder.DropTable(
                name: "statistic");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "tenant");

            migrationBuilder.DropTable(
                name: "tenant_database_object");
        }
    }
}
