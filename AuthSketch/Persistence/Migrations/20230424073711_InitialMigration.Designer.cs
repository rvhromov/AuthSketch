﻿// <auto-generated />
using System;
using AuthSketch.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthSketch.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230424073711_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AuthSketch.Entities.ExternalAuth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte>("Provider")
                        .HasColumnType("smallint");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExternalAuthProviders", (string)null);
                });

            modelBuilder.Entity("AuthSketch.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1L, null, null, null, null, null);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ReplacedByToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RevokedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RevokedByIp")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens", (string)null);
                });

            modelBuilder.Entity("AuthSketch.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 1L, null, null, null, null, null);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsTfaEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("ResetToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ResetTokenExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ResetTokenUsedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte>("Role")
                        .HasColumnType("smallint");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<DateTime?>("TfaEnabledAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TfaKey")
                        .HasColumnType("text");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3437),
                            Email = "admin@auth.com",
                            IsTfaEnabled = false,
                            Name = "Admin",
                            PasswordHash = "8AA0FCD19E374352DA62AEA1F176E8ACBBE3C4B9ECF7B5520EF514E3C0CDAE5C9195D6DC39580919EA569FD4B14C5A5F2DB0A882BE221E4A597BC6F8CE5AEA5D",
                            Role = (byte)1,
                            Salt = "6A5EF455C2FF6F3C554831666058A1943C0252ED0FC47BF4C1B3E39462F575A45CE8D3688C65E566FCBDABE3309DA91511051668E4B0EC6278E7D10BFF3A2B95",
                            VerificationToken = "DB96319BCB57D86D22C1AF2478F65AF198681171E97040E6552D2603C6C413E0CE7F62D7CCC8F90A106AF5C4F1867A35C7AC291E452D823EC797BDCC7F7146D9",
                            VerifiedAt = new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3440)
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3442),
                            Email = "user@auth.com",
                            IsTfaEnabled = false,
                            Name = "User",
                            PasswordHash = "8AA0FCD19E374352DA62AEA1F176E8ACBBE3C4B9ECF7B5520EF514E3C0CDAE5C9195D6DC39580919EA569FD4B14C5A5F2DB0A882BE221E4A597BC6F8CE5AEA5D",
                            Role = (byte)0,
                            Salt = "6A5EF455C2FF6F3C554831666058A1943C0252ED0FC47BF4C1B3E39462F575A45CE8D3688C65E566FCBDABE3309DA91511051668E4B0EC6278E7D10BFF3A2B95",
                            VerificationToken = "DB96319BCB57D86D22C1AF2478F65AF198681171E97040E6552D2603C6C413E0CE7F62D7CCC8F90A106AF5C4F1867A35C7AC291E452D823EC797BDCC7F7146D9",
                            VerifiedAt = new DateTime(2023, 4, 24, 7, 37, 11, 381, DateTimeKind.Utc).AddTicks(3442)
                        });
                });

            modelBuilder.Entity("AuthSketch.Entities.ExternalAuth", b =>
                {
                    b.HasOne("AuthSketch.Entities.User", null)
                        .WithMany("ExternalAuthProviders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthSketch.Entities.RefreshToken", b =>
                {
                    b.HasOne("AuthSketch.Entities.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthSketch.Entities.User", b =>
                {
                    b.Navigation("ExternalAuthProviders");

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
