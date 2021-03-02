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
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

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

                await AddCropPestToField(fieldForCreationDto.CropPest, fieldAsEntity);
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
                await AddCropPestToField(fieldForCreationDto.CropPest, fieldAsEntity);

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

        public async Task<GenericResponse<Field>> GetField(Guid id, HttpContext httpContext)
        {
            try
            {
                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = await this
                      .dataService
                      .Fields
                      .FindByConditionAsync(f => f.Id == id & f.FarmId == farm.Id, true);

                if (fieldAsEntity == null) return GenericResponseBuilder.NotFound<Field>();

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

        public async Task<GenericResponse> UpdateField(Field field, FieldForUpdateDto fieldToPatch, JsonPatchDocument<FieldForUpdateDto> patchDocument)
        {
            try
            {
                this.mapper.Map(fieldToPatch, field);
                await UpdateFieldCropPestByOperations(field, patchDocument);

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

        private async Task AddCropPestToField(CropPestForCreationDto cropPestRequest, Field field)
        {
            if (cropPestRequest is null) return;

            var fieldCrop = new FieldCrop()
            {
                CropEppoCode = cropPestRequest.CropEppoCode,
                Field = field
            };
            var cropPestAsEntity = await this.dataService.CropPests
                .FindByConditionAsync
                (c => c.CropEppoCode.ToUpper().Equals(cropPestRequest.CropEppoCode.ToUpper())
                 && c.PestEppoCode.ToUpper().Equals(cropPestRequest.PestEppoCode.ToUpper()));

            if (cropPestAsEntity == null)
            {
                cropPestAsEntity = new CropPest()
                {
                    CropEppoCode = cropPestRequest.CropEppoCode.ToUpper(),
                    PestEppoCode = cropPestRequest.PestEppoCode.ToUpper(),
                };
                this.dataService.CropPests.Create(cropPestAsEntity);
            }
            var newFieldCropPest = new FieldCropPest()
            {
                CropPest = cropPestAsEntity,
                FieldCrop = fieldCrop
            };

            var fieldCropPests = new List<FieldCropPest>();
            fieldCropPests.Add(newFieldCropPest);

            field.FieldCrop = fieldCrop;
            field.FieldCrop.FieldCropPests = fieldCropPests;
        }

        private async Task AddPestsToField(string pestEppoCode, Field field)
        {
            if (string.IsNullOrEmpty(pestEppoCode)) return;
            var cropPestAsEntity = await this.dataService.CropPests
                .FindByConditionAsync
                (c => c.CropEppoCode == field.FieldCrop.CropEppoCode
                && c.PestEppoCode == pestEppoCode);

            if (cropPestAsEntity == null)
            {
                cropPestAsEntity = new CropPest()
                {
                    CropEppoCode = field.FieldCrop.CropEppoCode,
                    PestEppoCode = pestEppoCode,
                };
                this.dataService.CropPests.Create(cropPestAsEntity);
            }

            var newFieldCropPest = new FieldCropPest()
            {
                CropPest = cropPestAsEntity,
                FieldCrop = field.FieldCrop
            };
            field.FieldCrop.FieldCropPests.Add(newFieldCropPest);
        }

        private async Task ReplaceCropPestFromField(string pestEppoCode, FieldCropPest fieldCropPestToReplace)
        {
            if (string.IsNullOrEmpty(pestEppoCode)) return;
            var newCropPestAsEntity = await this.dataService
                .CropPests
                .FindByConditionAsync
                (c => c.CropEppoCode == fieldCropPestToReplace.FieldCrop.CropEppoCode
                && c.PestEppoCode == pestEppoCode);

            if (newCropPestAsEntity == null)
            {
                newCropPestAsEntity = new CropPest()
                {
                    CropEppoCode = fieldCropPestToReplace.FieldCrop.CropEppoCode,
                    PestEppoCode = pestEppoCode,
                };
                this.dataService.CropPests.Create(newCropPestAsEntity);
            }

            // We need to remove all associated data... and recreate object because
            // UI wants to keep same UI.
            this.dataService.FieldCropPests.Delete(fieldCropPestToReplace);
            var newFieldCropPest = new FieldCropPest()
            {
                Id = fieldCropPestToReplace.Id,
                FieldCrop = fieldCropPestToReplace.FieldCrop,
                CropPest = newCropPestAsEntity
            };
            this.dataService.FieldCropPests.Create(newFieldCropPest);
        }

        private IDictionary<string, object> CreateFieldWithChildrenAsDictionary(
            FieldResourceParameter resourceParameter,
            bool includeLinks,
            Field fieldAsEntity,
            IEnumerable<LinkDto> links)
        {
            try
            {
                FieldCropDto fieldCropToReturn = null;
                if (fieldAsEntity.FieldCrop != null)
                {
                    fieldCropToReturn = ShapeFieldCropWithChildren(
                                    fieldAsEntity,
                                    resourceParameter,
                                    includeLinks);
                }

                var fieldToReturnWithChildren = this.mapper.Map<FieldWithChildrenDto>(fieldAsEntity);
                fieldToReturnWithChildren.FieldCropDto = fieldCropToReturn;
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

        private async Task UpdateFieldCropPest(Field field, JsonPatchDocument<FieldForUpdateDto> patchDocument)
        {
            var fieldCropPestProperty = nameof(FieldForUpdateDto.FieldCropPestDto);
            if (patchDocument.Operations.Any(o => o.path.ToLower().Contains(fieldCropPestProperty.ToLower())))
            {
                var fieldCropPestOperations = patchDocument.Operations.Where(o => o.path.ToLower().Contains(fieldCropPestProperty.ToLower())).FirstOrDefault().value;
                var fieldCropPestToPatch = JsonConvert.DeserializeObject<IEnumerable<FieldCropPestForUpdateDto>>(fieldCropPestOperations.ToString());

                var newPests = new List<string>();
                foreach (var cropPest in field.FieldCrop.FieldCropPests)
                {
                    var cropPestInPatchList = fieldCropPestToPatch.Where(r => r.Id == cropPest.Id).FirstOrDefault();

                    if (cropPestInPatchList == null)
                    {
                        this.dataService.FieldCropPests.Delete(cropPest);
                    }
                    else
                    {
                        if (cropPest.CropPest.PestEppoCode != cropPestInPatchList.PestEppoCode)
                        {
                            this.dataService.FieldCropPests.Delete(cropPest);
                            newPests.Add(cropPestInPatchList.PestEppoCode);
                        }
                    }
                }
                foreach (var newCropPest in fieldCropPestToPatch.Where(f => f.Id == Guid.Empty))
                {
                    newPests.Add(newCropPest.PestEppoCode);
                }
                foreach (var pestEppoCode in newPests)
                {
                    await AddPestsToField(pestEppoCode, field);
                }
            }
        }

        private async Task UpdateFieldCropPestByOperations(Field field, JsonPatchDocument<FieldForUpdateDto> patchDocument)
        {
            var fieldCropPestProperty = nameof(FieldForUpdateDto.FieldCropPestDto);
            if (patchDocument.Operations.Any(o => o.path.ToLower().Contains(fieldCropPestProperty.ToLower())))
            {
                var fieldCropPestOperations = patchDocument.Operations.Where(o => o.path.ToLower().Contains(fieldCropPestProperty.ToLower()));

                foreach (var operation in fieldCropPestOperations)
                {
                    switch (operation.op.ToLower())
                    {
                        case "add":
                            await AddPestsToField(operation.value.ToString(), field);
                            break;
                        case "remove":
                            FieldCropPest cropPestToDelete = GetFieldCropPestFromOperationPath(field, operation);
                            if (cropPestToDelete != null) this.dataService.FieldCropPests.Delete(cropPestToDelete);
                            break;
                        case "replace":
                            FieldCropPest cropPestToReplace = GetFieldCropPestFromOperationPath(field, operation);
                            if (cropPestToReplace != null) await ReplaceCropPestFromField(operation.value.ToString(), cropPestToReplace);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static FieldCropPest GetFieldCropPestFromOperationPath(Field field, Microsoft.AspNetCore.JsonPatch.Operations.Operation<FieldForUpdateDto> operation)
        {
            var fieldCropPestIdToReplace = Guid.Parse(operation.path.Split("/").LastOrDefault());
            var cropPestToReplace = field
                    .FieldCrop
                    .FieldCropPests
                    .Where(f => f.Id == fieldCropPestIdToReplace)
                    .FirstOrDefault();
            return cropPestToReplace;
        }
        #endregion
    }
}