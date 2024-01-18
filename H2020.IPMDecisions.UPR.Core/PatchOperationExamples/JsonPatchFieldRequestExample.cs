using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.UPR.Core.PatchOperationExamples
{
    public class JsonPatchFieldRequestExample : IExamplesProvider<Operation[]>
    {
        public Operation[] GetExamples()
        {
            return new[]
            {
                new Operation("replace","/Name","", "New Name"),
                new Operation("add","/fieldCropDto/fieldCropPestDto","","NEW"),
                new Operation("replace","/fieldCropDto/fieldCropPestDto/{fieldCropPestId}","","OTHER"),
                new Operation("remove","/fieldCropDto/fieldCropPestDto/{fieldCropPestId}",""),
            };
        }
    }
}