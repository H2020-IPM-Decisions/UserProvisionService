using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewField(
            FieldForCreationDto fieldForCreationDto,
            HttpContext httpContext,
            string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(fieldForCreationDto);
                fieldAsEntity.Farm = farm;
                this.dataService.Fields.Create(fieldAsEntity);

                List<FieldCropPest> listOfCropPest = await CreateCropListForInsertion(fieldForCreationDto.CropPests, fieldAsEntity);
                fieldAsEntity.FieldCropPests = listOfCropPest;

                await this.dataService.CompleteAsync();

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                bool includeChildren = parsedMediaType.SubTypeWithoutSuffix.ToString().Contains("withchildren");

                IEnumerable<LinkDto> links = new List<LinkDto>();
                if (includeLinks)
                {
                    links = UrlCreatorHelper.CreateLinksForField(url, fieldAsEntity.Id, fieldAsEntity.FarmId);
                }

                var fieldToReturn = this.mapper
                    .Map<FieldDto>(fieldAsEntity)
                    .ShapeData() as IDictionary<string, object>;
                if (includeLinks)
                {
                    fieldToReturn.Add("links", links);
                }
                return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewField. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
        
        public async Task<GenericResponse<FieldDto>> AddNewField(FieldForCreationDto fieldForCreationDto, HttpContext httpContext, Guid id)
        {
            try
            {
                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(fieldForCreationDto);
                fieldAsEntity.Farm = farm;
                fieldAsEntity.Id = id;

                this.dataService.Fields.Create(fieldAsEntity);
                List<FieldCropPest> listOfCropPest = await CreateCropListForInsertion(fieldForCreationDto.CropPests, fieldAsEntity);
                fieldAsEntity.FieldCropPests = listOfCropPest;

                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldDto>(fieldAsEntity);
                return GenericResponseBuilder.Success<FieldDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewField. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteField(Guid id)
        {
            try
            {
                var existingField = await this
                            .dataService
                            .Fields
                            .FindByIdAsync(id);

                if (existingField == null) return GenericResponseBuilder.Success();

                this.dataService.Fields.Delete(existingField);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteField. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFields(
            Guid farmId,
            FieldResourceParameter resourceParameter,
            string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldWithChildrenDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                if (!propertyMappingService.ValidMappingExistsFor<FieldDto, Field>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                bool includeChildren = parsedMediaType.SubTypeWithoutSuffix.ToString().Contains("withchildren");

                var fieldsAsEntities = await this
                    .dataService
                    .Fields
                    .FindAllAsync(resourceParameter, farmId, includeChildren);
                if (fieldsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(fieldsAsEntities);

                var links = UrlCreatorHelper.CreateLinksForFields(
                    this.url,
                    farmId,
                    resourceParameter,
                    fieldsAsEntities.HasNext,
                    fieldsAsEntities.HasPrevious);

                if (includeChildren)
                {
                    var fieldsAsDictionaryList = fieldsAsEntities.Select(field =>
                    {
                        return CreateFieldWithChildrenAsDictionary(
                            resourceParameter,
                            includeLinks,
                            field,
                            links);
                    });

                    var farmsToReturnWitChildren = new ShapedDataWithLinks()
                    {
                        Value = fieldsAsDictionaryList,
                        Links = links,
                        PaginationMetaData = paginationMetaData
                    };
                    return GenericResponseBuilder.Success<ShapedDataWithLinks>(farmsToReturnWitChildren);                 
                }

                var shapedFarmsToReturn = this.mapper
                    .Map<IEnumerable<FieldDto>>(fieldsAsEntities)
                    .ShapeData(resourceParameter.Fields);

                var farmsToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedFarmsToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(farmsToReturn);

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFields. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<Field>> GetField(Guid id)
        {
            try
            {
                var fieldAsEntity = await this
                    .dataService
                    .Fields
                    .FindByIdAsync(id);

                if (fieldAsEntity == null) return GenericResponseBuilder.Success<Field>(null);

                return GenericResponseBuilder.Success<Field>(fieldAsEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetField. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<Field>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetFieldDto(Guid id, FieldResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                        out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldWithChildrenDto>(resourceParameter.Fields, false))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                   .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                bool includeChildren = parsedMediaType.SubTypeWithoutSuffix.ToString().Contains("withchildren");

                var fieldAsEntity = await this
                    .dataService
                    .Fields
                    .FindByIdAsync(id, includeChildren);
                if (fieldAsEntity == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                IEnumerable<LinkDto> links = new List<LinkDto>();
                if (includeLinks)
                {
                    links = UrlCreatorHelper.CreateLinksForField(url, id, fieldAsEntity.FarmId, resourceParameter.Fields);
                }

                if (includeChildren)
                {
                    var fieldToReturnWithChildrenShaped = CreateFieldWithChildrenAsDictionary(
                       resourceParameter,
                       includeLinks,
                       fieldAsEntity,
                       links);

                    return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldToReturnWithChildrenShaped);
                }

                var fieldToReturn = this.mapper
                    .Map<FieldDto>(fieldAsEntity)
                    .ShapeData(resourceParameter.Fields) as IDictionary<string, object>;
                if (includeLinks)
                {
                    fieldToReturn.Add("links", links);
                }
                return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldDto. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateField(Field field, FieldForUpdateDto fieldToPatch)
        {
            try
            {
                this.mapper.Map(fieldToPatch, field);

                this.dataService.Fields.Update(field);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateField. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        public FieldForCreationDto MapToFieldForCreation(FieldForUpdateDto fieldForUpdateDto)
        {
            return this.mapper.Map<FieldForCreationDto>(fieldForUpdateDto);
        }

        public FieldForUpdateDto MapToFieldForUpdateDto(Field field)
        {
            return this.mapper.Map<FieldForUpdateDto>(field);
        }

        private ShapedDataWithLinks ShapeFieldsAsChildren(Farm farm, FieldResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<Field>.Create(
                    farm.Fields.AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                var childrenPaginationLinks = UrlCreatorHelper.CreateLinksForFields(
                    this.url,
                    farm.Id,
                    resourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var shapedChildrenToReturn = this.mapper
                    .Map<IEnumerable<FieldDto>>(childrenAsPaged)
                    .ShapeData(resourceParameter.Fields);

                var shapedChildrenToReturnWithLinks = shapedChildrenToReturn.Select(field =>
                {
                    var fieldAsDictionary = field as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = UrlCreatorHelper.CreateLinksForField(
                            this.url,
                            (Guid)fieldAsDictionary["Id"],
                            farm.Id,
                            resourceParameter.Fields);
                        fieldAsDictionary.Add("links", userLinks);
                    }
                    return fieldAsDictionary;
                });

                return new ShapedDataWithLinks()
                {
                    Value = shapedChildrenToReturnWithLinks,
                    Links = childrenPaginationLinks,
                    PaginationMetaData = paginationMetaDataChildren
                };
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ShapeFieldsAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }

        private async Task<List<FieldCropPest>> CreateCropListForInsertion(ICollection<CropPestForCreationDto> cropPestsFromRequest, Field field)
        {
            if (cropPestsFromRequest is null) return null;

            var listOfCropPest = new List<FieldCropPest>();           
            var cropWithoutDuplicates = cropPestsFromRequest
                                    .Select(c => new 
                                        { 
                                            CropEppoCode = c.CropEppoCode, 
                                            PestEppoCode = c.PestEppoCode 
                                        })
                                    .Distinct();
            
            foreach (var cropPest in cropWithoutDuplicates)
            {
                var cropPestAsEntity = await this.dataService.CropPests
                    .FindByConditionAsync
                    (c => c.CropEppoCode == cropPest.CropEppoCode
                    && c.PestEppoCode == cropPest.PestEppoCode);

                if (cropPestAsEntity == null)
                {
                    cropPestAsEntity = new CropPest()
                    {
                        CropEppoCode = cropPest.CropEppoCode,
                        PestEppoCode = cropPest.PestEppoCode,
                    };
                    this.dataService.CropPests.Create(cropPestAsEntity);
                }

                var newFieldCropPest = new FieldCropPest()
                {
                    CropPest = cropPestAsEntity,
                    Field = field
                };
                listOfCropPest.Add(newFieldCropPest);
            }

            return listOfCropPest;
        }

        private IDictionary<string, object> CreateFieldWithChildrenAsDictionary(
            FieldResourceParameter resourceParameter,
            bool includeLinks,
            Field fieldAsEntity,
            IEnumerable<LinkDto> links)
        {
            try
            {
                ShapedDataWithLinks fieldObservationsToReturn = null;
                // if (fieldAsEntity.FieldObservations != null && fieldAsEntity.FieldObservations.Count > 0)
                // {
                //     var fieldObservationResourceParameter = this.mapper.Map<FieldObservationResourceParameter>(resourceParameter);
                //     fieldObservationsToReturn = ShapeFieldObservationsAsChildren(
                //                     fieldAsEntity,
                //                     fieldObservationResourceParameter,
                //                     includeLinks);
                // }
                
                ShapedDataWithLinks fieldCropPestToReturn = null;
                if (fieldAsEntity.FieldCropPests != null && fieldAsEntity.FieldCropPests.Count > 0)
                {
                    var fieldCropPestResourceParameter = this.mapper.Map<FieldCropPestResourceParameter>(resourceParameter);
                    fieldCropPestToReturn = ShapeFieldCropPestAsChildren(
                                    fieldAsEntity,
                                    fieldCropPestResourceParameter,
                                    includeLinks);
                }

                var fieldToReturnWithChildren = this.mapper.Map<FieldWithChildrenDto>(fieldAsEntity);
                fieldToReturnWithChildren.FieldObservationsDto = fieldObservationsToReturn;
                fieldToReturnWithChildren.FieldCropPestsDto = fieldCropPestToReturn;

                var fieldToReturnWithChildrenShaped = fieldToReturnWithChildren
                    .ShapeData(resourceParameter.Fields) 
                    as IDictionary<string, object>;

                if (includeLinks)
                {
                    fieldToReturnWithChildrenShaped.Add("links", links);
                }
                return fieldToReturnWithChildrenShaped;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateFarmWithChildrenAsDictionary. {0}", ex.Message), ex);
                return null;
            }
        }
        #endregion
    }
}