using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.UPR.Core.PatchOperationExamples
{
    public class JsonPatchUserWidgetRequestExample : IExamplesProvider<Operation[]>
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation("replace","/maps/allowed","", "false"),
                new Operation("replace","/weather/allowed","", "true"),
                new Operation("replace","/actions/allowed","", "false"),
                new Operation("replace","/riskforecast/allowed","", "true"),
            };
        }
    }
}