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
    [Migration("20210413150212_TableWeatherForecastChanges")]
    partial class TableWeatherForecastChanges
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDss", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestId")
                        .HasColumnType("uuid");

                    b.Property<string>("DssExecutionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DssId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DssModelId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DssModelName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DssName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DssVersion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CropPestId", "DssId", "DssModelId", "DssVersion", "DssExecutionType")
                        .IsUnique();

                    b.ToTable("CropPestDss");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CropPestDssId")
                        .HasColumnType("uuid");

                    b.Property<string>("Inf1")
                        .HasColumnType("text");

                    b.Property<string>("Inf2")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CropPestDssId");

                    b.ToTable("CropPestDssResult");
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

                    b.HasIndex("Location");

                    b.ToTable("Farm");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FarmWeatherForecast", b =>
                {
                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WeatherForecastId")
                        .HasColumnType("uuid");

                    b.HasKey("FarmId", "WeatherForecastId");

                    b.HasIndex("WeatherForecastId");

                    b.ToTable("FarmWeatherForecast");
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCrop", b =>
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

                    b.HasKey("Id");

                    b.HasIndex("FieldId")
                        .IsUnique();

                    b.ToTable("FieldCrop");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FieldCropId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CropPestId");

                    b.HasIndex("FieldCropId", "CropPestId")
                        .IsUnique();

                    b.ToTable("FieldCropPest");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPestDss", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CropPestDssId")
                        .HasColumnType("uuid");

                    b.Property<string>("DssParameters")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("FieldCropPestId")
                        .HasColumnType("uuid");

                    b.Property<bool>("ObservationRequired")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CropPestDssId");

                    b.HasIndex("FieldCropPestId", "CropPestDssId")
                        .IsUnique();

                    b.ToTable("FieldCropPestDss");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldDssObservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DssObservation")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("FieldCropPestDssId")
                        .HasColumnType("uuid");

                    b.Property<Point>("Location")
                        .IsRequired()
                        .HasColumnType("geometry");

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.HasKey("Id");

                    b.HasIndex("FieldCropPestDssId");

                    b.ToTable("FieldDssObservation");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldDssResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("FieldCropPestDssId")
                        .HasColumnType("uuid");

                    b.Property<string>("Result")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("FieldCropPestDssId");

                    b.ToTable("FieldDssResult");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DssObservation")
                        .HasColumnType("text");

                    b.Property<Guid>("FieldCropPestId")
                        .HasColumnType("uuid");

                    b.Property<Point>("Location")
                        .HasColumnType("geometry");

                    b.Property<string>("Severity")
                        .HasColumnType("text");

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.HasKey("Id");

                    b.HasIndex("FieldCropPestId");

                    b.ToTable("FieldObservation");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldSprayApplication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("FieldCropPestId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Rate")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasDefaultValueSql("NOW()");

                    b.HasKey("Id");

                    b.HasIndex("FieldCropPestId");

                    b.ToTable("FieldSprayApplication");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastAlert", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("WeatherStationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

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

                    b.Property<Guid>("FieldObservationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WeatherStationId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("FieldObservationId");

                    b.HasIndex("WeatherStationId", "FieldObservationId")
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserWidget", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("WidgetId")
                        .HasColumnType("integer");

                    b.Property<bool>("Allowed")
                        .HasColumnType("boolean");

                    b.Property<string>("WidgetDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId", "WidgetId");

                    b.HasIndex("WidgetDescription");

                    b.ToTable("UserWidget");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.WeatherForecast", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("WeatherId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WeatherId")
                        .IsUnique();

                    b.ToTable("WeatherForecast");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.WeatherStation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AuthenticationRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("Credentials")
                        .HasColumnType("jsonb");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.Property<int>("Interval")
                        .HasColumnType("integer");

                    b.Property<bool>("IsForecast")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StationId")
                        .HasColumnType("text");

                    b.Property<DateTime>("TimeEnd")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("TimeStart")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FarmId");

                    b.ToTable("WeatherStation");
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.Widget", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Description")
                        .IsUnique();

                    b.ToTable("Widget");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Description = "Maps"
                        },
                        new
                        {
                            Id = 1,
                            Description = "Actions"
                        },
                        new
                        {
                            Id = 2,
                            Description = "RiskForecast"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Weather"
                        });
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDss", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPest", "CropPest")
                        .WithMany("CropPestDsses")
                        .HasForeignKey("CropPestId")
                        .HasConstraintName("FK_CropPest_CropPestDss")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.CropPestDssResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPestDss", "CropPestDss")
                        .WithMany("CropPestDssResults")
                        .HasForeignKey("CropPestDssId")
                        .HasConstraintName("FK_CropPestDss_CropPestDssResult")
                        .OnDelete(DeleteBehavior.Cascade)
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FarmWeatherForecast", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Farm", "Farm")
                        .WithMany("FarmWeatherForecast")
                        .HasForeignKey("FarmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.WeatherForecast", "WeatherForecast")
                        .WithMany("FarmWeatherForecast")
                        .HasForeignKey("WeatherForecastId")
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCrop", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Field", "Field")
                        .WithOne("FieldCrop")
                        .HasForeignKey("H2020.IPMDecisions.UPR.Core.Entities.FieldCrop", "FieldId")
                        .HasConstraintName("FK_Field_FieldCrop")
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

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCrop", "FieldCrop")
                        .WithMany("FieldCropPests")
                        .HasForeignKey("FieldCropId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPestDss", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.CropPestDss", "CropPestDss")
                        .WithMany("FieldCropPestDsses")
                        .HasForeignKey("CropPestDssId")
                        .HasConstraintName("FK_FieldCropPestDss_CropPestDss")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", "FieldCropPest")
                        .WithMany("FieldCropPestDsses")
                        .HasForeignKey("FieldCropPestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldDssObservation", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPestDss", "FieldCropPestDss")
                        .WithMany("FieldDssObservations")
                        .HasForeignKey("FieldCropPestDssId")
                        .HasConstraintName("FK_Observation_FieldCropPestDss")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldDssResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPestDss", "FieldCropPestDss")
                        .WithMany("FieldDssResults")
                        .HasForeignKey("FieldCropPestDssId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", "FieldCropPest")
                        .WithMany("FieldObservations")
                        .HasForeignKey("FieldCropPestId")
                        .HasConstraintName("FK_Observation_FieldCropPest")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.FieldSprayApplication", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldCropPest", "FieldCropPest")
                        .WithMany("FieldSprayApplications")
                        .HasForeignKey("FieldCropPestId")
                        .HasConstraintName("FK_Spray_FieldCropPest")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ForecastResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.ForecastAlert", "ForecastAlert")
                        .WithMany()
                        .HasForeignKey("ForecastAlertId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationAlert", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.FieldObservation", "FieldObservation")
                        .WithMany("ObservationAlerts")
                        .HasForeignKey("FieldObservationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.ObservationResult", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.ObservationAlert", "ObservationAlert")
                        .WithMany()
                        .HasForeignKey("ObservationAlertId")
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

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.UserWidget", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.UserProfile", "UserProfile")
                        .WithMany("UserWidgets")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserWidget_User")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Widget", "Widget")
                        .WithMany("UserWidgets")
                        .HasForeignKey("WidgetDescription")
                        .HasConstraintName("FK_UserWidget_Widget")
                        .HasPrincipalKey("Description")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("H2020.IPMDecisions.UPR.Core.Entities.WeatherStation", b =>
                {
                    b.HasOne("H2020.IPMDecisions.UPR.Core.Entities.Farm", "Farm")
                        .WithMany("FarmWeatherStations")
                        .HasForeignKey("FarmId")
                        .HasConstraintName("FK_FarmWeatherStation_Farm")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
