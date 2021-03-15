--
-- PostgreSQL database dump
--

-- Dumped from database version 12.5 (Debian 12.5-1.pgdg100+1)
-- Dumped by pg_dump version 12.5 (Debian 12.5-1.pgdg100+1)

-- Started on 2021-03-15 14:36:38 UTC

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
-- TOC entry 6 (class 2615 OID 263879)
-- Name: hangfire; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA hangfire;


--
-- TOC entry 2 (class 3079 OID 264217)
-- Name: postgis; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS postgis WITH SCHEMA public;


--
-- TOC entry 4228 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION postgis; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON EXTENSION postgis IS 'PostGIS geometry and geography spatial types and functions';


SET default_table_access_method = heap;

--
-- TOC entry 206 (class 1259 OID 263887)
-- Name: counter; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.counter (
    id bigint NOT NULL,
    key text NOT NULL,
    value bigint NOT NULL,
    expireat timestamp without time zone
);


--
-- TOC entry 205 (class 1259 OID 263885)
-- Name: counter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.counter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4229 (class 0 OID 0)
-- Dependencies: 205
-- Name: counter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.counter_id_seq OWNED BY hangfire.counter.id;


--
-- TOC entry 208 (class 1259 OID 263896)
-- Name: hash; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.hash (
    id bigint NOT NULL,
    key text NOT NULL,
    field text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 207 (class 1259 OID 263894)
-- Name: hash_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.hash_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4230 (class 0 OID 0)
-- Dependencies: 207
-- Name: hash_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.hash_id_seq OWNED BY hangfire.hash.id;


--
-- TOC entry 210 (class 1259 OID 263909)
-- Name: job; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.job (
    id bigint NOT NULL,
    stateid bigint,
    statename text,
    invocationdata text NOT NULL,
    arguments text NOT NULL,
    createdat timestamp without time zone NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 209 (class 1259 OID 263907)
-- Name: job_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.job_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4231 (class 0 OID 0)
-- Dependencies: 209
-- Name: job_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.job_id_seq OWNED BY hangfire.job.id;


--
-- TOC entry 221 (class 1259 OID 263979)
-- Name: jobparameter; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.jobparameter (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    value text,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 263977)
-- Name: jobparameter_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.jobparameter_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4232 (class 0 OID 0)
-- Dependencies: 220
-- Name: jobparameter_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.jobparameter_id_seq OWNED BY hangfire.jobparameter.id;


--
-- TOC entry 214 (class 1259 OID 263938)
-- Name: jobqueue; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.jobqueue (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    queue text NOT NULL,
    fetchedat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 213 (class 1259 OID 263936)
-- Name: jobqueue_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.jobqueue_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4233 (class 0 OID 0)
-- Dependencies: 213
-- Name: jobqueue_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.jobqueue_id_seq OWNED BY hangfire.jobqueue.id;


--
-- TOC entry 216 (class 1259 OID 263947)
-- Name: list; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.list (
    id bigint NOT NULL,
    key text NOT NULL,
    value text,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 215 (class 1259 OID 263945)
-- Name: list_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.list_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4234 (class 0 OID 0)
-- Dependencies: 215
-- Name: list_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.list_id_seq OWNED BY hangfire.list.id;


--
-- TOC entry 222 (class 1259 OID 263994)
-- Name: lock; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.lock (
    resource text NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL,
    acquired timestamp without time zone
);


--
-- TOC entry 204 (class 1259 OID 263880)
-- Name: schema; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.schema (
    version integer NOT NULL
);


--
-- TOC entry 217 (class 1259 OID 263956)
-- Name: server; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.server (
    id text NOT NULL,
    data text,
    lastheartbeat timestamp without time zone NOT NULL,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 219 (class 1259 OID 263966)
-- Name: set; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.set (
    id bigint NOT NULL,
    key text NOT NULL,
    score double precision NOT NULL,
    value text NOT NULL,
    expireat timestamp without time zone,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 218 (class 1259 OID 263964)
-- Name: set_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.set_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4235 (class 0 OID 0)
-- Dependencies: 218
-- Name: set_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.set_id_seq OWNED BY hangfire.set.id;


--
-- TOC entry 212 (class 1259 OID 263921)
-- Name: state; Type: TABLE; Schema: hangfire; Owner: -
--

CREATE TABLE hangfire.state (
    id bigint NOT NULL,
    jobid bigint NOT NULL,
    name text NOT NULL,
    reason text,
    createdat timestamp without time zone NOT NULL,
    data text,
    updatecount integer DEFAULT 0 NOT NULL
);


--
-- TOC entry 211 (class 1259 OID 263919)
-- Name: state_id_seq; Type: SEQUENCE; Schema: hangfire; Owner: -
--

CREATE SEQUENCE hangfire.state_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4236 (class 0 OID 0)
-- Dependencies: 211
-- Name: state_id_seq; Type: SEQUENCE OWNED BY; Schema: hangfire; Owner: -
--

ALTER SEQUENCE hangfire.state_id_seq OWNED BY hangfire.state.id;


--
-- TOC entry 238 (class 1259 OID 265486)
-- Name: CropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CropPest" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "PestEppoCode" character varying(6) NOT NULL
);


--
-- TOC entry 244 (class 1259 OID 265600)
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
-- TOC entry 255 (class 1259 OID 265829)
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
-- TOC entry 237 (class 1259 OID 265395)
-- Name: DataSharingRequest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequest" (
    "Id" uuid NOT NULL,
    "RequesteeId" uuid NOT NULL,
    "RequesterId" uuid NOT NULL,
    "RequestStatusDescription" text NOT NULL
);


--
-- TOC entry 236 (class 1259 OID 265385)
-- Name: DataSharingRequestStatus; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DataSharingRequestStatus" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 231 (class 1259 OID 265237)
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
-- TOC entry 248 (class 1259 OID 265683)
-- Name: FarmWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FarmWeatherStation" (
    "FarmId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 265262)
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
-- TOC entry 252 (class 1259 OID 265770)
-- Name: FieldCrop; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCrop" (
    "Id" uuid NOT NULL,
    "CropEppoCode" character varying(6) NOT NULL,
    "FieldId" uuid NOT NULL
);


--
-- TOC entry 239 (class 1259 OID 265491)
-- Name: FieldCropPest; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPest" (
    "CropPestId" uuid NOT NULL,
    "Id" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL,
    "FieldCropId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 245 (class 1259 OID 265625)
-- Name: FieldCropPestDss; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldCropPestDss" (
    "Id" uuid NOT NULL,
    "FieldCropPestId" uuid NOT NULL,
    "CropPestDssId" uuid NOT NULL,
    "DssParameters" jsonb
);


--
-- TOC entry 256 (class 1259 OID 265843)
-- Name: FieldDssResult; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldDssResult" (
    "Id" uuid NOT NULL,
    "CreationDate" timestamp without time zone NOT NULL,
    "Result" jsonb,
    "FieldCropPestDssId" uuid NOT NULL
);


--
-- TOC entry 234 (class 1259 OID 265276)
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
-- TOC entry 249 (class 1259 OID 265716)
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
-- TOC entry 253 (class 1259 OID 265791)
-- Name: FieldWeatherDataSource; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherDataSource" (
    "FieldId" uuid NOT NULL,
    "WeatherDataSourceId" uuid DEFAULT '00000000-0000-0000-0000-000000000000'::uuid NOT NULL
);


--
-- TOC entry 254 (class 1259 OID 265809)
-- Name: FieldWeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FieldWeatherStation" (
    "FieldId" uuid NOT NULL,
    "WeatherStationId" text NOT NULL
);


--
-- TOC entry 240 (class 1259 OID 265538)
-- Name: ForecastAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ForecastAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL
);


--
-- TOC entry 242 (class 1259 OID 265563)
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
-- TOC entry 241 (class 1259 OID 265548)
-- Name: ObservationAlert; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ObservationAlert" (
    "Id" uuid NOT NULL,
    "WeatherStationId" uuid NOT NULL,
    "FieldObservationId" uuid NOT NULL
);


--
-- TOC entry 243 (class 1259 OID 265576)
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
-- TOC entry 225 (class 1259 OID 264202)
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
-- TOC entry 232 (class 1259 OID 265245)
-- Name: UserFarm; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarm" (
    "UserId" uuid NOT NULL,
    "FarmId" uuid NOT NULL,
    "Authorised" boolean DEFAULT false NOT NULL,
    "UserFarmTypeDescription" text DEFAULT ''::text NOT NULL
);


--
-- TOC entry 235 (class 1259 OID 265368)
-- Name: UserFarmType; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserFarmType" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 264192)
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
-- TOC entry 251 (class 1259 OID 265749)
-- Name: UserWidget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserWidget" (
    "UserId" uuid NOT NULL,
    "WidgetId" integer NOT NULL,
    "WidgetDescription" text NOT NULL,
    "Allowed" boolean NOT NULL
);


--
-- TOC entry 246 (class 1259 OID 265649)
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
-- TOC entry 247 (class 1259 OID 265657)
-- Name: WeatherStation; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WeatherStation" (
    "Id" text NOT NULL,
    "Name" text
);


--
-- TOC entry 250 (class 1259 OID 265739)
-- Name: Widget; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Widget" (
    "Id" integer NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 264187)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- TOC entry 3843 (class 2604 OID 264029)
-- Name: counter id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.counter ALTER COLUMN id SET DEFAULT nextval('hangfire.counter_id_seq'::regclass);


--
-- TOC entry 3845 (class 2604 OID 264038)
-- Name: hash id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.hash ALTER COLUMN id SET DEFAULT nextval('hangfire.hash_id_seq'::regclass);


--
-- TOC entry 3847 (class 2604 OID 264049)
-- Name: job id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.job ALTER COLUMN id SET DEFAULT nextval('hangfire.job_id_seq'::regclass);


--
-- TOC entry 3858 (class 2604 OID 264103)
-- Name: jobparameter id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.jobparameter ALTER COLUMN id SET DEFAULT nextval('hangfire.jobparameter_id_seq'::regclass);


--
-- TOC entry 3851 (class 2604 OID 264128)
-- Name: jobqueue id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.jobqueue ALTER COLUMN id SET DEFAULT nextval('hangfire.jobqueue_id_seq'::regclass);


--
-- TOC entry 3853 (class 2604 OID 264150)
-- Name: list id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.list ALTER COLUMN id SET DEFAULT nextval('hangfire.list_id_seq'::regclass);


--
-- TOC entry 3856 (class 2604 OID 264160)
-- Name: set id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.set ALTER COLUMN id SET DEFAULT nextval('hangfire.set_id_seq'::regclass);


--
-- TOC entry 3849 (class 2604 OID 264078)
-- Name: state id; Type: DEFAULT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.state ALTER COLUMN id SET DEFAULT nextval('hangfire.state_id_seq'::regclass);


--
-- TOC entry 3884 (class 2606 OID 264031)
-- Name: counter counter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.counter
    ADD CONSTRAINT counter_pkey PRIMARY KEY (id);


--
-- TOC entry 3888 (class 2606 OID 264178)
-- Name: hash hash_key_field_key; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_key_field_key UNIQUE (key, field);


--
-- TOC entry 3890 (class 2606 OID 264040)
-- Name: hash hash_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.hash
    ADD CONSTRAINT hash_pkey PRIMARY KEY (id);


--
-- TOC entry 3893 (class 2606 OID 264051)
-- Name: job job_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- TOC entry 3912 (class 2606 OID 264105)
-- Name: jobparameter jobparameter_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_pkey PRIMARY KEY (id);


--
-- TOC entry 3900 (class 2606 OID 264130)
-- Name: jobqueue jobqueue_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.jobqueue
    ADD CONSTRAINT jobqueue_pkey PRIMARY KEY (id);


--
-- TOC entry 3903 (class 2606 OID 264152)
-- Name: list list_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.list
    ADD CONSTRAINT list_pkey PRIMARY KEY (id);


--
-- TOC entry 3914 (class 2606 OID 264020)
-- Name: lock lock_resource_key; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.lock
    ADD CONSTRAINT lock_resource_key UNIQUE (resource);


--
-- TOC entry 3882 (class 2606 OID 263884)
-- Name: schema schema_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.schema
    ADD CONSTRAINT schema_pkey PRIMARY KEY (version);


--
-- TOC entry 3905 (class 2606 OID 264181)
-- Name: server server_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.server
    ADD CONSTRAINT server_pkey PRIMARY KEY (id);


--
-- TOC entry 3907 (class 2606 OID 264183)
-- Name: set set_key_value_key; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_key_value_key UNIQUE (key, value);


--
-- TOC entry 3909 (class 2606 OID 264162)
-- Name: set set_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.set
    ADD CONSTRAINT set_pkey PRIMARY KEY (id);


--
-- TOC entry 3896 (class 2606 OID 264080)
-- Name: state state_pkey; Type: CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_pkey PRIMARY KEY (id);


--
-- TOC entry 3943 (class 2606 OID 265394)
-- Name: DataSharingRequestStatus AK_DataSharingRequestStatus_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "AK_DataSharingRequestStatus_Description" UNIQUE ("Description");


--
-- TOC entry 3938 (class 2606 OID 265377)
-- Name: UserFarmType AK_UserFarmType_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "AK_UserFarmType_Description" UNIQUE ("Description");


--
-- TOC entry 3990 (class 2606 OID 265748)
-- Name: Widget AK_Widget_Description; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "AK_Widget_Description" UNIQUE ("Description");


--
-- TOC entry 3954 (class 2606 OID 265490)
-- Name: CropPest PK_CropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPest"
    ADD CONSTRAINT "PK_CropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3973 (class 2606 OID 265607)
-- Name: CropPestDss PK_CropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "PK_CropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 4008 (class 2606 OID 265836)
-- Name: CropPestDssResult PK_CropPestDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "PK_CropPestDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3951 (class 2606 OID 265478)
-- Name: DataSharingRequest PK_DataSharingRequest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "PK_DataSharingRequest" PRIMARY KEY ("Id");


--
-- TOC entry 3946 (class 2606 OID 265392)
-- Name: DataSharingRequestStatus PK_DataSharingRequestStatus; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequestStatus"
    ADD CONSTRAINT "PK_DataSharingRequestStatus" PRIMARY KEY ("Id");


--
-- TOC entry 3926 (class 2606 OID 265244)
-- Name: Farm PK_Farm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Farm"
    ADD CONSTRAINT "PK_Farm" PRIMARY KEY ("Id");


--
-- TOC entry 3985 (class 2606 OID 265690)
-- Name: FarmWeatherStation PK_FarmWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "PK_FarmWeatherStation" PRIMARY KEY ("FarmId", "WeatherStationId");


--
-- TOC entry 3933 (class 2606 OID 265269)
-- Name: Field PK_Field; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "PK_Field" PRIMARY KEY ("Id");


--
-- TOC entry 3999 (class 2606 OID 265774)
-- Name: FieldCrop PK_FieldCrop; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "PK_FieldCrop" PRIMARY KEY ("Id");


--
-- TOC entry 3958 (class 2606 OID 265598)
-- Name: FieldCropPest PK_FieldCropPest; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "PK_FieldCropPest" PRIMARY KEY ("Id");


--
-- TOC entry 3977 (class 2606 OID 265629)
-- Name: FieldCropPestDss PK_FieldCropPestDss; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "PK_FieldCropPestDss" PRIMARY KEY ("Id");


--
-- TOC entry 4011 (class 2606 OID 265850)
-- Name: FieldDssResult PK_FieldDssResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "PK_FieldDssResult" PRIMARY KEY ("Id");


--
-- TOC entry 3936 (class 2606 OID 265283)
-- Name: FieldObservation PK_FieldObservation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "PK_FieldObservation" PRIMARY KEY ("Id");


--
-- TOC entry 3988 (class 2606 OID 265721)
-- Name: FieldSprayApplication PK_FieldSprayApplication; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "PK_FieldSprayApplication" PRIMARY KEY ("Id");


--
-- TOC entry 4002 (class 2606 OID 274146)
-- Name: FieldWeatherDataSource PK_FieldWeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "PK_FieldWeatherDataSource" PRIMARY KEY ("FieldId");


--
-- TOC entry 4005 (class 2606 OID 265816)
-- Name: FieldWeatherStation PK_FieldWeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "PK_FieldWeatherStation" PRIMARY KEY ("FieldId", "WeatherStationId");


--
-- TOC entry 3960 (class 2606 OID 265542)
-- Name: ForecastAlert PK_ForecastAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastAlert"
    ADD CONSTRAINT "PK_ForecastAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3967 (class 2606 OID 265570)
-- Name: ForecastResult PK_ForecastResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "PK_ForecastResult" PRIMARY KEY ("Id");


--
-- TOC entry 3964 (class 2606 OID 265552)
-- Name: ObservationAlert PK_ObservationAlert; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "PK_ObservationAlert" PRIMARY KEY ("Id");


--
-- TOC entry 3970 (class 2606 OID 265583)
-- Name: ObservationResult PK_ObservationResult; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "PK_ObservationResult" PRIMARY KEY ("Id");


--
-- TOC entry 3921 (class 2606 OID 264209)
-- Name: UserAddress PK_UserAddress; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserAddress"
    ADD CONSTRAINT "PK_UserAddress" PRIMARY KEY ("Id");


--
-- TOC entry 3930 (class 2606 OID 265451)
-- Name: UserFarm PK_UserFarm; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "PK_UserFarm" PRIMARY KEY ("UserId", "FarmId");


--
-- TOC entry 3941 (class 2606 OID 265375)
-- Name: UserFarmType PK_UserFarmType; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarmType"
    ADD CONSTRAINT "PK_UserFarmType" PRIMARY KEY ("Id");


--
-- TOC entry 3919 (class 2606 OID 265449)
-- Name: UserProfile PK_UserProfile; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "PK_UserProfile" PRIMARY KEY ("UserId");


--
-- TOC entry 3996 (class 2606 OID 265756)
-- Name: UserWidget PK_UserWidget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "PK_UserWidget" PRIMARY KEY ("UserId", "WidgetId");


--
-- TOC entry 3980 (class 2606 OID 274144)
-- Name: WeatherDataSource PK_WeatherDataSource; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherDataSource"
    ADD CONSTRAINT "PK_WeatherDataSource" PRIMARY KEY ("Id");


--
-- TOC entry 3982 (class 2606 OID 265664)
-- Name: WeatherStation PK_WeatherStation; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherStation"
    ADD CONSTRAINT "PK_WeatherStation" PRIMARY KEY ("Id");


--
-- TOC entry 3993 (class 2606 OID 265746)
-- Name: Widget PK_Widget; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Widget"
    ADD CONSTRAINT "PK_Widget" PRIMARY KEY ("Id");


--
-- TOC entry 3916 (class 2606 OID 264191)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 3885 (class 1259 OID 264011)
-- Name: ix_hangfire_counter_expireat; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_counter_expireat ON hangfire.counter USING btree (expireat);


--
-- TOC entry 3886 (class 1259 OID 264171)
-- Name: ix_hangfire_counter_key; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_counter_key ON hangfire.counter USING btree (key);


--
-- TOC entry 3891 (class 1259 OID 264179)
-- Name: ix_hangfire_job_statename; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_job_statename ON hangfire.job USING btree (statename);


--
-- TOC entry 3910 (class 1259 OID 264184)
-- Name: ix_hangfire_jobparameter_jobidandname; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_jobparameter_jobidandname ON hangfire.jobparameter USING btree (jobid, name);


--
-- TOC entry 3897 (class 1259 OID 264140)
-- Name: ix_hangfire_jobqueue_jobidandqueue; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_jobqueue_jobidandqueue ON hangfire.jobqueue USING btree (jobid, queue);


--
-- TOC entry 3898 (class 1259 OID 264024)
-- Name: ix_hangfire_jobqueue_queueandfetchedat; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_jobqueue_queueandfetchedat ON hangfire.jobqueue USING btree (queue, fetchedat);


--
-- TOC entry 3894 (class 1259 OID 264089)
-- Name: ix_hangfire_state_jobid; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX ix_hangfire_state_jobid ON hangfire.state USING btree (jobid);


--
-- TOC entry 3901 (class 1259 OID 264185)
-- Name: jobqueue_queue_fetchat_jobid; Type: INDEX; Schema: hangfire; Owner: -
--

CREATE INDEX jobqueue_queue_fetchat_jobid ON hangfire.jobqueue USING btree (queue, fetchedat, jobid);


--
-- TOC entry 4006 (class 1259 OID 265842)
-- Name: IX_CropPestDssResult_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CropPestDssResult_CropPestDssId" ON public."CropPestDssResult" USING btree ("CropPestDssId");


--
-- TOC entry 3971 (class 1259 OID 265790)
-- Name: IX_CropPestDss_CropPestId_DssId_DssModelId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPestDss_CropPestId_DssId_DssModelId" ON public."CropPestDss" USING btree ("CropPestId", "DssId", "DssModelId");


--
-- TOC entry 3952 (class 1259 OID 265507)
-- Name: IX_CropPest_CropEppoCode_PestEppoCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CropPest_CropEppoCode_PestEppoCode" ON public."CropPest" USING btree ("CropEppoCode", "PestEppoCode");


--
-- TOC entry 3944 (class 1259 OID 265421)
-- Name: IX_DataSharingRequestStatus_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequestStatus_Description" ON public."DataSharingRequestStatus" USING btree ("Description");


--
-- TOC entry 3947 (class 1259 OID 265418)
-- Name: IX_DataSharingRequest_RequestStatusDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequestStatusDescription" ON public."DataSharingRequest" USING btree ("RequestStatusDescription");


--
-- TOC entry 3948 (class 1259 OID 265479)
-- Name: IX_DataSharingRequest_RequesteeId_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DataSharingRequest_RequesteeId_RequesterId" ON public."DataSharingRequest" USING btree ("RequesteeId", "RequesterId");


--
-- TOC entry 3949 (class 1259 OID 265420)
-- Name: IX_DataSharingRequest_RequesterId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DataSharingRequest_RequesterId" ON public."DataSharingRequest" USING btree ("RequesterId");


--
-- TOC entry 3983 (class 1259 OID 265702)
-- Name: IX_FarmWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FarmWeatherStation_WeatherStationId" ON public."FarmWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3924 (class 1259 OID 265712)
-- Name: IX_Farm_Location; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Farm_Location" ON public."Farm" USING btree ("Location");


--
-- TOC entry 3974 (class 1259 OID 265640)
-- Name: IX_FieldCropPestDss_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPestDss_CropPestDssId" ON public."FieldCropPestDss" USING btree ("CropPestDssId");


--
-- TOC entry 3975 (class 1259 OID 265641)
-- Name: IX_FieldCropPestDss_FieldCropPestId_CropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPestDss_FieldCropPestId_CropPestDssId" ON public."FieldCropPestDss" USING btree ("FieldCropPestId", "CropPestDssId");


--
-- TOC entry 3955 (class 1259 OID 265508)
-- Name: IX_FieldCropPest_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldCropPest_CropPestId" ON public."FieldCropPest" USING btree ("CropPestId");


--
-- TOC entry 3956 (class 1259 OID 265780)
-- Name: IX_FieldCropPest_FieldCropId_CropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCropPest_FieldCropId_CropPestId" ON public."FieldCropPest" USING btree ("FieldCropId", "CropPestId");


--
-- TOC entry 3997 (class 1259 OID 265787)
-- Name: IX_FieldCrop_FieldId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_FieldCrop_FieldId" ON public."FieldCrop" USING btree ("FieldId");


--
-- TOC entry 4009 (class 1259 OID 265856)
-- Name: IX_FieldDssResult_FieldCropPestDssId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldDssResult_FieldCropPestDssId" ON public."FieldDssResult" USING btree ("FieldCropPestDssId");


--
-- TOC entry 3934 (class 1259 OID 265643)
-- Name: IX_FieldObservation_FieldCropPestdId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldObservation_FieldCropPestdId" ON public."FieldObservation" USING btree ("FieldCropPestdId");


--
-- TOC entry 3986 (class 1259 OID 265733)
-- Name: IX_FieldSprayApplication_FieldCropPestId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldSprayApplication_FieldCropPestId" ON public."FieldSprayApplication" USING btree ("FieldCropPestId");


--
-- TOC entry 4000 (class 1259 OID 274148)
-- Name: IX_FieldWeatherDataSource_WeatherDataSourceId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherDataSource_WeatherDataSourceId" ON public."FieldWeatherDataSource" USING btree ("WeatherDataSourceId");


--
-- TOC entry 4003 (class 1259 OID 265828)
-- Name: IX_FieldWeatherStation_WeatherStationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_FieldWeatherStation_WeatherStationId" ON public."FieldWeatherStation" USING btree ("WeatherStationId");


--
-- TOC entry 3931 (class 1259 OID 265275)
-- Name: IX_Field_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Field_FarmId" ON public."Field" USING btree ("FarmId");


--
-- TOC entry 3965 (class 1259 OID 265591)
-- Name: IX_ForecastResult_ForecastAlertId_Date; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ForecastResult_ForecastAlertId_Date" ON public."ForecastResult" USING btree ("ForecastAlertId", "Date");


--
-- TOC entry 3961 (class 1259 OID 265593)
-- Name: IX_ObservationAlert_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationAlert_FieldObservationId" ON public."ObservationAlert" USING btree ("FieldObservationId");


--
-- TOC entry 3962 (class 1259 OID 265613)
-- Name: IX_ObservationAlert_WeatherStationId_FieldObservationId; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ObservationAlert_WeatherStationId_FieldObservationId" ON public."ObservationAlert" USING btree ("WeatherStationId", "FieldObservationId");


--
-- TOC entry 3968 (class 1259 OID 265595)
-- Name: IX_ObservationResult_ObservationAlertId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ObservationResult_ObservationAlertId" ON public."ObservationResult" USING btree ("ObservationAlertId");


--
-- TOC entry 3939 (class 1259 OID 265379)
-- Name: IX_UserFarmType_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_UserFarmType_Description" ON public."UserFarmType" USING btree ("Description");


--
-- TOC entry 3927 (class 1259 OID 265261)
-- Name: IX_UserFarm_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_FarmId" ON public."UserFarm" USING btree ("FarmId");


--
-- TOC entry 3928 (class 1259 OID 265378)
-- Name: IX_UserFarm_UserFarmTypeDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserFarm_UserFarmTypeDescription" ON public."UserFarm" USING btree ("UserFarmTypeDescription");


--
-- TOC entry 3917 (class 1259 OID 264210)
-- Name: IX_UserProfile_UserAddressId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserProfile_UserAddressId" ON public."UserProfile" USING btree ("UserAddressId");


--
-- TOC entry 3994 (class 1259 OID 265767)
-- Name: IX_UserWidget_WidgetDescription; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserWidget_WidgetDescription" ON public."UserWidget" USING btree ("WidgetDescription");


--
-- TOC entry 3978 (class 1259 OID 265993)
-- Name: IX_WeatherDataSource_FarmId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_WeatherDataSource_FarmId" ON public."WeatherDataSource" USING btree ("FarmId");


--
-- TOC entry 3991 (class 1259 OID 265768)
-- Name: IX_Widget_Description; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Widget_Description" ON public."Widget" USING btree ("Description");


--
-- TOC entry 4013 (class 2606 OID 264115)
-- Name: jobparameter jobparameter_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.jobparameter
    ADD CONSTRAINT jobparameter_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4012 (class 2606 OID 264090)
-- Name: state state_jobid_fkey; Type: FK CONSTRAINT; Schema: hangfire; Owner: -
--

ALTER TABLE ONLY hangfire.state
    ADD CONSTRAINT state_jobid_fkey FOREIGN KEY (jobid) REFERENCES hangfire.job(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4042 (class 2606 OID 265837)
-- Name: CropPestDssResult FK_CropPestDss_CropPestDssResult; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDssResult"
    ADD CONSTRAINT "FK_CropPestDss_CropPestDssResult" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4023 (class 2606 OID 265496)
-- Name: FieldCropPest FK_CropPest_Crop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_CropPest_Crop" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4028 (class 2606 OID 265608)
-- Name: CropPestDss FK_CropPest_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CropPestDss"
    ADD CONSTRAINT "FK_CropPest_CropPestDss" FOREIGN KEY ("CropPestId") REFERENCES public."CropPest"("Id");


--
-- TOC entry 4020 (class 2606 OID 265403)
-- Name: DataSharingRequest FK_DataSharingRequest_RequestStatus_RequestDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_RequestStatus_RequestDescription" FOREIGN KEY ("RequestStatusDescription") REFERENCES public."DataSharingRequestStatus"("Description");


--
-- TOC entry 4021 (class 2606 OID 265452)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesteeId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesteeId" FOREIGN KEY ("RequesteeId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4022 (class 2606 OID 265457)
-- Name: DataSharingRequest FK_DataSharingRequest_UserProfile_RequesterId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DataSharingRequest"
    ADD CONSTRAINT "FK_DataSharingRequest_UserProfile_RequesterId" FOREIGN KEY ("RequesterId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4031 (class 2606 OID 265994)
-- Name: WeatherDataSource FK_FarmWeatherDataSource_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WeatherDataSource"
    ADD CONSTRAINT "FK_FarmWeatherDataSource_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4032 (class 2606 OID 265691)
-- Name: FarmWeatherStation FK_FarmWeatherStation_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4033 (class 2606 OID 265696)
-- Name: FarmWeatherStation FK_FarmWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FarmWeatherStation"
    ADD CONSTRAINT "FK_FarmWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 4029 (class 2606 OID 265630)
-- Name: FieldCropPestDss FK_FieldCropPestDss_CropPestDss; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_CropPestDss" FOREIGN KEY ("CropPestDssId") REFERENCES public."CropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4030 (class 2606 OID 265635)
-- Name: FieldCropPestDss FK_FieldCropPestDss_FieldCropPest_FieldCropPestId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPestDss"
    ADD CONSTRAINT "FK_FieldCropPestDss_FieldCropPest_FieldCropPestId" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4024 (class 2606 OID 265782)
-- Name: FieldCropPest FK_FieldCropPest_FieldCrop_FieldCropId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCropPest"
    ADD CONSTRAINT "FK_FieldCropPest_FieldCrop_FieldCropId" FOREIGN KEY ("FieldCropId") REFERENCES public."FieldCrop"("Id") ON DELETE CASCADE;


--
-- TOC entry 4043 (class 2606 OID 265851)
-- Name: FieldDssResult FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldDssResult"
    ADD CONSTRAINT "FK_FieldDssResult_FieldCropPestDss_FieldCropPestDssId" FOREIGN KEY ("FieldCropPestDssId") REFERENCES public."FieldCropPestDss"("Id") ON DELETE CASCADE;


--
-- TOC entry 4039 (class 2606 OID 265799)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 4038 (class 2606 OID 274149)
-- Name: FieldWeatherDataSource FK_FieldWeatherDataSource_WeatherDataSource; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherDataSource"
    ADD CONSTRAINT "FK_FieldWeatherDataSource_WeatherDataSource" FOREIGN KEY ("WeatherDataSourceId") REFERENCES public."WeatherDataSource"("Id") ON DELETE CASCADE;


--
-- TOC entry 4040 (class 2606 OID 265817)
-- Name: FieldWeatherStation FK_FieldWeatherStation_Field; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_Field" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 4041 (class 2606 OID 265822)
-- Name: FieldWeatherStation FK_FieldWeatherStation_WeatherStation; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldWeatherStation"
    ADD CONSTRAINT "FK_FieldWeatherStation_WeatherStation" FOREIGN KEY ("WeatherStationId") REFERENCES public."WeatherStation"("Id") ON DELETE CASCADE;


--
-- TOC entry 4018 (class 2606 OID 265270)
-- Name: Field FK_Field_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Field"
    ADD CONSTRAINT "FK_Field_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4037 (class 2606 OID 265775)
-- Name: FieldCrop FK_Field_FieldCrop; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldCrop"
    ADD CONSTRAINT "FK_Field_FieldCrop" FOREIGN KEY ("FieldId") REFERENCES public."Field"("Id") ON DELETE CASCADE;


--
-- TOC entry 4026 (class 2606 OID 265615)
-- Name: ForecastResult FK_ForecastResult_ForecastAlert_ForecastAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ForecastResult"
    ADD CONSTRAINT "FK_ForecastResult_ForecastAlert_ForecastAlertId" FOREIGN KEY ("ForecastAlertId") REFERENCES public."ForecastAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 4025 (class 2606 OID 265558)
-- Name: ObservationAlert FK_ObservationAlert_FieldObservation_FieldObservationId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationAlert"
    ADD CONSTRAINT "FK_ObservationAlert_FieldObservation_FieldObservationId" FOREIGN KEY ("FieldObservationId") REFERENCES public."FieldObservation"("Id");


--
-- TOC entry 4027 (class 2606 OID 265620)
-- Name: ObservationResult FK_ObservationResult_ObservationAlert_ObservationAlertId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ObservationResult"
    ADD CONSTRAINT "FK_ObservationResult_ObservationAlert_ObservationAlertId" FOREIGN KEY ("ObservationAlertId") REFERENCES public."ObservationAlert"("Id") ON DELETE CASCADE;


--
-- TOC entry 4019 (class 2606 OID 265644)
-- Name: FieldObservation FK_Observation_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldObservation"
    ADD CONSTRAINT "FK_Observation_FieldCropPest" FOREIGN KEY ("FieldCropPestdId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4034 (class 2606 OID 265734)
-- Name: FieldSprayApplication FK_Spray_FieldCropPest; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FieldSprayApplication"
    ADD CONSTRAINT "FK_Spray_FieldCropPest" FOREIGN KEY ("FieldCropPestId") REFERENCES public."FieldCropPest"("Id") ON DELETE CASCADE;


--
-- TOC entry 4016 (class 2606 OID 265467)
-- Name: UserFarm FK_UserFarm_Farm; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_Farm" FOREIGN KEY ("FarmId") REFERENCES public."Farm"("Id") ON DELETE CASCADE;


--
-- TOC entry 4017 (class 2606 OID 265472)
-- Name: UserFarm FK_UserFarm_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4015 (class 2606 OID 265380)
-- Name: UserFarm FK_UserFarm_UserFarmType_UserFarmTypeDescription; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserFarm"
    ADD CONSTRAINT "FK_UserFarm_UserFarmType_UserFarmTypeDescription" FOREIGN KEY ("UserFarmTypeDescription") REFERENCES public."UserFarmType"("Description");


--
-- TOC entry 4035 (class 2606 OID 265757)
-- Name: UserWidget FK_UserWidget_User; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_User" FOREIGN KEY ("UserId") REFERENCES public."UserProfile"("UserId") ON DELETE CASCADE;


--
-- TOC entry 4036 (class 2606 OID 265762)
-- Name: UserWidget FK_UserWidget_Widget; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserWidget"
    ADD CONSTRAINT "FK_UserWidget_Widget" FOREIGN KEY ("WidgetDescription") REFERENCES public."Widget"("Description") ON DELETE CASCADE;


--
-- TOC entry 4014 (class 2606 OID 265230)
-- Name: UserProfile FK_User_UserAddress; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "FK_User_UserAddress" FOREIGN KEY ("UserAddressId") REFERENCES public."UserAddress"("Id") ON DELETE CASCADE;


-- Completed on 2021-03-15 14:36:39 UTC

--
-- PostgreSQL database dump complete
--

