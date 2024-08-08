﻿// <auto-generated />
using System;
using HomeworkGB9.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeworkGB9.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20240808132157_ChangeNameRecieve")]
    partial class ChangeNameRecieve
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HomeworkGB9.Model.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp")
                        .HasColumnName("creation_time");

                    b.Property<bool>("IsReceived")
                        .HasColumnType("boolean")
                        .HasColumnName("is_recieved");

                    b.Property<int?>("RecipientId")
                        .HasColumnType("integer")
                        .HasColumnName("recipient_id");

                    b.Property<int?>("SenderId")
                        .HasColumnType("integer")
                        .HasColumnName("sender_id");

                    b.Property<string>("Text")
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.HasKey("Id")
                        .HasName("messages_pkey");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("messages", (string)null);
                });

            modelBuilder.Entity("HomeworkGB9.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("users_pkey");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("HomeworkGB9.Model.Message", b =>
                {
                    b.HasOne("HomeworkGB9.Model.User", "Recipient")
                        .WithMany("ToMessages")
                        .HasForeignKey("RecipientId")
                        .HasConstraintName("messages_to_user_id_fk");

                    b.HasOne("HomeworkGB9.Model.User", "Sender")
                        .WithMany("FromMessages")
                        .HasForeignKey("SenderId")
                        .HasConstraintName("messages_from_user_id_fk");

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("HomeworkGB9.Model.User", b =>
                {
                    b.Navigation("FromMessages");

                    b.Navigation("ToMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
