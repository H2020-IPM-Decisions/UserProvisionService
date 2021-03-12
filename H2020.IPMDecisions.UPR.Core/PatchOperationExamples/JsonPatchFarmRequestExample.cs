using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.UPR.Core.PatchOperationExamples
{
    public class JsonPatchFarmRequestExample : IExamplesProvider<Operation[]>
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation("replace","/name","", "New Weather Data Source"),
                new Operation("remove","/weatherDataSourceDto/credentials","",""),
                new Operation("replace","/weatherDataSourceDto/url","","newurl"),
                new Operation("replace","/weatherDataSourceDto/isforecast","","true"),
                new Operation("add","/weatherStationDto/id","","OTHER"),
                new Operation("replace","/location/x","","5"),
            };
        }
    }
}