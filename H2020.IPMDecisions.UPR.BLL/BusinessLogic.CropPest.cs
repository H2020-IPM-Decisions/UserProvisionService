using System;
using System.Collections.Generic;
using System.Linq;
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
        private FieldCropDto ShapeFieldCropWithChildren(Field field, FieldCropPestResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var fieldCropToReturn = this.mapper
                    .Map<FieldCropDto>(field.FieldCrop);

                fieldCropToReturn.FieldCropPestWithChildrenDto = ShapeFieldCropPestAsChildren(field.FieldCrop, resourceParameter, includeLinks);

                return fieldCropToReturn;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ShapeFieldCropPestAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }

        private ShapedDataWithLinks ShapeFieldCropPestAsChildren(FieldCrop fieldCrop, FieldCropPestResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<FieldCropPest>.Create(
                    fieldCrop.FieldCropPests.AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var shapedChildrenToReturn = this.mapper
                    .Map<IEnumerable<FieldCropPestWithChildrenDto>>(childrenAsPaged)
                    .ShapeData(resourceParameter.Fields);

                return new ShapedDataWithLinks()
                {
                    Value = shapedChildrenToReturn,
                    Links = null,
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