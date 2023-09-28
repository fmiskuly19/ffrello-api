﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using test.database;

#nullable disable

namespace test.Migrations
{
    [DbContext(typeof(FfrelloDbContext))]
    [Migration("20230928225312_AddWorkspaceColumns")]
    partial class AddWorkspaceColumns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("test.Models.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsStarred")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkspaceId");

                    b.ToTable("Boards");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsStarred = false,
                            Name = "Buffalo Board",
                            WorkspaceId = 1
                        },
                        new
                        {
                            Id = 2,
                            IsStarred = true,
                            Name = "Vermont Board",
                            WorkspaceId = 1
                        },
                        new
                        {
                            Id = 3,
                            IsStarred = false,
                            Name = "Philly Board",
                            WorkspaceId = 1
                        },
                        new
                        {
                            Id = 4,
                            IsStarred = true,
                            Name = "Peaches Board",
                            WorkspaceId = 2
                        },
                        new
                        {
                            Id = 5,
                            IsStarred = false,
                            Name = "Painting Board",
                            WorkspaceId = 2
                        },
                        new
                        {
                            Id = 6,
                            IsStarred = false,
                            Name = "Prill Board",
                            WorkspaceId = 3
                        });
                });

            modelBuilder.Entity("test.Models.BoardList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("BoardList");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BoardId = 3,
                            Name = "TODO"
                        },
                        new
                        {
                            Id = 2,
                            BoardId = 3,
                            Name = "In Progress"
                        },
                        new
                        {
                            Id = 3,
                            BoardId = 3,
                            Name = "DONE"
                        });
                });

            modelBuilder.Entity("test.Models.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardListId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BoardListId");

                    b.ToTable("Cards");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BoardListId = 1,
                            Description = "Franks Description of this card",
                            Name = "Franks Card"
                        },
                        new
                        {
                            Id = 2,
                            BoardListId = 2,
                            Description = "Franks Description of the 2nd card",
                            Name = "Franks 2nd Card"
                        },
                        new
                        {
                            Id = 3,
                            BoardListId = 3,
                            Description = "Franks Description of the 3nd card",
                            Name = "Franks 3rd Card"
                        });
                });

            modelBuilder.Entity("test.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("test.Models.Workspace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Workspaces");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Fwank"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Cafwin"
                        },
                        new
                        {
                            Id = 3,
                            Name = "M.C."
                        });
                });

            modelBuilder.Entity("test.Models.Board", b =>
                {
                    b.HasOne("test.Models.Workspace", "Workspace")
                        .WithMany("Boards")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("test.Models.BoardList", b =>
                {
                    b.HasOne("test.Models.Board", "Board")
                        .WithMany("BoardLists")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("test.Models.Card", b =>
                {
                    b.HasOne("test.Models.BoardList", "BoardList")
                        .WithMany("Cards")
                        .HasForeignKey("BoardListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoardList");
                });

            modelBuilder.Entity("test.Models.User", b =>
                {
                    b.HasOne("test.Models.Card", null)
                        .WithMany("Members")
                        .HasForeignKey("CardId");
                });

            modelBuilder.Entity("test.Models.Board", b =>
                {
                    b.Navigation("BoardLists");
                });

            modelBuilder.Entity("test.Models.BoardList", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("test.Models.Card", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("test.Models.Workspace", b =>
                {
                    b.Navigation("Boards");
                });
#pragma warning restore 612, 618
        }
    }
}
