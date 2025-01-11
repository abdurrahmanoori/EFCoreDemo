﻿// <auto-generated />
using EFCoreDemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCoreDemo.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("EFCoreDemo.Data.ENT_ACTIVITY", b =>
                {
                    b.Property<byte>("ENT_ACTIVITY_NO")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ENT_ACTIVITY_DESC")
                        .IsRequired()
                        .HasMaxLength(190)
                        .HasColumnType("TEXT");

                    b.HasKey("ENT_ACTIVITY_NO");

                    b.ToTable("ENT_ACTIVITY");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENT_BUS_ACT", b =>
                {
                    b.Property<long>("ENTERPRISE_NO")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("ENT_ACTIVITY_NO")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MAIN_ACTIVITY_FL")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("TEXT");

                    b.HasKey("ENTERPRISE_NO", "ENT_ACTIVITY_NO");

                    b.HasIndex("ENT_ACTIVITY_NO");

                    b.ToTable("ENT_BUS_ACT");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENTERPRISE", b =>
                {
                    b.Property<long>("ENTERPRISE_NO")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ENTERPRISE_NAME")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("TAX_PAYER_NO")
                        .HasColumnType("INTEGER");

                    b.HasKey("ENTERPRISE_NO");

                    b.HasIndex("TAX_PAYER_NO");

                    b.ToTable("ENTERPRISE");
                });

            modelBuilder.Entity("EFCoreDemo.Data.TAX_PAYER", b =>
                {
                    b.Property<long>("TAX_PAYER_NO")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TAX_PAYER_NAME")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("TAX_PAYER_NO");

                    b.ToTable("TAX_PAYER");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENT_BUS_ACT", b =>
                {
                    b.HasOne("EFCoreDemo.Data.ENTERPRISE", "ENTERPRISE")
                        .WithMany("ENT_BUS_ACT")
                        .HasForeignKey("ENTERPRISE_NO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreDemo.Data.ENT_ACTIVITY", "ENT_ACTIVITY")
                        .WithMany("ENT_BUS_ACT")
                        .HasForeignKey("ENT_ACTIVITY_NO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ENTERPRISE");

                    b.Navigation("ENT_ACTIVITY");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENTERPRISE", b =>
                {
                    b.HasOne("EFCoreDemo.Data.TAX_PAYER", "TAX_PAYER")
                        .WithMany("ENTERPRISE")
                        .HasForeignKey("TAX_PAYER_NO")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TAX_PAYER");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENT_ACTIVITY", b =>
                {
                    b.Navigation("ENT_BUS_ACT");
                });

            modelBuilder.Entity("EFCoreDemo.Data.ENTERPRISE", b =>
                {
                    b.Navigation("ENT_BUS_ACT");
                });

            modelBuilder.Entity("EFCoreDemo.Data.TAX_PAYER", b =>
                {
                    b.Navigation("ENTERPRISE");
                });
#pragma warning restore 612, 618
        }
    }
}
