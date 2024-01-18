﻿// <auto-generated />
using System;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200805135105_AddComposeKey")]
    partial class AddComposeKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Crop", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Crop");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropDecisionCombination", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DssId")
                        .HasColumnType("uuid");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<Guid>("PestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DssId");

                    b.HasIndex("PestId");

                    b.HasIndex("CropId", "DssId", "PestId");

                    b.ToTable("CropDecisionCombination");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.DataSharingRequest", b =>
                {
                    b.Property<Guid>("RequesteeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RequesterId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("RequestStatusDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RequesteeId", "RequesterId");

                    b.HasIndex("RequestStatusDescription");

                    b.HasIndex("RequesterId");

                    b.ToTable("DataSharingRequest");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.DataSharingRequestStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Description")
                        .IsUnique();

                    b.ToTable("DataSharingRequestStatus");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Description = "Pending"
                        },
                        new
                        {
                            Id = 1,
                            Description = "Accepted"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Declined"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Revoked"
                        });
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Dss", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Dss");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Farm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<string>("Inf2")
                        .HasColumnType("text");

                    b.Property<Point>("Location")
                        .IsRequired()
                        .HasColumnType("geometry (point)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Location")
                        .IsUnique();

                    b.ToTable("Farm");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Field", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<string>("Inf2")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FarmId");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropDecisionCombination", b =>
                {
                    b.Property<Guid>("FielId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropDecisionCombinationId")
                        .HasColumnType("uuid");

                    b.HasKey("FielId", "CropDecisionCombinationId");

                    b.HasIndex("CropDecisionCombinationId");

                    b.ToTable("FieldCropDecisionCombination");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CropEppoCode")
                        .IsRequired()
                        .HasColumnType("character varying(6)")
                        .HasMaxLength(6);

                    b.Property<Guid>("FieldId")
                        .HasColumnType("uuid");

                    b.Property<Point>("Location")
                        .IsRequired()
                        .HasColumnType("geometry (point)");

                    b.Property<string>("PestEppoCode")
                        .IsRequired()
                        .HasColumnType("character varying(6)")
                        .HasMaxLength(6);

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.ToTable("FieldObservation");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Pest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Pest");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Postcode")
                        .HasColumnType("text");

                    b.Property<string>("Street")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserAddress");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserFarm", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Authorised")
                        .HasColumnType("boolean");

                    b.Property<string>("UserFarmTypeDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId", "FarmId");

                    b.HasIndex("FarmId");

                    b.HasIndex("UserFarmTypeDescription");

                    b.ToTable("UserFarm");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserFarmType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Description")
                        .IsUnique();

                    b.ToTable("UserFarmType");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Description = "Unknown"
                        },
                        new
                        {
                            Id = 1,
                            Description = "Owner"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Advisor"
                        });
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(80)")
                        .HasMaxLength(80);

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserAddressId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserAddressId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropDecisionCombination", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Crop", "Crop")
                        .WithMany("CropDecisionCombinations")
                        .HasForeignKey("CropId")
                        .HasConstraintName("FK_CropCombination_Crop")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Dss", "Dss")
                        .WithMany("CropDecisionCombinations")
                        .HasForeignKey("DssId")
                        .HasConstraintName("FK_CropCombination_Dss")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Pest", "Pest")
                        .WithMany("CropDecisionCombinations")
                        .HasForeignKey("PestId")
                        .HasConstraintName("FK_CropCombination_Pest")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.DataSharingRequest", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.DataSharingRequestStatus", "RequestStatus")
                        .WithMany("DataSharingRequests")
                        .HasForeignKey("RequestStatusDescription")
                        .HasConstraintName("FK_DataSharingRequest_RequestStatus_RequestDescription")
                        .HasPrincipalKey("Description")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", "Requestee")
                        .WithMany()
                        .HasForeignKey("RequesteeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", "Requester")
                        .WithMany()
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Field", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Farm", "Farm")
                        .WithMany("Fields")
                        .HasForeignKey("FarmId")
                        .HasConstraintName("FK_Field_Farm")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropDecisionCombination", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropDecisionCombination", "CropDecisionCombination")
                        .WithMany("FieldCropDecisionCombinations")
                        .HasForeignKey("CropDecisionCombinationId")
                        .HasConstraintName("FK_FieldCropDecision_CropDecision")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Field", "Field")
                        .WithMany("FieldCropDecisionCombinations")
                        .HasForeignKey("FielId")
                        .HasConstraintName("FK_FieldCropDecision_Field")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Field", "Field")
                        .WithMany("FieldObservations")
                        .HasForeignKey("FieldId")
                        .HasConstraintName("FK_Observation_Field")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserFarm", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Farm", "Farm")
                        .WithMany("UserFarms")
                        .HasForeignKey("FarmId")
                        .HasConstraintName("FK_UserFarm_Farm")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserFarmType", "UserFarmType")
                        .WithMany("UserFarms")
                        .HasForeignKey("UserFarmTypeDescription")
                        .HasPrincipalKey("Description")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", "UserProfile")
                        .WithMany("UserFarms")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserFarm_User")
                        .HasPrincipalKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserAddress", "UserAddress")
                        .WithMany()
                        .HasForeignKey("UserAddressId")
                        .HasConstraintName("FK_User_UserAddress")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
