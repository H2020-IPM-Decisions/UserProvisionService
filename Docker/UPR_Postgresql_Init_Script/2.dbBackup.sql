--
-- PostgreSQL database dump
--

-- Dumped from database version 12.8
-- Dumped by pg_dump version 12.8

-- Started on 2023-01-20 09:04:11 UTC

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
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA IF NOT EXISTS public;

--
-- TOC entry 4785 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_table_access_method = heap;

--
-- TOC entry 247 (class 1259 OID 140473)
-- Name: AdministrationVariable; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AdministrationVariable" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL,
    "Value" text
);


--
-- TOC entry 229 (class 1259 OID 139912)
-- Name: CropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPest" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "PestEppoCode" character varying(6) NOT NULL
);


--
-- TOC entry 235 (class 1259 OID 140026)
-- Name: CropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPestDss" (
    "Id" uuid NOT NULL,
    "CropPestId" uuid NOT NULL,
    "DssId" text NOT NULL,
    "DssModelId" text DEFAULT ''::text NOT NULL,
    "DssExecutionType" text DEFAULT ''::text NOT NULL,
    "DssEndPoint" text,
    "DssModelName" text DEFAULT ''::text NOT NULL,
    "DssName" text NOT NULL,
    "DssModelVersion" text DEFAULT ''::text NOT NULL,
    "DssVersion" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 241 (class 1259 OID 140255)
-- Name: CropPestDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPestDssResult" (
    "Id" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL
);


--
-- TOC entry 228 (class 1259 OID 139821)
-- Name: DataSharingRequest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequest" (
    "Id" uuid NOT NULL,
    "RequesteeId" uuid NOT NULL,
    "RequesterId" uuid NOT NULL,
    "RequestStatusDescription" text NOT NULL
);


--
-- TOC entry 227 (class 1259 OID 139811)
-- Name: DataSharingRequestStatus; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequestStatus" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 246 (class 1259 OID 140464)
-- Name: EppoCode; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EppoCode" (
    "Type" text NOT NULL,
    "Data" jsonb NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 139663)
-- Name: Farm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Farm" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Location" public.geometry(Point) NOT NULL,
    "WeatherForecastId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "WeatherHistoricalId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 139688)
-- Name: Field; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Field" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "FarmId" uuid NOT NULL,
    "SowingDate" timestamp without time zone,
    "Location" public.geometry(Point)
);


--
-- TOC entry 240 (class 1259 OID 140196)
-- Name: FieldCrop; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCrop" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "FieldId" uuid NOT NULL
);


--
-- TOC entry 230 (class 1259 OID 139917)
-- Name: FieldCropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPest" (
    "CropPestId" uuid NOT NULL,
    "Id" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "FieldCropId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 236 (class 1259 OID 140051)
-- Name: FieldCropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPestDss" (
    "Id" uuid NOT NULL,
    "FieldCropPestId" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "DssParameters" jsonb,
    "ObservationRequired" boolean DEFAULT false NOT NULL,
    "LastJobId" text,
    "ReScheduleCount" integer DEFAULT 0 NOT NULL,
    "IsCustomDss" boolean DEFAULT false NOT NULL,
    "CustomName" text
);


--
-- TOC entry 243 (class 1259 OID 140362)
-- Name: FieldDssObservation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldDssObservation" (
    "Id" uuid NOT NULL,
    "FieldCropPestDssId" uuid NOT NULL,
    "DssObservation" jsonb,
    "Location" public.geometry NOT NULL,
    "Time" timestamp without time zone DEFAULT now() NOT NULL
);


--
-- TOC entry 242 (class 1259 OID 140269)
-- Name: FieldDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldDssResult" (
    "Id" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL,
    "DssFullResult" jsonb,
    "FieldCropPestDssId" uuid NOT NULL,
    "IsValid" boolean DEFAULT false NOT NULL,
    "WarningMessage" text,
    "WarningStatus" integer,
    "ResultMessage" text,
    "ResultMessageType" integer
);


--
-- TOC entry 225 (class 1259 OID 139702)
-- Name: FieldObservation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldObservation" (
    "Id" uuid NOT NULL,
    "Location" public.geometry,
    "Time" timestamp without time zone DEFAULT now() NOT NULL,
    "Severity" text,
    "FieldCropPestId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "DssObservation" text
);


--
-- TOC entry 237 (class 1259 OID 140142)
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
-- TOC entry 231 (class 1259 OID 139964)
-- Name: ForecastAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 139989)
-- Name: ForecastResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastResult" (
    "Id" uuid NOT NULL,
    "ForecastAlertId" uuid NOT NULL,
    "Date" timestamp without time zone NOT NULL
);


--
-- TOC entry 232 (class 1259 OID 139974)
-- Name: ObservationAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL,
    "FieldObservationId" uuid NOT NULL
);


--
-- TOC entry 234 (class 1259 OID 140002)
-- Name: ObservationResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationResult" (
    "Id" uuid NOT NULL,
    "ObservationAlertId" uuid NOT NULL,
    "Date" timestamp without time zone NOT NULL
);


--
-- TOC entry 206 (class 1259 OID 138063)
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
-- TOC entry 223 (class 1259 OID 139671)
-- Name: UserFarm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarm" (
    "UserId" uuid NOT NULL,
    "FarmId" uuid NOT NULL,
    "Authorised" boolean DEFAULT false NOT NULL,
    "UserFarmTypeDescription" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 226 (class 1259 OID 139794)
-- Name: UserFarmType; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarmType" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 205 (class 1259 OID 138053)
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
-- TOC entry 239 (class 1259 OID 140175)
-- Name: UserWidget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserWidget" (
    "UserId" uuid NOT NULL,
    "WidgetId" integer NOT NULL,
    "WidgetDescription" text NOT NULL,
    "Allowed" boolean NOT NULL
);


--
-- TOC entry 244 (class 1259 OID 140383)
-- Name: WeatherForecast; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherForecast" (
    "Id" uuid NOT NULL,
    "WeatherId" text NOT NULL,
    "Name" text NOT NULL,
    "Url" text NOT NULL
);


--
-- TOC entry 245 (class 1259 OID 140425)
-- Name: WeatherHistorical; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherHistorical" (
    "Id" uuid NOT NULL,
    "WeatherId" text NOT NULL,
    "Name" text NOT NULL,
    "Url" text NOT NULL
);


--
-- TOC entry 238 (class 1259 OID 140165)
-- Name: Widget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Widget" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 204 (class 1259 OID 138048)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- TOC entry 4522 (class 2606 OID 139820)
-- Name: DataSharingRequestStatus AK_DataSharingRequestStatus_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "AK_DataSharingRequestStatus_Description" UNIQUE ("Description");


--
-- TOC entry 4517 (class 2606 OID 139803)
-- Name: UserFarmType AK_UserFarmType_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "AK_UserFarmType_Description" UNIQUE ("Description");


--
-- TOC entry 4561 (class 2606 OID 140174)
-- Name: Widget AK_Widget_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "AK_Widget_Description" UNIQUE ("Description");


--
-- TOC entry 4591 (class 2606 OID 140480)
-- Name: AdministrationVariable PK_AdministrationVariable; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AdministrationVariable"
    ADD CONSTRAINT "PK_AdministrationVariable" PRIMARY KEY ("Id");


--
-- TOC entry 4533 (class 2606 OID 139916)
-- Name: CropPest PK_CropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPest"
    ADD CONSTRAINT "PK_CropPest" PRIMARY KEY ("Id");


--
-- TOC entry 4552 (class 2606 OID 140033)
-- Name: CropPestDss PK_CropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "PK_CropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 4573 (class 2606 OID 140262)
-- Name: CropPestDssResult PK_CropPestDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "PK_CropPestDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 4530 (class 2606 OID 139904)
-- Name: DataSharingRequest PK_DataSharingRequest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "PK_DataSharingRequest" PRIMARY KEY ("Id");


--
-- TOC entry 4525 (class 2606 OID 139818)
-- Name: DataSharingRequestStatus PK_DataSharingRequestStatus; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "PK_DataSharingRequestStatus" PRIMARY KEY ("Id");


--
-- TOC entry 4588 (class 2606 OID 140471)
-- Name: EppoCode PK_EppoCode; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EppoCode"
    ADD CONSTRAINT "PK_EppoCode" PRIMARY KEY ("Type");


--
-- TOC entry 4505 (class 2606 OID 139670)
-- Name: Farm PK_Farm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "PK_Farm" PRIMARY KEY ("Id");


--
-- TOC entry 4512 (class 2606 OID 139695)
-- Name: Field PK_Field; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "PK_Field" PRIMARY KEY ("Id");


--
-- TOC entry 4570 (class 2606 OID 140200)
-- Name: FieldCrop PK_FieldCrop; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "PK_FieldCrop" PRIMARY KEY ("Id");


--
-- TOC entry 4537 (class 2606 OID 140024)
-- Name: FieldCropPest PK_FieldCropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "PK_FieldCropPest" PRIMARY KEY ("Id");


--
-- TOC entry 4556 (class 2606 OID 140055)
-- Name: FieldCropPestDss PK_FieldCropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "PK_FieldCropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 4579 (class 2606 OID 140370)
-- Name: FieldDssObservation PK_FieldDssObservation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssObservation"
    ADD CONSTRAINT "PK_FieldDssObservation" PRIMARY KEY ("Id");


--
-- TOC entry 4576 (class 2606 OID 140276)
-- Name: FieldDssResult PK_FieldDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "PK_FieldDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 4515 (class 2606 OID 139709)
-- Name: FieldObservation PK_FieldObservation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "PK_FieldObservation" PRIMARY KEY ("Id");


--
-- TOC entry 4559 (class 2606 OID 140147)
-- Name: FieldSprayApplication PK_FieldSprayApplication; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "PK_FieldSprayApplication" PRIMARY KEY ("Id");


--
-- TOC entry 4539 (class 2606 OID 139968)
-- Name: ForecastAlert PK_ForecastAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastAlert"
    ADD CONSTRAINT "PK_ForecastAlert" PRIMARY KEY ("Id");


--
-- TOC entry 4546 (class 2606 OID 139996)
-- Name: ForecastResult PK_ForecastResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "PK_ForecastResult" PRIMARY KEY ("Id");


--
-- TOC entry 4543 (class 2606 OID 139978)
-- Name: ObservationAlert PK_ObservationAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "PK_ObservationAlert" PRIMARY KEY ("Id");


--
-- TOC entry 4549 (class 2606 OID 140009)
-- Name: ObservationResult PK_ObservationResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "PK_ObservationResult" PRIMARY KEY ("Id");


--
-- TOC entry 4498 (class 2606 OID 138070)
-- Name: UserAddress PK_UserAddress; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserAddress"
    ADD CONSTRAINT "PK_UserAddress" PRIMARY KEY ("Id");


--
-- TOC entry 4509 (class 2606 OID 139877)
-- Name: UserFarm PK_UserFarm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "PK_UserFarm" PRIMARY KEY ("UserId", "FarmId");


--
-- TOC entry 4520 (class 2606 OID 139801)
-- Name: UserFarmType PK_UserFarmType; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "PK_UserFarmType" PRIMARY KEY ("Id");


--
-- TOC entry 4496 (class 2606 OID 139875)
-- Name: UserProfile PK_UserProfile; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "PK_UserProfile" PRIMARY KEY ("UserId");


--
-- TOC entry 4567 (class 2606 OID 140182)
-- Name: UserWidget PK_UserWidget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "PK_UserWidget" PRIMARY KEY ("UserId", "WidgetId");


--
-- TOC entry 4582 (class 2606 OID 140390)
-- Name: WeatherForecast PK_WeatherForecast; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherForecast"
    ADD CONSTRAINT "PK_WeatherForecast" PRIMARY KEY ("Id");


--
-- TOC entry 4585 (class 2606 OID 140432)
-- Name: WeatherHistorical PK_WeatherHistorical; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherHistorical"
    ADD CONSTRAINT "PK_WeatherHistorical" PRIMARY KEY ("Id");


--
-- TOC entry 4564 (class 2606 OID 140172)
-- Name: Widget PK_Widget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "PK_Widget" PRIMARY KEY ("Id");


--
-- TOC entry 4493 (class 2606 OID 138052)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 4589 (class 1259 OID 140481)
-- Name: IX_AdministrationVariable_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_AdministrationVariable_Description" ON public."AdministrationVariable" USING btree ("Description");


--
-- TOC entry 4571 (class 1259 OID 140268)
-- Name: IX_CropPestDssResult_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CropPestDssResult_CropPestDssId" ON public."CropPestDssResult" USING btree ("CropPestDssId");


--
-- TOC entry 4550 (class 1259 OID 140485)
-- Name: IX_CropPestDss_All; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPestDss_All" ON public."CropPestDss" USING btree ("CropPestId", "DssId", "DssVersion", "DssModelId", "DssModelVersion", "DssExecutionType");


--
-- TOC entry 4531 (class 1259 OID 139933)
-- Name: IX_CropPest_CropEppoCode_PestEppoCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPest_CropEppoCode_PestEppoCode" ON public."CropPest" USING btree ("CropEppoCode", "PestEppoCode");


--
-- TOC entry 4523 (class 1259 OID 139847)
-- Name: IX_DataSharingRequestStatus_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequestStatus_Description" ON public."DataSharingRequestStatus" USING btree ("Description");


--
-- TOC entry 4526 (class 1259 OID 139844)
-- Name: IX_DataSharingRequest_RequestStatusDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequestStatusDescription" ON public."DataSharingRequest" USING btree ("RequestStatusDescription");


--
-- TOC entry 4527 (class 1259 OID 139905)
-- Name: IX_DataSharingRequest_RequesteeId_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequest_RequesteeId_RequesterId" ON public."DataSharingRequest" USING btree ("RequesteeId", "RequesterId");


--
-- TOC entry 4528 (class 1259 OID 139846)
-- Name: IX_DataSharingRequest_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequesterId" ON public."DataSharingRequest" USING btree ("RequesterId");


--
-- TOC entry 4586 (class 1259 OID 140472)
-- Name: IX_EppoCode_Type; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_EppoCode_Type" ON public."EppoCode" USING btree ("Type");


--
-- TOC entry 4501 (class 1259 OID 140138)
-- Name: IX_Farm_Location; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_Location" ON public."Farm" USING btree ("Location");


--
-- TOC entry 4502 (class 1259 OID 140409)
-- Name: IX_Farm_WeatherForecastId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_WeatherForecastId" ON public."Farm" USING btree ("WeatherForecastId");


--
-- TOC entry 4503 (class 1259 OID 140433)
-- Name: IX_Farm_WeatherHistoricalId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_WeatherHistoricalId" ON public."Farm" USING btree ("WeatherHistoricalId");


--
-- TOC entry 4553 (class 1259 OID 140066)
-- Name: IX_FieldCropPestDss_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPestDss_CropPestDssId" ON public."FieldCropPestDss" USING btree ("CropPestDssId");


--
-- TOC entry 4554 (class 1259 OID 140067)
-- Name: IX_FieldCropPestDss_FieldCropPestId_CropPestDssId_IsCustomDss_~; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId_IsCustomDss_~" ON public."FieldCropPestDss" USING btree ("FieldCropPestId", "CropPestDssId", "IsCustomDss", "CustomName");


--
-- TOC entry 4534 (class 1259 OID 139934)
-- Name: IX_FieldCropPest_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPest_CropPestId" ON public."FieldCropPest" USING btree ("CropPestId");


--
-- TOC entry 4535 (class 1259 OID 140206)
-- Name: IX_FieldCropPest_FieldCropId_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPest_FieldCropId_CropPestId" ON public."FieldCropPest" USING btree ("FieldCropId", "CropPestId");


--
-- TOC entry 4568 (class 1259 OID 140213)
-- Name: IX_FieldCrop_FieldId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCrop_FieldId" ON public."FieldCrop" USING btree ("FieldId");


--
-- TOC entry 4577 (class 1259 OID 140453)
-- Name: IX_FieldDssObservation_FieldCropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldDssObservation_FieldCropPestDssId" ON public."FieldDssObservation" USING btree ("FieldCropPestDssId");


--
-- TOC entry 4574 (class 1259 OID 140282)
-- Name: IX_FieldDssResult_FieldCropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldDssResult_FieldCropPestDssId" ON public."FieldDssResult" USING btree ("FieldCropPestDssId");


--
-- TOC entry 4513 (class 1259 OID 140376)
-- Name: IX_FieldObservation_FieldCropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldObservation_FieldCropPestId" ON public."FieldObservation" USING btree ("FieldCropPestId");


--
-- TOC entry 4557 (class 1259 OID 140159)
-- Name: IX_FieldSprayApplication_FieldCropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldSprayApplication_FieldCropPestId" ON public."FieldSprayApplication" USING btree ("FieldCropPestId");


--
-- TOC entry 4510 (class 1259 OID 139701)
-- Name: IX_Field_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Field_FarmId" ON public."Field" USING btree ("FarmId");


--
-- TOC entry 4544 (class 1259 OID 140017)
-- Name: IX_ForecastResult_ForecastAlertId_Date; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ForecastResult_ForecastAlertId_Date" ON public."ForecastResult" USING btree ("ForecastAlertId", "Date");


--
-- TOC entry 4540 (class 1259 OID 140019)
-- Name: IX_ObservationAlert_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationAlert_FieldObservationId" ON public."ObservationAlert" USING btree ("FieldObservationId");


--
-- TOC entry 4541 (class 1259 OID 140039)
-- Name: IX_ObservationAlert_WeatherStationId_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ObservationAlert_WeatherStationId_FieldObservationId" ON public."ObservationAlert" USING btree ("WeatherStationId", "FieldObservationId");


--
-- TOC entry 4547 (class 1259 OID 140021)
-- Name: IX_ObservationResult_ObservationAlertId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationResult_ObservationAlertId" ON public."ObservationResult" USING btree ("ObservationAlertId");


--
-- TOC entry 4518 (class 1259 OID 139805)
-- Name: IX_UserFarmType_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_UserFarmType_Description" ON public."UserFarmType" USING btree ("Description");


--
-- TOC entry 4506 (class 1259 OID 139687)
-- Name: IX_UserFarm_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_FarmId" ON public."UserFarm" USING btree ("FarmId");


--
-- TOC entry 4507 (class 1259 OID 139804)
-- Name: IX_UserFarm_UserFarmTypeDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_UserFarmTypeDescription" ON public."UserFarm" USING btree ("UserFarmTypeDescription");


--
-- TOC entry 4494 (class 1259 OID 138071)
-- Name: IX_UserProfile_UserAddressId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserProfile_UserAddressId" ON public."UserProfile" USING btree ("UserAddressId");


--
-- TOC entry 4565 (class 1259 OID 140193)
-- Name: IX_UserWidget_WidgetDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserWidget_WidgetDescription" ON public."UserWidget" USING btree ("WidgetDescription");


--
-- TOC entry 4580 (class 1259 OID 140415)
-- Name: IX_WeatherForecast_WeatherId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_WeatherForecast_WeatherId" ON public."WeatherForecast" USING btree ("WeatherId");


--
-- TOC entry 4583 (class 1259 OID 140434)
-- Name: IX_WeatherHistorical_WeatherId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_WeatherHistorical_WeatherId" ON public."WeatherHistorical" USING btree ("WeatherId");


--
-- TOC entry 4562 (class 1259 OID 140194)
-- Name: IX_Widget_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Widget_Description" ON public."Widget" USING btree ("Description");


--
-- TOC entry 4615 (class 2606 OID 140263)
-- Name: CropPestDssResult FK_CropPestDss_CropPestDssResult; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "FK_CropPestDss_CropPestDssResult" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4603 (class 2606 OID 139922)
-- Name: FieldCropPest FK_CropPest_Crop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_CropPest_Crop" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4608 (class 2606 OID 140034)
-- Name: CropPestDss FK_CropPest_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "FK_CropPest_CropPestDss" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id");


--
-- TOC entry 4600 (class 2606 OID 139829)
-- Name: DataSharingRequest FK_DataSharingRequest_RequestStatus_RequestDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_RequestStatus_RequestDescription" FOREIGN KEY ("RequestStatusDescription") REFERENCES public."DataSharingRequestStatus"("Description");


--
-- TOC entry 4601 (class 2606 OID 139878)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesteeId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesteeId" FOREIGN KEY ("RequesteeId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4602 (class 2606 OID 139883)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesterId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesterId" FOREIGN KEY ("RequesterId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4593 (class 2606 OID 140435)
-- Name: Farm FK_Farm_WeatherForecast; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "FK_Farm_WeatherForecast" FOREIGN KEY ("WeatherForecastId") REFERENCES public."WeatherForecast"("Id");


--
-- TOC entry 4594 (class 2606 OID 140440)
-- Name: Farm FK_Farm_WeatherHistorical; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "FK_Farm_WeatherHistorical" FOREIGN KEY ("WeatherHistoricalId") REFERENCES public."WeatherHistorical"("Id");


--
-- TOC entry 4609 (class 2606 OID 140056)
-- Name: FieldCropPestDss FK_FieldCropPestDss_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_CropPestDss" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4610 (class 2606 OID 140061)
-- Name: FieldCropPestDss FK_FieldCropPestDss_FieldCropPest_FieldCropPestId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_FieldCropPest_FieldCropPestId" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4604 (class 2606 OID 140208)
-- Name: FieldCropPest FK_FieldCropPest_FieldCrop_FieldCropId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_FieldCropPest_FieldCrop_FieldCropId" FOREIGN KEY ("FieldCropId") REFERENCES public."FieldCrop"("Id") ON DELETE CASCADE;


--
-- TOC entry 4616 (class 2606 OID 140277)
-- Name: FieldDssResult FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId" FOREIGN KEY ("FieldCropPestDssId") REFERENCES public."FieldCropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4598 (class 2606 OID 139696)
-- Name: Field FK_Field_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "FK_Field_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4614 (class 2606 OID 140201)
-- Name: FieldCrop FK_Field_FieldCrop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "FK_Field_FieldCrop" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 4606 (class 2606 OID 140041)
-- Name: ForecastResult FK_ForecastResult_ForecastAlert_ForecastAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "FK_ForecastResult_ForecastAlert_ForecastAlertId" FOREIGN KEY ("ForecastAlertId") REFERENCES public."ForecastAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 4605 (class 2606 OID 139984)
-- Name: ObservationAlert FK_ObservationAlert_FieldObservation_FieldObservationId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "FK_ObservationAlert_FieldObservation_FieldObservationId" FOREIGN KEY ("FieldObservationId") REFERENCES public."FieldObservation"("Id");


--
-- TOC entry 4607 (class 2606 OID 140046)
-- Name: ObservationResult FK_ObservationResult_ObservationAlert_ObservationAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "FK_ObservationResult_ObservationAlert_ObservationAlertId" FOREIGN KEY ("ObservationAlertId") REFERENCES public."ObservationAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 4599 (class 2606 OID 140378)
-- Name: FieldObservation FK_Observation_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "FK_Observation_FieldCropPest" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4617 (class 2606 OID 140454)
-- Name: FieldDssObservation FK_Observation_FieldCropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssObservation"
    ADD CONSTRAINT "FK_Observation_FieldCropPestDss" FOREIGN KEY ("FieldCropPestDssId") REFERENCES public."FieldCropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4611 (class 2606 OID 140160)
-- Name: FieldSprayApplication FK_Spray_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "FK_Spray_FieldCropPest" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4596 (class 2606 OID 139893)
-- Name: UserFarm FK_UserFarm_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4597 (class 2606 OID 139898)
-- Name: UserFarm FK_UserFarm_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4595 (class 2606 OID 139806)
-- Name: UserFarm FK_UserFarm_UserFarmType_UserFarmTypeDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_UserFarmType_UserFarmTypeDescription" FOREIGN KEY ("UserFarmTypeDescription") REFERENCES public."UserFarmType"("Description");


--
-- TOC entry 4612 (class 2606 OID 140183)
-- Name: UserWidget FK_UserWidget_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4613 (class 2606 OID 140188)
-- Name: UserWidget FK_UserWidget_Widget; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_Widget" FOREIGN KEY ("WidgetDescription") REFERENCES public."Widget"("Description") ON DELETE CASCADE;


--
-- TOC entry 4592 (class 2606 OID 139656)
-- Name: UserProfile FK_User_UserAddress; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "FK_User_UserAddress" FOREIGN KEY ("UserAddressId") REFERENCES public."UserAddress"("Id") ON DELETE CASCADE;


-- Completed on 2023-01-20 09:04:13 UTC

--
-- PostgreSQL database dump complete
--

