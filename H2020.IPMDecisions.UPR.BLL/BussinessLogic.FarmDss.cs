using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public async Task<GenericResponse> AddNewFarmDss(FarmDssForCreationDto farmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(farmDssDto);
                fieldAsEntity.Farm = farm;

                this.dataService.Fields.Create(fieldAsEntity);

                var cropPestAsCollection = new Collection<CropPestForCreationDto>();
                cropPestAsCollection.Add(farmDssDto.CropPest);

                List<FieldCropPest> fieldCropPests = await CreateCropListForInsertion(cropPestAsCollection, fieldAsEntity);
                fieldAsEntity.FieldCropPests = fieldCropPests;

                var newFieldCropPestDss = await CreateFieldCropPestDss(fieldCropPests.FirstOrDefault(), farmDssDto.DssId);
                



                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFarmDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}