namespace H2020.IPMDecisions.UPR.Data.Persistence.SqlQueries
{
    public static class RawSqlQueries
    {
        public const string GetDssResults = @"SELECT ""Field"".""FarmId"", ""Field"".""Id"" as ""FieldId"", ""FieldCropPestDss"".""Id"",fc.""CropEppoCode"", ""CropPest"".""PestEppoCode"",
        ""CropPestDss"".""DssId"",""CropPestDss"".""DssModelId"",""CropPestDss"".""DssExecutionType"", ""CropPestDss"".""DssModelName"", ""CropPestDss"".""DssName"",
        dssResults.""CreationDate"", dssResults.""DssFullResult"", dssResults.""WarningStatus"", dssResults.""WarningMessage"",
        dssResults.""IsValid""
                FROM ""FieldCrop"" fc
                INNER JOIN ""Field"" ON ""Field"".""Id"" = fc.""FieldId""
                INNER JOIN ""FieldCropPest"" ON ""FieldCropPest"".""FieldCropId"" = fc.""Id""
                LEFT JOIN ""CropPest"" ON ""CropPest"".""Id"" = ""FieldCropPest"".""CropPestId""
                INNER JOIN ""FieldCropPestDss"" ON ""FieldCropPestDss"".""FieldCropPestId"" = ""FieldCropPest"".""Id"" 
                LEFT JOIN ""CropPestDss"" ON ""CropPestDss"".""Id"" = ""FieldCropPestDss"".""CropPestDssId""
                INNER JOIN(SELECT fdr.""FieldCropPestDssId"" as dssResultFCPI, *
                    FROM ""FieldCropPestDss"" fcpd
                    INNER JOIN(SELECT ""FieldCropPestDssId"", MAX(""CreationDate"") LastResult
                    FROM ""FieldDssResult""
                    GROUP BY ""FieldCropPestDssId"") LatestResults
                    ON fcpd.""Id"" = LatestResults.""FieldCropPestDssId"" 
                    INNER JOIN ""FieldDssResult"" fdr ON(LatestResults.""FieldCropPestDssId"" = fdr.""FieldCropPestDssId""
                    AND LatestResults.LastResult = fdr.""CreationDate"")) dssResults
                ON dssResults.dssResultFCPI = ""FieldCropPestDss"".""Id""
                WHERE fc.""FieldId"" IN(
                    SELECT ""Id""
                    FROM ""Field"" WHERE ""FarmId"" IN
                    (SELECT ""FarmId""
                    FROM ""Farm""
                    INNER JOIN ""UserFarm"" ON ""UserFarm"".""FarmId"" = ""Farm"".""Id""
                WHERE ""UserFarm"".""UserId"" = {0} AND ""UserFarm"".""Authorised"" = true));";
    }
}