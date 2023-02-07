﻿// <auto-generated />
using System;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Acropolis.Infrastructure.Contexts.Migrations
{
    [DbContext(typeof(AcropolisContext))]
    [Migration("20221213111642_AddIndexesUniqueCustomers")]
    partial class AddIndexesUniqueCustomers
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Acropolis.Application.Features.Attributes.Attribute", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AttributeKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AttributeKeyDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("AttributeKeyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AttributeKeyIsBeginOpen")
                        .HasColumnType("bit");

                    b.Property<string>("AttributeKeyLabel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AttributeKeyStatus")
                        .HasColumnType("int");

                    b.Property<int>("AttributeKeyType")
                        .HasColumnType("int");

                    b.Property<string>("AttributeValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("AttributeValueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AttributeValueLabel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AttributeValueStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AttributeKey");

                    b.HasIndex("AttributeValue");

                    b.ToTable("Attributes", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Customers.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("nvarchar(14)");

                    b.Property<string>("CustomerCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SellerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Cnpj", "SellerId")
                        .IsUnique();

                    b.ToTable("Customers", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Parameters.Parameter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId", "Code", "Value");

                    b.ToTable("Parameters", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Products.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MaterialCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("SellerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<string>("UnitMeasure")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UnitWeight")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.Promotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DtEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DtStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)");

                    b.Property<Guid>("SellerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<string>("UnitMeasurement")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Promotions", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("AttributesId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionsAttributes", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttributeSku", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PromotionAttributeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PromotionAttributeId");

                    b.ToTable("PromotionsAttributesSKUs", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionCnpj", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("nvarchar(14)");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SellerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.HasIndex("ExternalId", "Cnpj")
                        .IsUnique();

                    b.ToTable("PromotionsCnpjs", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionCnpjDiscountLimit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("nvarchar(14)");

                    b.Property<decimal>("Percent")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("SellerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Cnpj")
                        .IsUnique();

                    b.ToTable("PromotionsCnpjsDiscountLimits", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionParameter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionsParameters", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionRule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("GreaterEqualValue")
                        .HasColumnType("decimal(24,2)");

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(24,2)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TotalAttributes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionsRules", (string)null);
                });

            modelBuilder.Entity("Ziggurat.SqlServer.MessageTracking", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Parameters.Parameter", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Customers.Customer", "Customer")
                        .WithMany("Parameters")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttribute", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.Promotion", "Promotion")
                        .WithMany("Attributes")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttributeSku", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.PromotionAttribute", "PromotionAttribute")
                        .WithMany("SKUs")
                        .HasForeignKey("PromotionAttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PromotionAttribute");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionCnpj", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.Promotion", "Promotion")
                        .WithMany("Cnpjs")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionParameter", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.Promotion", "Promotion")
                        .WithMany("Parameters")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionRule", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.Promotion", "Promotion")
                        .WithMany("Rules")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Customers.Customer", b =>
                {
                    b.Navigation("Parameters");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.Promotion", b =>
                {
                    b.Navigation("Attributes");

                    b.Navigation("Cnpjs");

                    b.Navigation("Parameters");

                    b.Navigation("Rules");
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttribute", b =>
                {
                    b.Navigation("SKUs");
                });
#pragma warning restore 612, 618
        }
    }
}
