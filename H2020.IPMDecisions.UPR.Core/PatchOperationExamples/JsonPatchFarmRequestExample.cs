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
                new Operation("replace","/name","", "New Farm Name"),
                new Operation("replace","/weatherHistoricalDto/WeatherId","","NewId"),
                new Operation("replace","/weatherHistoricalDto/url","","newurl"),
                new Operation("replace","/weatherHistoricalDto/name","","new name"),
                new Operation("replace","/weatherForecastDto/WeatherId","","OTHER"),
                new Operation("replace","/weatherForecastDto/name","","OTHER"),
                new Operation("replace","/weatherForecastDto/name","","OTHER"),
                new Operation("replace","/location/x","","5"),
            };
        }
    }
}