﻿// <auto-generated />
using System;
using EFCoreDemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCoreDemo.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250114092709_addcolld")]
    partial class addcolld
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("EFCoreDemo.Data.Activity", b =>
                {
                    b.Property<byte>("ActivityId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ActivityDescription")
                        .IsRequired()
                        .HasMaxLength(190)
                        .HasColumnType("TEXT");

                    b.HasKey("ActivityId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("EFCoreDemo.Data.collClass", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<byte?>("ActivityId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("EnterpriseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EnterpriseId", "ActivityId");

                    b.ToTable("collClasses");
                });

            modelBuilder.Entity("EFCoreDemo.Data.Enterprise", b =>
                {
                    b.Property<long>("EnterpriseId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EnterpriseName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("TaxPayerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("EnterpriseId");

                    b.HasIndex("TaxPayerId");

                    b.ToTable("Enterprises");
                });

            modelBuilder.Entity("EFCoreDemo.Data.EnterpriseBusinessActivity", b =>
                {
                    b.Property<long>("EnterpriseId")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("ActivityId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MainActivityFlag")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("TEXT");

                    b.HasKey("EnterpriseId", "ActivityId");

                    b.HasIndex("ActivityId");

                    b.ToTable("EnterpriseBusinessActivities");
                });

            modelBuilder.Entity("EFCoreDemo.Data.TaxPayer", b =>
                {
                    b.Property<long>("TaxPayerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TaxPayerName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("TaxPayerId");

                    b.ToTable("TaxPayers");
                });

            modelBuilder.Entity("EFCoreDemo.Data.collClass", b =>
                {
                    b.HasOne("EFCoreDemo.Data.Enterprise", "Enterprise")
                        .WithMany()
                        .HasForeignKey("EnterpriseId");

                    b.HasOne("EFCoreDemo.Data.EnterpriseBusinessActivity", "EnterpriseBusinessActivity")
                        .WithMany("collClasses")
                        .HasForeignKey("EnterpriseId", "ActivityId");

                    b.Navigation("Enterprise");

                    b.Navigation("EnterpriseBusinessActivity");
                });

            modelBuilder.Entity("EFCoreDemo.Data.Enterprise", b =>
                {
                    b.HasOne("EFCoreDemo.Data.TaxPayer", "TaxPayer")
                        .WithMany("Enterprises")
                        .HasForeignKey("TaxPayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaxPayer");
                });

            modelBuilder.Entity("EFCoreDemo.Data.EnterpriseBusinessActivity", b =>
                {
                    b.HasOne("EFCoreDemo.Data.Activity", "Activity")
                        .WithMany("EnterpriseBusinessActivities")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreDemo.Data.Enterprise", "Enterprise")
                        .WithMany("EnterpriseBusinessActivities")
                        .HasForeignKey("EnterpriseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("Enterprise");
                });

            modelBuilder.Entity("EFCoreDemo.Data.Activity", b =>
                {
                    b.Navigation("EnterpriseBusinessActivities");
                });

            modelBuilder.Entity("EFCoreDemo.Data.Enterprise", b =>
                {
                    b.Navigation("EnterpriseBusinessActivities");
                });

            modelBuilder.Entity("EFCoreDemo.Data.EnterpriseBusinessActivity", b =>
                {
                    b.Navigation("collClasses");
                });

            modelBuilder.Entity("EFCoreDemo.Data.TaxPayer", b =>
                {
                    b.Navigation("Enterprises");
                });
#pragma warning restore 612, 618
        }
    }
}
