﻿// <auto-generated />
using System;
using Ballware.Meta.Data.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Ballware.Meta.Data.Ef.Migrations
{
    [DbContext(typeof(MetaDbContext))]
    [Migration("20231019130131_AddTenantDatabaseObject")]
    partial class AddTenantDatabaseObject
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Ballware.Meta.Data.Characteristic", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LookupDisplayMember")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("LookupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LookupValueMember")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Multi")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Identifier")
                        .IsUnique()
                        .HasFilter("[Identifier] IS NOT NULL");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Characteristic", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.CharacteristicAssociation", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<bool?>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid?>("CharacteristicGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CharacteristicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Length")
                        .HasColumnType("int");

                    b.Property<bool?>("Readonly")
                        .HasColumnType("bit");

                    b.Property<bool?>("Required")
                        .HasColumnType("bit");

                    b.Property<int?>("Sorting")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Entity");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.HasIndex("TenantId", "Entity", "CharacteristicId")
                        .IsUnique()
                        .HasFilter("[Entity] IS NOT NULL AND [CharacteristicId] IS NOT NULL");

                    b.ToTable("CharacteristicAssociation", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.CharacteristicGroup", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RegisterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Sorting")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Entity");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.HasIndex("TenantId", "Entity", "Name")
                        .IsUnique()
                        .HasFilter("[Entity] IS NOT NULL AND [Name] IS NOT NULL");

                    b.ToTable("CharacteristicGroup", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Document", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("ReportBinary")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ReportParameter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Document", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Documentation", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Field")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.HasIndex("TenantId", "Entity", "Field")
                        .IsUnique()
                        .HasFilter("[Entity] IS NOT NULL AND [Field] IS NOT NULL");

                    b.ToTable("Documentation", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Ef.Lookup", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("ByIdQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HasParam")
                        .HasColumnType("bit");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ListQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Meta")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Identifier")
                        .IsUnique();

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Lookup", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Ef.Notification", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Params")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Identifier")
                        .IsUnique();

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Notification", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.EntityMetadata", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Application")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BaseUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BeforeSaveScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ByIdQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ByIdScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomFunctions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomScripts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EditLayout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("GeneratedSchema")
                        .HasColumnType("bit");

                    b.Property<string>("GridLayout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Indices")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemMappingScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ItemReverseMappingScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ListQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lookups")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Meta")
                        .HasColumnType("bit");

                    b.Property<string>("NewQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NoIdentity")
                        .HasColumnType("bit");

                    b.Property<string>("Picklists")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RemovePreliminaryCheckScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RemoveScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RemoveStatement")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SaveScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SaveStatement")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ScalarValueQuery")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateAllowedScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateColumn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StateReasonColumn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Templates")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Entity", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.EntityRight", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Container")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Entity");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("EntityRight", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Export", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Application")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpirationStamp")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MediaType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Query")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Export", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.NotificationTrigger", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Finished")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("NotificationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Params")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("NotificationTrigger", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Page", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomScripts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Layout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lookups")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Picklists")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Identifier")
                        .IsUnique()
                        .HasFilter("[Identifier] IS NOT NULL");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Page", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Pickvalue", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Field")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Sorting")
                        .HasColumnType("int");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Entity");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Pickvalue", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.ProcessingState", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ReasonRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("RecordFinished")
                        .HasColumnType("bit");

                    b.Property<bool>("RecordLocked")
                        .HasColumnType("bit");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Successors")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Entity");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("ProcessingState", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Statistic", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomScripts")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FetchSql")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Layout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MappingScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Meta")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Identifier")
                        .IsUnique()
                        .HasFilter("[Identifier] IS NOT NULL");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Statistic", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Subscription", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<bool>("Attachment")
                        .HasColumnType("bit");

                    b.Property<string>("AttachmentFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Frequency")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastError")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastSendStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("NotificationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Frequency");

                    b.HasIndex("TenantId");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.ToTable("Subscription", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.Tenant", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<string>("Catalog")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("ManagedDatabase")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Navigation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReportSchemaDefinition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RightsCheckScript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Schema")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Server")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServerScriptDefinitions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SessionInitializationSql")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Templates")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("Tenant", (string)null);
                });

            modelBuilder.Entity("Ballware.Meta.Data.TenantDatabaseObject", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreateStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastChangeStamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastChangerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Sql")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Uuid")
                        .IsUnique();

                    b.HasIndex("TenantId", "Type", "Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("TenantDatabaseObject", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
