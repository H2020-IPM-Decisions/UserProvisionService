--
-- INSERT DATA
--


INSERT INTO public."UserFarmType" ("Id", "Description") VALUES
    (0,	'Unknown'),
    (1,	'Owner'),
    (3,	'Advisor');


INSERT INTO public."DataSharingRequestStatus" ("Id", "Description") VALUES
    (0,	'Pending'),
    (1,	'Accepted'),
    (3,	'Declined');


INSERT INTO public."Widget" ("Id", "Description") VALUES
    (0,	'Maps'),
    (1,	'Actions'),
    (2,	'RiskForecast'),
    (3,	'Weather');