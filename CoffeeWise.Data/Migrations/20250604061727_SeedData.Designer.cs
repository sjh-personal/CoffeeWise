﻿// <auto-generated />
using System;
using CoffeeWise.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoffeeWise.Data.Migrations
{
    [DbContext(typeof(CoffeeWiseDbContext))]
    [Migration("20250604061727_SeedData")]
    partial class SeedData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CoffeeWise.Data.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_groups");

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.GroupMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.Property<Guid>("PersonId")
                        .HasColumnType("uuid")
                        .HasColumnName("person_id");

                    b.HasKey("Id")
                        .HasName("pk_group_members");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_group_members_group_id");

                    b.HasIndex("PersonId", "GroupId")
                        .IsUnique()
                        .HasDatabaseName("ix_group_members_person_id_group_id");

                    b.ToTable("group_members", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.Property<Guid>("PayerGroupMemberId")
                        .HasColumnType("uuid")
                        .HasColumnName("payer_group_member_id");

                    b.HasKey("Id")
                        .HasName("pk_orders");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_orders_group_id");

                    b.HasIndex("PayerGroupMemberId")
                        .HasDatabaseName("ix_orders_payer_group_member_id");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid>("GroupMemberId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_member_id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.HasKey("Id")
                        .HasName("pk_order_items");

                    b.HasIndex("GroupMemberId")
                        .HasDatabaseName("ix_order_items_group_member_id");

                    b.HasIndex("OrderId")
                        .HasDatabaseName("ix_order_items_order_id");

                    b.ToTable("order_items", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("SlackUserId")
                        .HasColumnType("text")
                        .HasColumnName("slack_user_id");

                    b.HasKey("Id")
                        .HasName("pk_persons");

                    b.ToTable("persons", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Presence", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.Property<bool>("IsPresent")
                        .HasColumnType("boolean")
                        .HasColumnName("is_present");

                    b.Property<Guid>("PersonId")
                        .HasColumnType("uuid")
                        .HasColumnName("person_id");

                    b.HasKey("Id")
                        .HasName("pk_presences");

                    b.HasIndex("PersonId", "Date")
                        .IsUnique()
                        .HasDatabaseName("ix_presences_person_id_date");

                    b.ToTable("presences", (string)null);
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.GroupMember", b =>
                {
                    b.HasOne("CoffeeWise.Data.Entities.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_members_groups_group_id");

                    b.HasOne("CoffeeWise.Data.Entities.Person", "Person")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_members_persons_person_id");

                    b.Navigation("Group");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Order", b =>
                {
                    b.HasOne("CoffeeWise.Data.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_orders_groups_group_id");

                    b.HasOne("CoffeeWise.Data.Entities.GroupMember", "PayerGroupMember")
                        .WithMany()
                        .HasForeignKey("PayerGroupMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_orders_group_members_payer_group_member_id");

                    b.Navigation("Group");

                    b.Navigation("PayerGroupMember");
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.OrderItem", b =>
                {
                    b.HasOne("CoffeeWise.Data.Entities.GroupMember", "GroupMember")
                        .WithMany()
                        .HasForeignKey("GroupMemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_order_items_group_members_group_member_id");

                    b.HasOne("CoffeeWise.Data.Entities.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_items_orders_order_id");

                    b.Navigation("GroupMember");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Group", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("CoffeeWise.Data.Entities.Person", b =>
                {
                    b.Navigation("GroupMemberships");
                });
#pragma warning restore 612, 618
        }
    }
}
