--
-- PostgreSQL database dump
--

-- Dumped from database version 12.5 (Debian 12.5-1.pgdg100+1)
-- Dumped by pg_dump version 12.5 (Debian 12.5-1.pgdg100+1)

-- Started on 2021-03-16 09:33:28 UTC

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 2 (class 3079 OID 282846)
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- TOC entry 4145 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 284115)
-- Name: CropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPest" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "PestEppoCode" character varying(6) NOT NULL
);


--
-- TOC entry 225 (class 1259 OID 284229)
-- Name: CropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPestDss" (
    "Id" uuid NOT NULL,
    "CropPestId" uuid NOT NULL,
    "DssId" text NOT NULL,
    "DssName" text NOT NULL,
    "DssModelId" text DEFAULT ''::text NOT NULL,
    "DssModelName" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 236 (class 1259 OID 284458)
-- Name: CropPestDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPestDssResult" (
    "Id" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL,
    "Inf1" text,
    "Inf2" text
);


--
-- TOC entry 218 (class 1259 OID 284024)
-- Name: DataSharingRequest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequest" (
    "Id" uuid NOT NULL,
    "RequesteeId" uuid NOT NULL,
    "RequesterId" uuid NOT NULL,
    "RequestStatusDescription" text NOT NULL
);


--
-- TOC entry 217 (class 1259 OID 284014)
-- Name: DataSharingRequestStatus; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequestStatus" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 212 (class 1259 OID 283866)
-- Name: Farm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Farm" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Inf1" text,
    "Inf2" text,
    "Location" public.geometry(Point) NOT NULL
);


--
-- TOC entry 229 (class 1259 OID 284312)
-- Name: FarmWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FarmWeatherStation" (
    "FarmId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 214 (class 1259 OID 283891)
-- Name: Field; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Field" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Inf1" text,
    "Inf2" text,
    "FarmId" uuid NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 284399)
-- Name: FieldCrop; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCrop" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "FieldId" uuid NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 284120)
-- Name: FieldCropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPest" (
    "CropPestId" uuid NOT NULL,
    "Id" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "FieldCropId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 226 (class 1259 OID 284254)
-- Name: FieldCropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPestDss" (
    "Id" uuid NOT NULL,
    "FieldCropPestId" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "DssParameters" jsonb
);


--
-- TOC entry 237 (class 1259 OID 284472)
-- Name: FieldDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldDssResult" (
    "Id" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL,
    "Result" jsonb,
    "FieldCropPestDssId" uuid NOT NULL
);


--
-- TOC entry 215 (class 1259 OID 283905)
-- Name: FieldObservation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldObservation" (
    "Id" uuid NOT NULL,
    "Location" public.geometry,
    "Time" timestamp without time zone DEFAULT now() NOT NULL,
    "FieldCropPestdId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "Severity" text
);


--
-- TOC entry 230 (class 1259 OID 284345)
-- Name: FieldSprayApplication; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldSprayApplication" (
    "Id" uuid NOT NULL,
    "Time" timestamp without time zone DEFAULT now() NOT NULL,
    "Rate" double precision NOT NULL,
    "Name" text DEFAULT ''::text NOT NULL,
    "FieldCropPestId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 234 (class 1259 OID 284420)
-- Name: FieldWeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherDataSource" (
    "FieldId" uuid NOT NULL,
    "WeatherDataSourceId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 235 (class 1259 OID 284438)
-- Name: FieldWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherStation" (
    "FieldId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 284167)
-- Name: ForecastAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 284192)
-- Name: ForecastResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastResult" (
    "Id" uuid NOT NULL,
    "ForecastAlertId" uuid NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "Inf1" text,
    "Inf2" text
);


--
-- TOC entry 222 (class 1259 OID 284177)
-- Name: ObservationAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL,
    "FieldObservationId" uuid NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 284205)
-- Name: ObservationResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationResult" (
    "Id" uuid NOT NULL,
    "ObservationAlertId" uuid NOT NULL,
    "Date" timestamp without time zone NOT NULL,
    "Inf1" text,
    "Inf2" text
);


--
-- TOC entry 206 (class 1259 OID 282832)
-- Name: UserAddress; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserAddress" (
    "Id" uuid NOT NULL,
    "Street" text,
    "City" text,
    "Postcode" text,
    "Country" text
);


--
-- TOC entry 213 (class 1259 OID 283874)
-- Name: UserFarm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarm" (
    "UserId" uuid NOT NULL,
    "FarmId" uuid NOT NULL,
    "Authorised" boolean DEFAULT false NOT NULL,
    "UserFarmTypeDescription" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 216 (class 1259 OID 283997)
-- Name: UserFarmType; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarmType" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 205 (class 1259 OID 282822)
-- Name: UserProfile; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserProfile" (
    "FirstName" character varying(80) NOT NULL,
    "LastName" text,
    "PhoneNumber" text,
    "MobileNumber" text,
    "UserId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "UserAddressId" uuid
);


--
-- TOC entry 232 (class 1259 OID 284378)
-- Name: UserWidget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserWidget" (
    "UserId" uuid NOT NULL,
    "WidgetId" integer NOT NULL,
    "WidgetDescription" text NOT NULL,
    "Allowed" boolean NOT NULL
);


--
-- TOC entry 227 (class 1259 OID 284278)
-- Name: WeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherDataSource" (
    "Name" text NOT NULL,
    "Url" text NOT NULL,
    "AuthenticationRequired" boolean DEFAULT false NOT NULL,
    "Credentials" jsonb,
    "FarmId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "Interval" integer DEFAULT 0 NOT NULL,
    "IsForecast" boolean DEFAULT false NOT NULL,
    "Parameters" text,
    "StationId" text,
    "TimeEnd" timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "TimeStart" timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL,
    "Id" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 228 (class 1259 OID 284286)
-- Name: WeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherStation" (
    "Id" text NOT NULL,
    "Name" text
);


--
-- TOC entry 231 (class 1259 OID 284368)
-- Name: Widget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Widget" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 204 (class 1259 OID 282817)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- TOC entry 3881 (class 2606 OID 284023)
-- Name: DataSharingRequestStatus AK_DataSharingRequestStatus_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "AK_DataSharingRequestStatus_Description" UNIQUE ("Description");


--
-- TOC entry 3876 (class 2606 OID 284006)
-- Name: UserFarmType AK_UserFarmType_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "AK_UserFarmType_Description" UNIQUE ("Description");


--
-- TOC entry 3928 (class 2606 OID 284377)
-- Name: Widget AK_Widget_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "AK_Widget_Description" UNIQUE ("Description");


--
-- TOC entry 3892 (class 2606 OID 284119)
-- Name: CropPest PK_CropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPest"
    ADD CONSTRAINT "PK_CropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3911 (class 2606 OID 284236)
-- Name: CropPestDss PK_CropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "PK_CropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 3946 (class 2606 OID 284465)
-- Name: CropPestDssResult PK_CropPestDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "PK_CropPestDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3889 (class 2606 OID 284107)
-- Name: DataSharingRequest PK_DataSharingRequest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "PK_DataSharingRequest" PRIMARY KEY ("Id");


--
-- TOC entry 3884 (class 2606 OID 284021)
-- Name: DataSharingRequestStatus PK_DataSharingRequestStatus; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "PK_DataSharingRequestStatus" PRIMARY KEY ("Id");


--
-- TOC entry 3864 (class 2606 OID 283873)
-- Name: Farm PK_Farm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "PK_Farm" PRIMARY KEY ("Id");


--
-- TOC entry 3923 (class 2606 OID 284319)
-- Name: FarmWeatherStation PK_FarmWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "PK_FarmWeatherStation" PRIMARY KEY ("FarmId", "WeatherStationId");


--
-- TOC entry 3871 (class 2606 OID 283898)
-- Name: Field PK_Field; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "PK_Field" PRIMARY KEY ("Id");


--
-- TOC entry 3937 (class 2606 OID 284403)
-- Name: FieldCrop PK_FieldCrop; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "PK_FieldCrop" PRIMARY KEY ("Id");


--
-- TOC entry 3896 (class 2606 OID 284227)
-- Name: FieldCropPest PK_FieldCropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "PK_FieldCropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3915 (class 2606 OID 284258)
-- Name: FieldCropPestDss PK_FieldCropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "PK_FieldCropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 3949 (class 2606 OID 284479)
-- Name: FieldDssResult PK_FieldDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "PK_FieldDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3874 (class 2606 OID 283912)
-- Name: FieldObservation PK_FieldObservation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "PK_FieldObservation" PRIMARY KEY ("Id");


--
-- TOC entry 3926 (class 2606 OID 284350)
-- Name: FieldSprayApplication PK_FieldSprayApplication; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "PK_FieldSprayApplication" PRIMARY KEY ("Id");


--
-- TOC entry 3940 (class 2606 OID 284511)
-- Name: FieldWeatherDataSource PK_FieldWeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "PK_FieldWeatherDataSource" PRIMARY KEY ("FieldId");


--
-- TOC entry 3943 (class 2606 OID 284445)
-- Name: FieldWeatherStation PK_FieldWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "PK_FieldWeatherStation" PRIMARY KEY ("FieldId", "WeatherStationId");


--
-- TOC entry 3898 (class 2606 OID 284171)
-- Name: ForecastAlert PK_ForecastAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastAlert"
    ADD CONSTRAINT "PK_ForecastAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3905 (class 2606 OID 284199)
-- Name: ForecastResult PK_ForecastResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "PK_ForecastResult" PRIMARY KEY ("Id");


--
-- TOC entry 3902 (class 2606 OID 284181)
-- Name: ObservationAlert PK_ObservationAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "PK_ObservationAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3908 (class 2606 OID 284212)
-- Name: ObservationResult PK_ObservationResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "PK_ObservationResult" PRIMARY KEY ("Id");


--
-- TOC entry 3859 (class 2606 OID 282839)
-- Name: UserAddress PK_UserAddress; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserAddress"
    ADD CONSTRAINT "PK_UserAddress" PRIMARY KEY ("Id");


--
-- TOC entry 3868 (class 2606 OID 284080)
-- Name: UserFarm PK_UserFarm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "PK_UserFarm" PRIMARY KEY ("UserId", "FarmId");


--
-- TOC entry 3879 (class 2606 OID 284004)
-- Name: UserFarmType PK_UserFarmType; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "PK_UserFarmType" PRIMARY KEY ("Id");


--
-- TOC entry 3857 (class 2606 OID 284078)
-- Name: UserProfile PK_UserProfile; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "PK_UserProfile" PRIMARY KEY ("UserId");


--
-- TOC entry 3934 (class 2606 OID 284385)
-- Name: UserWidget PK_UserWidget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "PK_UserWidget" PRIMARY KEY ("UserId", "WidgetId");


--
-- TOC entry 3918 (class 2606 OID 284509)
-- Name: WeatherDataSource PK_WeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherDataSource"
    ADD CONSTRAINT "PK_WeatherDataSource" PRIMARY KEY ("Id");


--
-- TOC entry 3920 (class 2606 OID 284293)
-- Name: WeatherStation PK_WeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherStation"
    ADD CONSTRAINT "PK_WeatherStation" PRIMARY KEY ("Id");


--
-- TOC entry 3931 (class 2606 OID 284375)
-- Name: Widget PK_Widget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "PK_Widget" PRIMARY KEY ("Id");


--
-- TOC entry 3854 (class 2606 OID 282821)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 3944 (class 1259 OID 284471)
-- Name: IX_CropPestDssResult_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CropPestDssResult_CropPestDssId" ON public."CropPestDssResult" USING btree ("CropPestDssId");


--
-- TOC entry 3909 (class 1259 OID 284419)
-- Name: IX_CropPestDss_CropPestId_DssId_DssModelId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPestDss_CropPestId_DssId_DssModelId" ON public."CropPestDss" USING btree ("CropPestId", "DssId", "DssModelId");


--
-- TOC entry 3890 (class 1259 OID 284136)
-- Name: IX_CropPest_CropEppoCode_PestEppoCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPest_CropEppoCode_PestEppoCode" ON public."CropPest" USING btree ("CropEppoCode", "PestEppoCode");


--
-- TOC entry 3882 (class 1259 OID 284050)
-- Name: IX_DataSharingRequestStatus_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequestStatus_Description" ON public."DataSharingRequestStatus" USING btree ("Description");


--
-- TOC entry 3885 (class 1259 OID 284047)
-- Name: IX_DataSharingRequest_RequestStatusDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequestStatusDescription" ON public."DataSharingRequest" USING btree ("RequestStatusDescription");


--
-- TOC entry 3886 (class 1259 OID 284108)
-- Name: IX_DataSharingRequest_RequesteeId_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequest_RequesteeId_RequesterId" ON public."DataSharingRequest" USING btree ("RequesteeId", "RequesterId");


--
-- TOC entry 3887 (class 1259 OID 284049)
-- Name: IX_DataSharingRequest_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequesterId" ON public."DataSharingRequest" USING btree ("RequesterId");


--
-- TOC entry 3921 (class 1259 OID 284331)
-- Name: IX_FarmWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FarmWeatherStation_WeatherStationId" ON public."FarmWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3862 (class 1259 OID 284341)
-- Name: IX_Farm_Location; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_Location" ON public."Farm" USING btree ("Location");


--
-- TOC entry 3912 (class 1259 OID 284269)
-- Name: IX_FieldCropPestDss_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPestDss_CropPestDssId" ON public."FieldCropPestDss" USING btree ("CropPestDssId");


--
-- TOC entry 3913 (class 1259 OID 284270)
-- Name: IX_FieldCropPestDss_FieldCropPestId_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId" ON public."FieldCropPestDss" USING btree ("FieldCropPestId", "CropPestDssId");


--
-- TOC entry 3893 (class 1259 OID 284137)
-- Name: IX_FieldCropPest_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPest_CropPestId" ON public."FieldCropPest" USING btree ("CropPestId");


--
-- TOC entry 3894 (class 1259 OID 284409)
-- Name: IX_FieldCropPest_FieldCropId_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPest_FieldCropId_CropPestId" ON public."FieldCropPest" USING btree ("FieldCropId", "CropPestId");


--
-- TOC entry 3935 (class 1259 OID 284416)
-- Name: IX_FieldCrop_FieldId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCrop_FieldId" ON public."FieldCrop" USING btree ("FieldId");


--
-- TOC entry 3947 (class 1259 OID 284485)
-- Name: IX_FieldDssResult_FieldCropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldDssResult_FieldCropPestDssId" ON public."FieldDssResult" USING btree ("FieldCropPestDssId");


--
-- TOC entry 3872 (class 1259 OID 284272)
-- Name: IX_FieldObservation_FieldCropPestdId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldObservation_FieldCropPestdId" ON public."FieldObservation" USING btree ("FieldCropPestdId");


--
-- TOC entry 3924 (class 1259 OID 284362)
-- Name: IX_FieldSprayApplication_FieldCropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldSprayApplication_FieldCropPestId" ON public."FieldSprayApplication" USING btree ("FieldCropPestId");


--
-- TOC entry 3938 (class 1259 OID 284513)
-- Name: IX_FieldWeatherDataSource_WeatherDataSourceId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherDataSource_WeatherDataSourceId" ON public."FieldWeatherDataSource" USING btree ("WeatherDataSourceId");


--
-- TOC entry 3941 (class 1259 OID 284457)
-- Name: IX_FieldWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherStation_WeatherStationId" ON public."FieldWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3869 (class 1259 OID 283904)
-- Name: IX_Field_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Field_FarmId" ON public."Field" USING btree ("FarmId");


--
-- TOC entry 3903 (class 1259 OID 284220)
-- Name: IX_ForecastResult_ForecastAlertId_Date; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ForecastResult_ForecastAlertId_Date" ON public."ForecastResult" USING btree ("ForecastAlertId", "Date");


--
-- TOC entry 3899 (class 1259 OID 284222)
-- Name: IX_ObservationAlert_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationAlert_FieldObservationId" ON public."ObservationAlert" USING btree ("FieldObservationId");


--
-- TOC entry 3900 (class 1259 OID 284242)
-- Name: IX_ObservationAlert_WeatherStationId_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ObservationAlert_WeatherStationId_FieldObservationId" ON public."ObservationAlert" USING btree ("WeatherStationId", "FieldObservationId");


--
-- TOC entry 3906 (class 1259 OID 284224)
-- Name: IX_ObservationResult_ObservationAlertId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationResult_ObservationAlertId" ON public."ObservationResult" USING btree ("ObservationAlertId");


--
-- TOC entry 3877 (class 1259 OID 284008)
-- Name: IX_UserFarmType_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_UserFarmType_Description" ON public."UserFarmType" USING btree ("Description");


--
-- TOC entry 3865 (class 1259 OID 283890)
-- Name: IX_UserFarm_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_FarmId" ON public."UserFarm" USING btree ("FarmId");


--
-- TOC entry 3866 (class 1259 OID 284007)
-- Name: IX_UserFarm_UserFarmTypeDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_UserFarmTypeDescription" ON public."UserFarm" USING btree ("UserFarmTypeDescription");


--
-- TOC entry 3855 (class 1259 OID 282840)
-- Name: IX_UserProfile_UserAddressId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserProfile_UserAddressId" ON public."UserProfile" USING btree ("UserAddressId");


--
-- TOC entry 3932 (class 1259 OID 284396)
-- Name: IX_UserWidget_WidgetDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserWidget_WidgetDescription" ON public."UserWidget" USING btree ("WidgetDescription");


--
-- TOC entry 3916 (class 1259 OID 284492)
-- Name: IX_WeatherDataSource_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_WeatherDataSource_FarmId" ON public."WeatherDataSource" USING btree ("FarmId");


--
-- TOC entry 3929 (class 1259 OID 284397)
-- Name: IX_Widget_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Widget_Description" ON public."Widget" USING btree ("Description");


--
-- TOC entry 3978 (class 2606 OID 284466)
-- Name: CropPestDssResult FK_CropPestDss_CropPestDssResult; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "FK_CropPestDss_CropPestDssResult" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3959 (class 2606 OID 284125)
-- Name: FieldCropPest FK_CropPest_Crop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_CropPest_Crop" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3964 (class 2606 OID 284237)
-- Name: CropPestDss FK_CropPest_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "FK_CropPest_CropPestDss" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id");


--
-- TOC entry 3956 (class 2606 OID 284032)
-- Name: DataSharingRequest FK_DataSharingRequest_RequestStatus_RequestDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_RequestStatus_RequestDescription" FOREIGN KEY ("RequestStatusDescription") REFERENCES public."DataSharingRequestStatus"("Description");


--
-- TOC entry 3957 (class 2606 OID 284081)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesteeId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesteeId" FOREIGN KEY ("RequesteeId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3958 (class 2606 OID 284086)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesterId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesterId" FOREIGN KEY ("RequesterId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3967 (class 2606 OID 284493)
-- Name: WeatherDataSource FK_FarmWeatherDataSource_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherDataSource"
    ADD CONSTRAINT "FK_FarmWeatherDataSource_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3968 (class 2606 OID 284320)
-- Name: FarmWeatherStation FK_FarmWeatherStation_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3969 (class 2606 OID 284325)
-- Name: FarmWeatherStation FK_FarmWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 3965 (class 2606 OID 284259)
-- Name: FieldCropPestDss FK_FieldCropPestDss_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_CropPestDss" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3966 (class 2606 OID 284264)
-- Name: FieldCropPestDss FK_FieldCropPestDss_FieldCropPest_FieldCropPestId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_FieldCropPest_FieldCropPestId" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3960 (class 2606 OID 284411)
-- Name: FieldCropPest FK_FieldCropPest_FieldCrop_FieldCropId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_FieldCropPest_FieldCrop_FieldCropId" FOREIGN KEY ("FieldCropId") REFERENCES public."FieldCrop"("Id") ON DELETE CASCADE;


--
-- TOC entry 3979 (class 2606 OID 284480)
-- Name: FieldDssResult FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId" FOREIGN KEY ("FieldCropPestDssId") REFERENCES public."FieldCropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3974 (class 2606 OID 284428)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3975 (class 2606 OID 284514)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_WeatherDataSource; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_WeatherDataSource" FOREIGN KEY ("WeatherDataSourceId") REFERENCES public."WeatherDataSource"("Id") ON DELETE CASCADE;


--
-- TOC entry 3976 (class 2606 OID 284446)
-- Name: FieldWeatherStation FK_FieldWeatherStation_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3977 (class 2606 OID 284451)
-- Name: FieldWeatherStation FK_FieldWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 3954 (class 2606 OID 283899)
-- Name: Field FK_Field_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "FK_Field_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3973 (class 2606 OID 284404)
-- Name: FieldCrop FK_Field_FieldCrop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "FK_Field_FieldCrop" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3962 (class 2606 OID 284244)
-- Name: ForecastResult FK_ForecastResult_ForecastAlert_ForecastAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "FK_ForecastResult_ForecastAlert_ForecastAlertId" FOREIGN KEY ("ForecastAlertId") REFERENCES public."ForecastAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 3961 (class 2606 OID 284187)
-- Name: ObservationAlert FK_ObservationAlert_FieldObservation_FieldObservationId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "FK_ObservationAlert_FieldObservation_FieldObservationId" FOREIGN KEY ("FieldObservationId") REFERENCES public."FieldObservation"("Id");


--
-- TOC entry 3963 (class 2606 OID 284249)
-- Name: ObservationResult FK_ObservationResult_ObservationAlert_ObservationAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "FK_ObservationResult_ObservationAlert_ObservationAlertId" FOREIGN KEY ("ObservationAlertId") REFERENCES public."ObservationAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 3955 (class 2606 OID 284273)
-- Name: FieldObservation FK_Observation_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "FK_Observation_FieldCropPest" FOREIGN KEY ("FieldCropPestdId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3970 (class 2606 OID 284363)
-- Name: FieldSprayApplication FK_Spray_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "FK_Spray_FieldCropPest" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3952 (class 2606 OID 284096)
-- Name: UserFarm FK_UserFarm_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3953 (class 2606 OID 284101)
-- Name: UserFarm FK_UserFarm_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3951 (class 2606 OID 284009)
-- Name: UserFarm FK_UserFarm_UserFarmType_UserFarmTypeDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_UserFarmType_UserFarmTypeDescription" FOREIGN KEY ("UserFarmTypeDescription") REFERENCES public."UserFarmType"("Description");


--
-- TOC entry 3971 (class 2606 OID 284386)
-- Name: UserWidget FK_UserWidget_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3972 (class 2606 OID 284391)
-- Name: UserWidget FK_UserWidget_Widget; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_Widget" FOREIGN KEY ("WidgetDescription") REFERENCES public."Widget"("Description") ON DELETE CASCADE;


--
-- TOC entry 3950 (class 2606 OID 283859)
-- Name: UserProfile FK_User_UserAddress; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "FK_User_UserAddress" FOREIGN KEY ("UserAddressId") REFERENCES public."UserAddress"("Id") ON DELETE CASCADE;


-- Completed on 2021-03-16 09:33:28 UTC

--
-- PostgreSQL database dump complete
--

