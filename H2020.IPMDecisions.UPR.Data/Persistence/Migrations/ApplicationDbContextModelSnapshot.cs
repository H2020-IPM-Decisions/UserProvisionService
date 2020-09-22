﻿// <auto-generated />
using System;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CropEppoCode")
                        .IsRequired()
                        .HasColumnType("character varying(6)")
                        .HasMaxLength(6);

                    b.Property<string>("PestEppoCode")
                        .IsRequired()
                        .HasColumnType("character varying(6)")
                        .HasMaxLength(6);

                    b.HasKey("Id");

                    b.HasIndex("CropEppoCode", "PestEppoCode")
                        .IsUnique();

                    b.ToTable("CropPest");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssCombination", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestId")
                        .HasColumnType("uuid");

                    b.Property<string>("DssName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CropPestId", "DssName")
                        .IsUnique();

                    b.ToTable("CropPestDssCombination");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.DataSharingRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("RequestStatusDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("RequesteeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RequesterId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RequestStatusDescription");

                    b.HasIndex("RequesterId");

                    b.HasIndex("RequesteeId", "RequesterId")
                        .IsUnique();

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
                        });
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", b =>
                {
                    b.Property<Guid>("FieldId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestId")
                        .HasColumnType("uuid");

                    b.HasKey("FieldId", "CropPestId");

                    b.HasIndex("CropPestId");

                    b.ToTable("FieldCropPest");
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastAlert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestDssCombinationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WeatherStationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CropPestDssCombinationId");

                    b.HasIndex("WeatherStationId", "CropPestDssCombinationId")
                        .IsUnique();

                    b.ToTable("ForecastAlert");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("ForecastAlertId")
                        .HasColumnType("uuid");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<string>("Inf2")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ForecastAlertId", "Date")
                        .IsUnique();

                    b.ToTable("ForecastResult");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationAlert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestDssCombinationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FieldObservationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WeatherStationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CropPestDssCombinationId");

                    b.HasIndex("FieldObservationId");

                    b.HasIndex("WeatherStationId", "CropPestDssCombinationId", "FieldObservationId")
                        .IsUnique();

                    b.ToTable("ObservationAlert");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<string>("Inf2")
                        .HasColumnType("text");

                    b.Property<Guid>("ObservationAlertId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ObservationAlertId");

                    b.ToTable("ObservationResult");
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
                    b.Property<Guid>("UserId")
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

                    b.HasKey("UserId");

                    b.HasIndex("UserAddressId");

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssCombination", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPest", "CropPest")
                        .WithMany("CropPestDssCombinations")
                        .HasForeignKey("CropPestId")
                        .HasConstraintName("FK_CropPestDss_Combination")
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
                        .WithMany("DataSharingRequests")
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPest", "CropPest")
                        .WithMany("FieldCropPests")
                        .HasForeignKey("CropPestId")
                        .HasConstraintName("FK_CropPest_Crop")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Field", "Field")
                        .WithMany("FieldCropPests")
                        .HasForeignKey("FieldId")
                        .HasConstraintName("FK_CropPest_Field")
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastAlert", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssCombination", "CropPestDssCombination")
                        .WithMany("ForecastAlerts")
                        .HasForeignKey("CropPestDssCombinationId")
                        .HasConstraintName("FK_ForecastAlert_CropPestDssCombination_Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.ForecastAlert", "ForecastAlert")
                        .WithMany("ForecastResults")
                        .HasForeignKey("ForecastAlertId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationAlert", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssCombination", "CropPestDssCombination")
                        .WithMany("ObservationAlerts")
                        .HasForeignKey("CropPestDssCombinationId")
                        .HasConstraintName("FK_ObservationAlert_CropPestDssCombination_Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", "FieldObservation")
                        .WithMany("ObservationAlerts")
                        .HasForeignKey("FieldObservationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.ObservationAlert", "ObservationAlert")
                        .WithMany("ObservationResults")
                        .HasForeignKey("ObservationAlertId")
                        .OnDelete(DeleteBehavior.NoAction)
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
