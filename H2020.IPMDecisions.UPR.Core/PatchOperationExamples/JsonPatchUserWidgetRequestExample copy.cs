using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.UPR.Core.PatchOperationExamples
{
    public class JsonPatchUserProfileRequestExample : IExamplesProvider<Operation[]>
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation("add","/country","", "Spain"),
                new Operation("replace","/firstName","", "Name"),
                new Operation("remove","/street","", ""),
            };
        }
    }
}