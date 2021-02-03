using H2020.IPMDecisions.UPR.Core.Models;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.UPR.Core.PatchOperationExamples
{
    public class JsonPatchFieldRequestExample : IExamplesProvider<Operation[]>
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation
                {
                    Op = "replace",
                    Path = "/Name",
                    Value = "New Field Name"
                },
                new Operation
                {
                    Op = "replace",
                    Path = "/fieldCropPest",
                    Value = @"[
                        // No ID will create a new one
                        {""pestEppoCode"" :""NEW""},
                        // ID and same pest EPPO code, do nothing
                        { ""id"": ""95211737-7c56-4f59-b899-3681bc5277f4"",""pestEppoCode"" : ""STAY""},
                        // ID exist on DB but new pest EPPO code, it will be deleted and recreate new one with new EPPO code
                        {""id"": ""3cad7eb1-27fe-4c92-8ba9-18734681b271"",""pestEppoCode"" : ""NEW""}
                        // Exist on DB, but it will be deleted because is not on payload
                        //{//""id"": ""e2303a29-1615-4da0-94d5-30896b8fca05"",//""pestEppoCode"" : ""DELETE""//}]"
                }
            };
        }
    }
}