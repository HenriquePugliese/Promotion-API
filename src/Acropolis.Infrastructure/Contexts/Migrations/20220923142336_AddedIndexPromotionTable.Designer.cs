// <auto-generated />
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
    [Migration("20220923142336_AddedIndexPromotionTable")]
    partial class AddedIndexPromotionTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionsAttributes", (string)null);
                });

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttributeSKU", b =>
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

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionParameter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
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

                    b.Property<decimal>("Percentage")
                        .HasColumnType("decimal(24,2)");

                    b.Property<Guid>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TotalAttributes")
                        .HasColumnType("int");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(24,2)");

                    b.HasKey("Id");

                    b.HasIndex("PromotionId");

                    b.ToTable("PromotionsRules", (string)null);
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

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.PromotionAttributeSKU", b =>
                {
                    b.HasOne("Acropolis.Application.Features.Promotions.PromotionAttribute", "PromotionAttribute")
                        .WithMany("SKUs")
                        .HasForeignKey("PromotionAttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PromotionAttribute");
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

            modelBuilder.Entity("Acropolis.Application.Features.Promotions.Promotion", b =>
                {
                    b.Navigation("Attributes");

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
