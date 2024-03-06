﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NauraaBot.Database;

#nullable disable

namespace NauraaBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.27");

            modelBuilder.Entity("NauraaBot.Database.Models.Card", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrentFactionID")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("TEXT");

                    b.Property<string>("MainFactionID")
                        .HasColumnType("TEXT");

                    b.Property<string>("RarityID")
                        .HasColumnType("TEXT");

                    b.Property<string>("SetID")
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CurrentFactionID");

                    b.HasIndex("MainFactionID");

                    b.HasIndex("RarityID");

                    b.HasIndex("SetID");

                    b.HasIndex("TypeID");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("NauraaBot.Database.Models.CardSet", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("CardSet");
                });

            modelBuilder.Entity("NauraaBot.Database.Models.CardType", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("CardType");

                    b.HasData(
                        new
                        {
                            ID = "HERO",
                            Name = "Hero"
                        },
                        new
                        {
                            ID = "SPELL",
                            Name = "Spell"
                        },
                        new
                        {
                            ID = "PERMANENT",
                            Name = "Permanent"
                        });
                });

            modelBuilder.Entity("NauraaBot.Database.Models.Faction", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Faction");

                    b.HasData(
                        new
                        {
                            ID = "AX",
                            Name = "Axiom"
                        },
                        new
                        {
                            ID = "BR",
                            Name = "Bravos"
                        },
                        new
                        {
                            ID = "LY",
                            Name = "Lyra"
                        },
                        new
                        {
                            ID = "MU",
                            Name = "Muna"
                        },
                        new
                        {
                            ID = "OR",
                            Name = "Ordis"
                        },
                        new
                        {
                            ID = "YZ",
                            Name = "Yzmir"
                        });
                });

            modelBuilder.Entity("NauraaBot.Database.Models.Rarity", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Short")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Rarity");

                    b.HasData(
                        new
                        {
                            ID = "COMMON",
                            Name = "Common",
                            Short = "C"
                        },
                        new
                        {
                            ID = "RARE",
                            Name = "Rare",
                            Short = "R"
                        },
                        new
                        {
                            ID = "UNIQUE",
                            Name = "Unique",
                            Short = "U"
                        });
                });

            modelBuilder.Entity("NauraaBot.Database.Models.Card", b =>
                {
                    b.HasOne("NauraaBot.Database.Models.Faction", "CurrentFaction")
                        .WithMany()
                        .HasForeignKey("CurrentFactionID");

                    b.HasOne("NauraaBot.Database.Models.Faction", "MainFaction")
                        .WithMany()
                        .HasForeignKey("MainFactionID");

                    b.HasOne("NauraaBot.Database.Models.Rarity", "Rarity")
                        .WithMany()
                        .HasForeignKey("RarityID");

                    b.HasOne("NauraaBot.Database.Models.CardSet", "Set")
                        .WithMany()
                        .HasForeignKey("SetID");

                    b.HasOne("NauraaBot.Database.Models.CardType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeID");

                    b.OwnsOne("NauraaBot.Core.Utils.LocalizedString", "Effect", b1 =>
                        {
                            b1.Property<string>("CardID")
                                .HasColumnType("TEXT");

                            b1.Property<string>("de")
                                .HasColumnType("TEXT");

                            b1.Property<string>("en")
                                .HasColumnType("TEXT");

                            b1.Property<string>("es")
                                .HasColumnType("TEXT");

                            b1.Property<string>("fr")
                                .HasColumnType("TEXT");

                            b1.Property<string>("it")
                                .HasColumnType("TEXT");

                            b1.HasKey("CardID");

                            b1.ToTable("Card");

                            b1.WithOwner()
                                .HasForeignKey("CardID");
                        });

                    b.OwnsOne("NauraaBot.Core.Utils.LocalizedString", "ImagesURLs", b1 =>
                        {
                            b1.Property<string>("CardID")
                                .HasColumnType("TEXT");

                            b1.Property<string>("de")
                                .HasColumnType("TEXT");

                            b1.Property<string>("en")
                                .HasColumnType("TEXT");

                            b1.Property<string>("es")
                                .HasColumnType("TEXT");

                            b1.Property<string>("fr")
                                .HasColumnType("TEXT");

                            b1.Property<string>("it")
                                .HasColumnType("TEXT");

                            b1.HasKey("CardID");

                            b1.ToTable("Card");

                            b1.WithOwner()
                                .HasForeignKey("CardID");
                        });

                    b.OwnsOne("NauraaBot.Core.Utils.LocalizedString", "Names", b1 =>
                        {
                            b1.Property<string>("CardID")
                                .HasColumnType("TEXT");

                            b1.Property<string>("de")
                                .HasColumnType("TEXT");

                            b1.Property<string>("en")
                                .HasColumnType("TEXT");

                            b1.Property<string>("es")
                                .HasColumnType("TEXT");

                            b1.Property<string>("fr")
                                .HasColumnType("TEXT");

                            b1.Property<string>("it")
                                .HasColumnType("TEXT");

                            b1.HasKey("CardID");

                            b1.ToTable("Card");

                            b1.WithOwner()
                                .HasForeignKey("CardID");
                        });

                    b.OwnsOne("NauraaBot.Database.Models.Costs", "Costs", b1 =>
                        {
                            b1.Property<string>("CardID")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Hand")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Reserve")
                                .HasColumnType("INTEGER");

                            b1.HasKey("CardID");

                            b1.ToTable("Card");

                            b1.WithOwner()
                                .HasForeignKey("CardID");
                        });

                    b.OwnsOne("NauraaBot.Database.Models.Power", "Power", b1 =>
                        {
                            b1.Property<string>("CardID")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Forest")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Mountain")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Ocean")
                                .HasColumnType("INTEGER");

                            b1.HasKey("CardID");

                            b1.ToTable("Card");

                            b1.WithOwner()
                                .HasForeignKey("CardID");
                        });

                    b.Navigation("Costs");

                    b.Navigation("CurrentFaction");

                    b.Navigation("Effect");

                    b.Navigation("ImagesURLs");

                    b.Navigation("MainFaction");

                    b.Navigation("Names");

                    b.Navigation("Power");

                    b.Navigation("Rarity");

                    b.Navigation("Set");

                    b.Navigation("Type");
                });
#pragma warning restore 612, 618
        }
    }
}
