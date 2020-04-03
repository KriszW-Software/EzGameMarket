﻿// <auto-generated />
using System;
using CloudGamingSupport.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CloudGamingSupport.API.Migrations
{
    [DbContext(typeof(CGDbContext))]
    partial class CGDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CloudGamingSupport.API.Models.CloudGamingProvider", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SearchURl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Providers");
                });

            modelBuilder.Entity("CloudGamingSupport.API.Models.CloudGamingProvidersAndGames", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CloudGamingProviderID")
                        .HasColumnType("int");

                    b.Property<int>("CloudGamingSupportedID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("CloudGamingProviderID");

                    b.HasIndex("CloudGamingSupportedID");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("CloudGamingSupport.API.Models.CloudGamingSupported", b =>
                {
                    b.Property<int?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProductID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("CloudGamingSupport.API.Models.CloudGamingProvidersAndGames", b =>
                {
                    b.HasOne("CloudGamingSupport.API.Models.CloudGamingProvider", "Provider")
                        .WithMany("SupportedGames")
                        .HasForeignKey("CloudGamingProviderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CloudGamingSupport.API.Models.CloudGamingSupported", "Game")
                        .WithMany("Providers")
                        .HasForeignKey("CloudGamingSupportedID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
