﻿// <auto-generated />
using System;
using FFrelloApi.database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FFrelloApi.Migrations
{
    [DbContext(typeof(FfrelloDbContext))]
    [Migration("20240218220959_AddCardToChecklist")]
    partial class AddCardToChecklist
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("FFrelloApi.Models.Board", b =>
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

            modelBuilder.Entity("FFrelloApi.Models.BoardList", b =>
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

                    b.ToTable("BoardLists");

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

            modelBuilder.Entity("FFrelloApi.Models.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardListId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BoardListName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
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
                            BoardListName = "TODO",
                            Description = "Franks Description of this card",
                            Title = "Franks Card"
                        },
                        new
                        {
                            Id = 2,
                            BoardListId = 2,
                            BoardListName = "In Progress",
                            Description = "Franks Description of the 2nd card",
                            Title = "Franks 2nd Card"
                        },
                        new
                        {
                            Id = 3,
                            BoardListId = 3,
                            BoardListName = "DONE",
                            Description = "Franks Description of the 3nd card",
                            Title = "Franks 3rd Card"
                        });
                });

            modelBuilder.Entity("FFrelloApi.Models.CardChecklist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("CardChecklists");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardChecklistItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardChecklistId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CardChecklistId");

                    b.HasIndex("UserId");

                    b.ToTable("CardChecklistItem");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("UserId");

                    b.ToTable("CardComments");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardWatcher", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "CardId");

                    b.HasIndex("CardId");

                    b.ToTable("CardWatchers");
                });

            modelBuilder.Entity("FFrelloApi.Models.FFrelloRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("FFrelloApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePhotoUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RefreshTokenId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "yluksim9@gmail.com",
                            Name = "Frank M",
                            ProfilePhotoUrl = "https://lh3.googleusercontent.com/a/ACg8ocIJ36231TQrGFILAqYBP5CXuKJhnxqtHt4MJuT7GtUgOg=s96-c",
                            RefreshTokenId = 0
                        });
                });

            modelBuilder.Entity("FFrelloApi.Models.Workspace", b =>
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

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Workspaces");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Frank",
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "Catherine",
                            UserId = 1
                        },
                        new
                        {
                            Id = 3,
                            Name = "M.C.",
                            UserId = 1
                        });
                });

            modelBuilder.Entity("FFrelloApi.Models.Board", b =>
                {
                    b.HasOne("FFrelloApi.Models.Workspace", "Workspace")
                        .WithMany("Boards")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("FFrelloApi.Models.BoardList", b =>
                {
                    b.HasOne("FFrelloApi.Models.Board", "Board")
                        .WithMany("BoardLists")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Board");
                });

            modelBuilder.Entity("FFrelloApi.Models.Card", b =>
                {
                    b.HasOne("FFrelloApi.Models.BoardList", "BoardList")
                        .WithMany("Cards")
                        .HasForeignKey("BoardListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoardList");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardChecklist", b =>
                {
                    b.HasOne("FFrelloApi.Models.Card", "Card")
                        .WithMany("Checklists")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardChecklistItem", b =>
                {
                    b.HasOne("FFrelloApi.Models.CardChecklist", "CardChecklist")
                        .WithMany("Items")
                        .HasForeignKey("CardChecklistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FFrelloApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CardChecklist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardComment", b =>
                {
                    b.HasOne("FFrelloApi.Models.Card", "Card")
                        .WithMany("Comments")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FFrelloApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardWatcher", b =>
                {
                    b.HasOne("FFrelloApi.Models.Card", "Card")
                        .WithMany("CardWatchers")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FFrelloApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FFrelloApi.Models.FFrelloRefreshToken", b =>
                {
                    b.HasOne("FFrelloApi.Models.User", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("FFrelloApi.Models.FFrelloRefreshToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FFrelloApi.Models.User", b =>
                {
                    b.HasOne("FFrelloApi.Models.Card", null)
                        .WithMany("Members")
                        .HasForeignKey("CardId");
                });

            modelBuilder.Entity("FFrelloApi.Models.Workspace", b =>
                {
                    b.HasOne("FFrelloApi.Models.User", "User")
                        .WithMany("Workspaces")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FFrelloApi.Models.Board", b =>
                {
                    b.Navigation("BoardLists");
                });

            modelBuilder.Entity("FFrelloApi.Models.BoardList", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("FFrelloApi.Models.Card", b =>
                {
                    b.Navigation("CardWatchers");

                    b.Navigation("Checklists");

                    b.Navigation("Comments");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("FFrelloApi.Models.CardChecklist", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("FFrelloApi.Models.User", b =>
                {
                    b.Navigation("RefreshToken")
                        .IsRequired();

                    b.Navigation("Workspaces");
                });

            modelBuilder.Entity("FFrelloApi.Models.Workspace", b =>
                {
                    b.Navigation("Boards");
                });
#pragma warning restore 612, 618
        }
    }
}
