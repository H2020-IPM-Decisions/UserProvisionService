using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        #region Helpers
        private FieldCropDto ShapeFieldCropWithChildren(
            Field field,
            FieldResourceParameter resourceParameter,
            bool includeLinks,
            List<EppoCode> eppoCodes)
        {
            try
            {
                var fieldCropToReturn = this.mapper
                    .Map<FieldCropDto>(field.FieldCrop);

                fieldCropToReturn.CropLanguages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodes, "crop", fieldCropToReturn.CropEppoCode);
                var fieldCropPestResourceParameter = this.mapper.Map<FieldCropPestResourceParameter>(resourceParameter);
                fieldCropToReturn.FieldCropPestDto = ShapeFieldCropPestAsChildren(field.FieldCrop, fieldCropPestResourceParameter, includeLinks, eppoCodes, fieldCropToReturn.CropLanguages).Result;

                return fieldCropToReturn;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ShapeFieldCropPestAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }

        private async Task<ShapedDataWithLinks> ShapeFieldCropPestAsChildren(
            FieldCrop fieldCrop,
            FieldCropPestResourceParameter resourceParameter,
            bool includeLinks,
            List<EppoCode> eppoCodes,
            IDictionary<string, string> cropLanguages)
        {
            try
            {
                var childrenAsPaged = PagedList<FieldCropPest>.Create(
                    fieldCrop.FieldCropPests.AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);
                var links = UrlCreatorHelper.CreateLinksForFieldCropPests(
                    this.url,
                    fieldCrop.FieldId,
                    resourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var shapedChildrenAsDto = this.mapper
                    .Map<IEnumerable<FieldCropPestWithChildrenDto>>(childrenAsPaged);
                foreach (var fieldCropPestDto in shapedChildrenAsDto)
                {
                    fieldCropPestDto.PestLanguages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodes, "pest", fieldCropPestDto.PestEppoCode);
                    fieldCropPestDto.CropPestDto.PestLanguages = fieldCropPestDto.PestLanguages;
                    fieldCropPestDto.CropPestDto.CropLanguages = cropLanguages;
                }

                var shapedChildrenToReturn = shapedChildrenAsDto
                    .ShapeData(resourceParameter.Fields) as IEnumerable<IDictionary<string, object>>;

                var listOfDssWithInformation = await this.internalCommunicationProvider.GetAllListOfDssFromDssMicroservice();
                foreach (var shapedChildren in shapedChildrenToReturn)
                {
                    var fieldCropPestId = Guid.Parse(shapedChildren["Id"].ToString());

                    var fieldObservationResourceParameter = this.mapper.Map<FieldObservationResourceParameter>(resourceParameter);
                    shapedChildren.Add("FieldObservationDto", ShapeFieldObservationsAsChildren(
                                                        fieldCrop, fieldCropPestId, fieldObservationResourceParameter, includeLinks));

                    var fieldSprayResourceParameter = this.mapper.Map<FieldSprayResourceParameter>(resourceParameter);
                    shapedChildren.Add("FieldSprayApplicationDto", ShapeFieldSpraysAsChildren(
                                                        fieldCrop, fieldCropPestId, fieldSprayResourceParameter, includeLinks));

                    var fieldCropPestDssResourceParameter = this.mapper.Map<FieldCropPestDssResourceParameter>(resourceParameter);
                    shapedChildren.Add("FieldCropPestDssDto", ShapeFieldCropPestDssAsChildren(
                                                    fieldCrop, fieldCropPestId, fieldCropPestDssResourceParameter, includeLinks, listOfDssWithInformation));
                }

                return new ShapedDataWithLinks()
                {
                    Value = shapedChildrenToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaDataChildren
                };
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ShapeFieldCropPestAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }
        #endregion
    }
}