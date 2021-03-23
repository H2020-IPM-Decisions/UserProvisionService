using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FarmDssDto>> AddNewFarmDss(FarmDssForCreationDto farmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FarmDssDto>(null, "Wrong media type.");

                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(farmDssDto);
                fieldAsEntity.Farm = farm;
                this.dataService.Fields.Create(fieldAsEntity);

                await AddCropPestToField(farmDssDto.CropPest, fieldAsEntity);

                var cropPestDss = this.mapper.Map<CropPestDss>(farmDssDto);
                var newFieldCropPestDss = await CreateFieldCropPestDss(
                    fieldAsEntity.FieldCrop.FieldCropPests.FirstOrDefault(),
                    cropPestDss,
                    farmDssDto.DssParameters);
                await this.dataService.CompleteAsync();
                var farmToReturn = this.mapper.Map<FarmDssDto>(farm);

                if (farmDssDto.DssExecutionType.ToLower() == "onthefly")
                {
                    this.queueJobs.AddDssOnOnTheFlyQueue(newFieldCropPestDss.Id);
                    return GenericResponseBuilder.Accepted<FarmDssDto>(farmToReturn);
                }
                return GenericResponseBuilder.Success<FarmDssDto>(farmToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFarmDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FarmDssDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}