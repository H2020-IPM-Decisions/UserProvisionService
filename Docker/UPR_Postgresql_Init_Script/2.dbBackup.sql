--
-- PostgreSQL database dump
--

-- Dumped from database version 12.5 (Debian 12.5-1.pgdg100+1)
-- Dumped by pg_dump version 12.5 (Debian 12.5-1.pgdg100+1)

-- Started on 2021-03-04 15:36:38 UTC

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
-- TOC entry 2 (class 3079 OID 261729)
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- TOC entry 4087 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 262998)
-- Name: CropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPest" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "PestEppoCode" character varying(6) NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 263112)
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
-- TOC entry 236 (class 1259 OID 263341)
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
-- TOC entry 217 (class 1259 OID 262907)
-- Name: DataSharingRequest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequest" (
    "Id" uuid NOT NULL,
    "RequesteeId" uuid NOT NULL,
    "RequesterId" uuid NOT NULL,
    "RequestStatusDescription" text NOT NULL
);


--
-- TOC entry 216 (class 1259 OID 262897)
-- Name: DataSharingRequestStatus; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequestStatus" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 211 (class 1259 OID 262749)
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
-- TOC entry 228 (class 1259 OID 263177)
-- Name: FarmWeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FarmWeatherDataSource" (
    "FarmId" uuid NOT NULL,
    "WeatherDataSourceId" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 229 (class 1259 OID 263195)
-- Name: FarmWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FarmWeatherStation" (
    "FarmId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 213 (class 1259 OID 262774)
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
-- TOC entry 233 (class 1259 OID 263282)
-- Name: FieldCrop; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCrop" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "FieldId" uuid NOT NULL
);


--
-- TOC entry 219 (class 1259 OID 263003)
-- Name: FieldCropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPest" (
    "CropPestId" uuid NOT NULL,
    "Id" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "FieldCropId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 225 (class 1259 OID 263137)
-- Name: FieldCropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPestDss" (
    "Id" uuid NOT NULL,
    "FieldCropPestId" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "DssParameters" jsonb
);


--
-- TOC entry 237 (class 1259 OID 263355)
-- Name: FieldDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldDssResult" (
    "Id" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL,
    "Result" jsonb,
    "FieldCropPestDssId" uuid NOT NULL
);


--
-- TOC entry 214 (class 1259 OID 262788)
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
-- TOC entry 230 (class 1259 OID 263228)
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
-- TOC entry 234 (class 1259 OID 263303)
-- Name: FieldWeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherDataSource" (
    "FieldId" uuid NOT NULL,
    "WeatherDataSourceId" text NOT NULL
);


--
-- TOC entry 235 (class 1259 OID 263321)
-- Name: FieldWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherStation" (
    "FieldId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 263050)
-- Name: ForecastAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 263075)
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
-- TOC entry 221 (class 1259 OID 263060)
-- Name: ObservationAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL,
    "FieldObservationId" uuid NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 263088)
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
-- TOC entry 205 (class 1259 OID 261715)
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
-- TOC entry 212 (class 1259 OID 262757)
-- Name: UserFarm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarm" (
    "UserId" uuid NOT NULL,
    "FarmId" uuid NOT NULL,
    "Authorised" boolean DEFAULT false NOT NULL,
    "UserFarmTypeDescription" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 215 (class 1259 OID 262880)
-- Name: UserFarmType; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarmType" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 204 (class 1259 OID 261705)
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
-- TOC entry 232 (class 1259 OID 263261)
-- Name: UserWidget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserWidget" (
    "UserId" uuid NOT NULL,
    "WidgetId" integer NOT NULL,
    "WidgetDescription" text NOT NULL,
    "Allowed" boolean NOT NULL
);


--
-- TOC entry 226 (class 1259 OID 263161)
-- Name: WeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherDataSource" (
    "Id" text NOT NULL,
    "Name" text,
    "FileData" bytea,
    "FileExtension" text,
    "FileName" text,
    "FileUploadedOn" timestamp without time zone,
    "Url" text
);


--
-- TOC entry 227 (class 1259 OID 263169)
-- Name: WeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherStation" (
    "Id" text NOT NULL,
    "Name" text
);


--
-- TOC entry 231 (class 1259 OID 263251)
-- Name: Widget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Widget" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 203 (class 1259 OID 261700)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- TOC entry 3819 (class 2606 OID 262906)
-- Name: DataSharingRequestStatus AK_DataSharingRequestStatus_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "AK_DataSharingRequestStatus_Description" UNIQUE ("Description");


--
-- TOC entry 3814 (class 2606 OID 262889)
-- Name: UserFarmType AK_UserFarmType_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "AK_UserFarmType_Description" UNIQUE ("Description");


--
-- TOC entry 3868 (class 2606 OID 263260)
-- Name: Widget AK_Widget_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "AK_Widget_Description" UNIQUE ("Description");


--
-- TOC entry 3830 (class 2606 OID 263002)
-- Name: CropPest PK_CropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPest"
    ADD CONSTRAINT "PK_CropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3849 (class 2606 OID 263119)
-- Name: CropPestDss PK_CropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "PK_CropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 3886 (class 2606 OID 263348)
-- Name: CropPestDssResult PK_CropPestDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "PK_CropPestDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3827 (class 2606 OID 262990)
-- Name: DataSharingRequest PK_DataSharingRequest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "PK_DataSharingRequest" PRIMARY KEY ("Id");


--
-- TOC entry 3822 (class 2606 OID 262904)
-- Name: DataSharingRequestStatus PK_DataSharingRequestStatus; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "PK_DataSharingRequestStatus" PRIMARY KEY ("Id");


--
-- TOC entry 3802 (class 2606 OID 262756)
-- Name: Farm PK_Farm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "PK_Farm" PRIMARY KEY ("Id");


--
-- TOC entry 3860 (class 2606 OID 263217)
-- Name: FarmWeatherDataSource PK_FarmWeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherDataSource"
    ADD CONSTRAINT "PK_FarmWeatherDataSource" PRIMARY KEY ("FarmId", "WeatherDataSourceId");


--
-- TOC entry 3863 (class 2606 OID 263202)
-- Name: FarmWeatherStation PK_FarmWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "PK_FarmWeatherStation" PRIMARY KEY ("FarmId", "WeatherStationId");


--
-- TOC entry 3809 (class 2606 OID 262781)
-- Name: Field PK_Field; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "PK_Field" PRIMARY KEY ("Id");


--
-- TOC entry 3877 (class 2606 OID 263286)
-- Name: FieldCrop PK_FieldCrop; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "PK_FieldCrop" PRIMARY KEY ("Id");


--
-- TOC entry 3834 (class 2606 OID 263110)
-- Name: FieldCropPest PK_FieldCropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "PK_FieldCropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3853 (class 2606 OID 263141)
-- Name: FieldCropPestDss PK_FieldCropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "PK_FieldCropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 3889 (class 2606 OID 263362)
-- Name: FieldDssResult PK_FieldDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "PK_FieldDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3812 (class 2606 OID 262795)
-- Name: FieldObservation PK_FieldObservation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "PK_FieldObservation" PRIMARY KEY ("Id");


--
-- TOC entry 3866 (class 2606 OID 263233)
-- Name: FieldSprayApplication PK_FieldSprayApplication; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "PK_FieldSprayApplication" PRIMARY KEY ("Id");


--
-- TOC entry 3880 (class 2606 OID 263310)
-- Name: FieldWeatherDataSource PK_FieldWeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "PK_FieldWeatherDataSource" PRIMARY KEY ("FieldId", "WeatherDataSourceId");


--
-- TOC entry 3883 (class 2606 OID 263328)
-- Name: FieldWeatherStation PK_FieldWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "PK_FieldWeatherStation" PRIMARY KEY ("FieldId", "WeatherStationId");


--
-- TOC entry 3836 (class 2606 OID 263054)
-- Name: ForecastAlert PK_ForecastAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastAlert"
    ADD CONSTRAINT "PK_ForecastAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3843 (class 2606 OID 263082)
-- Name: ForecastResult PK_ForecastResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "PK_ForecastResult" PRIMARY KEY ("Id");


--
-- TOC entry 3840 (class 2606 OID 263064)
-- Name: ObservationAlert PK_ObservationAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "PK_ObservationAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3846 (class 2606 OID 263095)
-- Name: ObservationResult PK_ObservationResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "PK_ObservationResult" PRIMARY KEY ("Id");


--
-- TOC entry 3797 (class 2606 OID 261722)
-- Name: UserAddress PK_UserAddress; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserAddress"
    ADD CONSTRAINT "PK_UserAddress" PRIMARY KEY ("Id");


--
-- TOC entry 3806 (class 2606 OID 262963)
-- Name: UserFarm PK_UserFarm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "PK_UserFarm" PRIMARY KEY ("UserId", "FarmId");


--
-- TOC entry 3817 (class 2606 OID 262887)
-- Name: UserFarmType PK_UserFarmType; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "PK_UserFarmType" PRIMARY KEY ("Id");


--
-- TOC entry 3795 (class 2606 OID 262961)
-- Name: UserProfile PK_UserProfile; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "PK_UserProfile" PRIMARY KEY ("UserId");


--
-- TOC entry 3874 (class 2606 OID 263268)
-- Name: UserWidget PK_UserWidget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "PK_UserWidget" PRIMARY KEY ("UserId", "WidgetId");


--
-- TOC entry 3855 (class 2606 OID 263168)
-- Name: WeatherDataSource PK_WeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherDataSource"
    ADD CONSTRAINT "PK_WeatherDataSource" PRIMARY KEY ("Id");


--
-- TOC entry 3857 (class 2606 OID 263176)
-- Name: WeatherStation PK_WeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherStation"
    ADD CONSTRAINT "PK_WeatherStation" PRIMARY KEY ("Id");


--
-- TOC entry 3871 (class 2606 OID 263258)
-- Name: Widget PK_Widget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "PK_Widget" PRIMARY KEY ("Id");


--
-- TOC entry 3792 (class 2606 OID 261704)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 3884 (class 1259 OID 263354)
-- Name: IX_CropPestDssResult_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CropPestDssResult_CropPestDssId" ON public."CropPestDssResult" USING btree ("CropPestDssId");


--
-- TOC entry 3847 (class 1259 OID 263302)
-- Name: IX_CropPestDss_CropPestId_DssId_DssModelId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPestDss_CropPestId_DssId_DssModelId" ON public."CropPestDss" USING btree ("CropPestId", "DssId", "DssModelId");


--
-- TOC entry 3828 (class 1259 OID 263019)
-- Name: IX_CropPest_CropEppoCode_PestEppoCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPest_CropEppoCode_PestEppoCode" ON public."CropPest" USING btree ("CropEppoCode", "PestEppoCode");


--
-- TOC entry 3820 (class 1259 OID 262933)
-- Name: IX_DataSharingRequestStatus_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequestStatus_Description" ON public."DataSharingRequestStatus" USING btree ("Description");


--
-- TOC entry 3823 (class 1259 OID 262930)
-- Name: IX_DataSharingRequest_RequestStatusDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequestStatusDescription" ON public."DataSharingRequest" USING btree ("RequestStatusDescription");


--
-- TOC entry 3824 (class 1259 OID 262991)
-- Name: IX_DataSharingRequest_RequesteeId_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequest_RequesteeId_RequesterId" ON public."DataSharingRequest" USING btree ("RequesteeId", "RequesterId");


--
-- TOC entry 3825 (class 1259 OID 262932)
-- Name: IX_DataSharingRequest_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequesterId" ON public."DataSharingRequest" USING btree ("RequesterId");


--
-- TOC entry 3858 (class 1259 OID 263218)
-- Name: IX_FarmWeatherDataSource_WeatherDataSourceId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FarmWeatherDataSource_WeatherDataSourceId" ON public."FarmWeatherDataSource" USING btree ("WeatherDataSourceId");


--
-- TOC entry 3861 (class 1259 OID 263214)
-- Name: IX_FarmWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FarmWeatherStation_WeatherStationId" ON public."FarmWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3800 (class 1259 OID 263224)
-- Name: IX_Farm_Location; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_Location" ON public."Farm" USING btree ("Location");


--
-- TOC entry 3850 (class 1259 OID 263152)
-- Name: IX_FieldCropPestDss_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPestDss_CropPestDssId" ON public."FieldCropPestDss" USING btree ("CropPestDssId");


--
-- TOC entry 3851 (class 1259 OID 263153)
-- Name: IX_FieldCropPestDss_FieldCropPestId_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId" ON public."FieldCropPestDss" USING btree ("FieldCropPestId", "CropPestDssId");


--
-- TOC entry 3831 (class 1259 OID 263020)
-- Name: IX_FieldCropPest_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPest_CropPestId" ON public."FieldCropPest" USING btree ("CropPestId");


--
-- TOC entry 3832 (class 1259 OID 263292)
-- Name: IX_FieldCropPest_FieldCropId_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPest_FieldCropId_CropPestId" ON public."FieldCropPest" USING btree ("FieldCropId", "CropPestId");


--
-- TOC entry 3875 (class 1259 OID 263299)
-- Name: IX_FieldCrop_FieldId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCrop_FieldId" ON public."FieldCrop" USING btree ("FieldId");


--
-- TOC entry 3887 (class 1259 OID 263368)
-- Name: IX_FieldDssResult_FieldCropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldDssResult_FieldCropPestDssId" ON public."FieldDssResult" USING btree ("FieldCropPestDssId");


--
-- TOC entry 3810 (class 1259 OID 263155)
-- Name: IX_FieldObservation_FieldCropPestdId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldObservation_FieldCropPestdId" ON public."FieldObservation" USING btree ("FieldCropPestdId");


--
-- TOC entry 3864 (class 1259 OID 263245)
-- Name: IX_FieldSprayApplication_FieldCropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldSprayApplication_FieldCropPestId" ON public."FieldSprayApplication" USING btree ("FieldCropPestId");


--
-- TOC entry 3878 (class 1259 OID 263339)
-- Name: IX_FieldWeatherDataSource_WeatherDataSourceId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherDataSource_WeatherDataSourceId" ON public."FieldWeatherDataSource" USING btree ("WeatherDataSourceId");


--
-- TOC entry 3881 (class 1259 OID 263340)
-- Name: IX_FieldWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherStation_WeatherStationId" ON public."FieldWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3807 (class 1259 OID 262787)
-- Name: IX_Field_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Field_FarmId" ON public."Field" USING btree ("FarmId");


--
-- TOC entry 3841 (class 1259 OID 263103)
-- Name: IX_ForecastResult_ForecastAlertId_Date; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ForecastResult_ForecastAlertId_Date" ON public."ForecastResult" USING btree ("ForecastAlertId", "Date");


--
-- TOC entry 3837 (class 1259 OID 263105)
-- Name: IX_ObservationAlert_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationAlert_FieldObservationId" ON public."ObservationAlert" USING btree ("FieldObservationId");


--
-- TOC entry 3838 (class 1259 OID 263125)
-- Name: IX_ObservationAlert_WeatherStationId_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ObservationAlert_WeatherStationId_FieldObservationId" ON public."ObservationAlert" USING btree ("WeatherStationId", "FieldObservationId");


--
-- TOC entry 3844 (class 1259 OID 263107)
-- Name: IX_ObservationResult_ObservationAlertId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationResult_ObservationAlertId" ON public."ObservationResult" USING btree ("ObservationAlertId");


--
-- TOC entry 3815 (class 1259 OID 262891)
-- Name: IX_UserFarmType_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_UserFarmType_Description" ON public."UserFarmType" USING btree ("Description");


--
-- TOC entry 3803 (class 1259 OID 262773)
-- Name: IX_UserFarm_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_FarmId" ON public."UserFarm" USING btree ("FarmId");


--
-- TOC entry 3804 (class 1259 OID 262890)
-- Name: IX_UserFarm_UserFarmTypeDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_UserFarmTypeDescription" ON public."UserFarm" USING btree ("UserFarmTypeDescription");


--
-- TOC entry 3793 (class 1259 OID 261723)
-- Name: IX_UserProfile_UserAddressId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserProfile_UserAddressId" ON public."UserProfile" USING btree ("UserAddressId");


--
-- TOC entry 3872 (class 1259 OID 263279)
-- Name: IX_UserWidget_WidgetDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserWidget_WidgetDescription" ON public."UserWidget" USING btree ("WidgetDescription");


--
-- TOC entry 3869 (class 1259 OID 263280)
-- Name: IX_Widget_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Widget_Description" ON public."Widget" USING btree ("Description");


--
-- TOC entry 3919 (class 2606 OID 263349)
-- Name: CropPestDssResult FK_CropPestDss_CropPestDssResult; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "FK_CropPestDss_CropPestDssResult" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3899 (class 2606 OID 263008)
-- Name: FieldCropPest FK_CropPest_Crop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_CropPest_Crop" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3904 (class 2606 OID 263120)
-- Name: CropPestDss FK_CropPest_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "FK_CropPest_CropPestDss" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id");


--
-- TOC entry 3896 (class 2606 OID 262915)
-- Name: DataSharingRequest FK_DataSharingRequest_RequestStatus_RequestDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_RequestStatus_RequestDescription" FOREIGN KEY ("RequestStatusDescription") REFERENCES public."DataSharingRequestStatus"("Description");


--
-- TOC entry 3897 (class 2606 OID 262964)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesteeId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesteeId" FOREIGN KEY ("RequesteeId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3898 (class 2606 OID 262969)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesterId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesterId" FOREIGN KEY ("RequesterId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3907 (class 2606 OID 263185)
-- Name: FarmWeatherDataSource FK_FarmWeatherDataSource_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherDataSource"
    ADD CONSTRAINT "FK_FarmWeatherDataSource_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3908 (class 2606 OID 263219)
-- Name: FarmWeatherDataSource FK_FarmWeatherDataSource_WeatherDataSource; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherDataSource"
    ADD CONSTRAINT "FK_FarmWeatherDataSource_WeatherDataSource" FOREIGN KEY ("WeatherDataSourceId") REFERENCES public."WeatherDataSource"("Id") ON DELETE CASCADE;


--
-- TOC entry 3909 (class 2606 OID 263203)
-- Name: FarmWeatherStation FK_FarmWeatherStation_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3910 (class 2606 OID 263208)
-- Name: FarmWeatherStation FK_FarmWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 3905 (class 2606 OID 263142)
-- Name: FieldCropPestDss FK_FieldCropPestDss_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_CropPestDss" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3906 (class 2606 OID 263147)
-- Name: FieldCropPestDss FK_FieldCropPestDss_FieldCropPest_FieldCropPestId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_FieldCropPest_FieldCropPestId" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3900 (class 2606 OID 263294)
-- Name: FieldCropPest FK_FieldCropPest_FieldCrop_FieldCropId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_FieldCropPest_FieldCrop_FieldCropId" FOREIGN KEY ("FieldCropId") REFERENCES public."FieldCrop"("Id") ON DELETE CASCADE;


--
-- TOC entry 3920 (class 2606 OID 263363)
-- Name: FieldDssResult FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId" FOREIGN KEY ("FieldCropPestDssId") REFERENCES public."FieldCropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 3915 (class 2606 OID 263311)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3916 (class 2606 OID 263316)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_WeatherDataSource; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_WeatherDataSource" FOREIGN KEY ("WeatherDataSourceId") REFERENCES public."WeatherDataSource"("Id") ON DELETE CASCADE;


--
-- TOC entry 3917 (class 2606 OID 263329)
-- Name: FieldWeatherStation FK_FieldWeatherStation_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3918 (class 2606 OID 263334)
-- Name: FieldWeatherStation FK_FieldWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 3894 (class 2606 OID 262782)
-- Name: Field FK_Field_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "FK_Field_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3914 (class 2606 OID 263287)
-- Name: FieldCrop FK_Field_FieldCrop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "FK_Field_FieldCrop" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 3902 (class 2606 OID 263127)
-- Name: ForecastResult FK_ForecastResult_ForecastAlert_ForecastAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "FK_ForecastResult_ForecastAlert_ForecastAlertId" FOREIGN KEY ("ForecastAlertId") REFERENCES public."ForecastAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 3901 (class 2606 OID 263070)
-- Name: ObservationAlert FK_ObservationAlert_FieldObservation_FieldObservationId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "FK_ObservationAlert_FieldObservation_FieldObservationId" FOREIGN KEY ("FieldObservationId") REFERENCES public."FieldObservation"("Id");


--
-- TOC entry 3903 (class 2606 OID 263132)
-- Name: ObservationResult FK_ObservationResult_ObservationAlert_ObservationAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "FK_ObservationResult_ObservationAlert_ObservationAlertId" FOREIGN KEY ("ObservationAlertId") REFERENCES public."ObservationAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 3895 (class 2606 OID 263156)
-- Name: FieldObservation FK_Observation_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "FK_Observation_FieldCropPest" FOREIGN KEY ("FieldCropPestdId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3911 (class 2606 OID 263246)
-- Name: FieldSprayApplication FK_Spray_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "FK_Spray_FieldCropPest" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 3892 (class 2606 OID 262979)
-- Name: UserFarm FK_UserFarm_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 3893 (class 2606 OID 262984)
-- Name: UserFarm FK_UserFarm_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3891 (class 2606 OID 262892)
-- Name: UserFarm FK_UserFarm_UserFarmType_UserFarmTypeDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_UserFarmType_UserFarmTypeDescription" FOREIGN KEY ("UserFarmTypeDescription") REFERENCES public."UserFarmType"("Description");


--
-- TOC entry 3912 (class 2606 OID 263269)
-- Name: UserWidget FK_UserWidget_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 3913 (class 2606 OID 263274)
-- Name: UserWidget FK_UserWidget_Widget; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_Widget" FOREIGN KEY ("WidgetDescription") REFERENCES public."Widget"("Description") ON DELETE CASCADE;


--
-- TOC entry 3890 (class 2606 OID 262742)
-- Name: UserProfile FK_User_UserAddress; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "FK_User_UserAddress" FOREIGN KEY ("UserAddressId") REFERENCES public."UserAddress"("Id") ON DELETE CASCADE;


-- Completed on 2021-03-04 15:36:39 UTC

--
-- PostgreSQL database dump complete
--

