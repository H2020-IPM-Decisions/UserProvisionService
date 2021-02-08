using System;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class UrlCreatorHelper
    {
        #region Farms
        internal static IEnumerable<LinkDto> CreateLinksForFarms(
            this IUrlHelper url,
            FarmResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFarmResourceUri(url, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFarmResourceUri(url, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFarmResourceUri(url, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFarmResourceUri(
            IUrlHelper url,
            FarmResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.farm.get.all",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.farm.get.all",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.farm.get.all",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
            }
        }

        internal static IEnumerable<LinkDto> CreateLinksForFarm(
            this IUrlHelper url,
            Guid farmId,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.farm.get.farmbyid", new { farmId }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.farm.get.farmbyid", new { farmId, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.farm.delete.farmbyid", new { farmId }),
                "delete_farm",
                "DELETE"));

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.farm.patch.farmbyid", new { farmId }),
                "update_farm",
                "PATCH"));

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.field.get.all", new { farmId }),
                "farm_fields",
                "GET"));

            return links;
        }
        #endregion

        #region FieldCropDecisions
        internal static IEnumerable<LinkDto> CreateLinksForFieldCropDecisions(
            IUrlHelper url,
            Guid fieldId,
            FieldCropPestDssResourceParameter resourceParameter,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldCropDecisionResourceUri(url, fieldId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(
                CreateFieldCropDecisionResourceUri(url, fieldId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(
               CreateFieldCropDecisionResourceUri(url, fieldId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFieldCropDecisionResourceUri(
            IUrlHelper url,
            Guid fieldId,
            FieldCropPestDssResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.fieldcropdecisions.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.fieldcropdecisions.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.fieldcropdecisions.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                    });
            }
        }

        internal static IEnumerable<LinkDto> CreateLinksForFieldCropDecision(
           this IUrlHelper url,
           Guid id,
           Guid fieldId,
           string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.fieldcropdecisions.get.cropdecisionbyid", new { fieldId, id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.fieldcropdecisions.get.cropdecisionbyid", new { fieldId, id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.fieldcropdecisions.delete.cropdecisionbyid", new { fieldId, id }),
                "delete_field_crop_decision",
                "DELETE"));

            return links;
        }
        #endregion

        #region Fields
        internal static IEnumerable<LinkDto> CreateLinksForFields(
            this IUrlHelper url,
            Guid farmId,
            FieldResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldResourceUri(url, farmId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFieldResourceUri(url, farmId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFieldResourceUri(url, farmId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFieldResourceUri(
            IUrlHelper url,
            Guid farmId,
            FieldResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.field.get.all",
                    new
                    {
                        farmId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.field.get.all",
                    new
                    {
                        farmId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.field.get.all",
                    new
                    {
                        farmId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery,
                        childPageNumber = resourceParameter.ChildPageNumber,
                        childPageSize = resourceParameter.ChildPageSize
                    });
            }
        }

        internal static IEnumerable<LinkDto> CreateLinksForField(
            this IUrlHelper url,
            Guid id,
            Guid farmId,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.field.get.fieldbyid", new { farmId, id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 GenerateResourceLink(url, "api.field.get.fieldbyid", new { farmId, id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.field.delete.fieldbyid", new { farmId, id }),
                "delete_field",
                "DELETE"));

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.field.patch.fieldbyid", new { farmId, id }),
                "update_field",
                "PATCH"));

            return links;
        }
        #endregion

        #region Profiles
        internal static IEnumerable<LinkDto> CreateLinksForUserProfile(
            this IUrlHelper url,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.userprofile.get.profilebyid", new { }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 GenerateResourceLink(url, "api.userprofile.get.profilebyid", new { fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.userprofile.delete.profilebyid", new { }),
                "delete_user_profile",
                "DELETE"));

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.userprofile.patch.profilebyid", new { }),
                "update_user_profile",
                "PATCH"));

            return links;
        }
        #endregion

        #region Observations
        internal static IEnumerable<LinkDto> CreateLinksForFieldObservations(
            this IUrlHelper url,
            Guid fieldId,
            FieldObservationResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldObservationResourceUri(url, fieldId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFieldObservationResourceUri(url, fieldId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFieldObservationResourceUri(url, fieldId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFieldObservationResourceUri(
            IUrlHelper url,
            Guid fieldId,
            FieldObservationResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.observation.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.observation.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.observation.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
            }
        }

        internal static IEnumerable<LinkDto> CreateLinksForFieldObservation(
           this IUrlHelper url,
           Guid id,
           Guid fieldId,
           string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.observation.get.observationbyid", new { fieldId, id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 GenerateResourceLink(url, "api.observation.get.observationbyid", new { fieldId, id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.observation.delete.observationbyid", new { fieldId, id }),
                "delete_field_observation",
                "DELETE"));

            return links;
        }
        #endregion

        #region DataShare
        internal static IEnumerable<LinkDto> CreateLinksForRequests(
            IUrlHelper url,
            DataShareResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateDataRequestResourceUri(url, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateDataRequestResourceUri(url, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateDataRequestResourceUri(url, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateDataRequestResourceUri(
            IUrlHelper url,
            DataShareResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.datashare.get.all",
                    new
                    {
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        requeststatus = resourceParameter.RequestStatus
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.datashare.get.all",
                    new
                    {
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        requeststatus = resourceParameter.RequestStatus
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.datashare.get.all",
                    new
                    {
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        requeststatus = resourceParameter.RequestStatus
                    });
            }
        }
        #endregion

        #region CropPests
        internal static IEnumerable<LinkDto> CreateLinksForFieldCropPests(
            IUrlHelper url,
            Guid fieldId,
            FieldCropPestResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldCropPestResourceUri(url, fieldId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFieldCropPestResourceUri(url, fieldId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFieldCropPestResourceUri(url, fieldId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFieldCropPestResourceUri(
            IUrlHelper url,
            Guid fieldId,
            FieldCropPestResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.fieldcroppests.get.all",
                    new
                    {
                        fieldId,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.fieldcroppests.get.all",
                    new
                    {
                        fieldId,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.fieldcroppests.get.all",
                    new
                    {
                        fieldId,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                    });
            }
        }
        #endregion

        #region FieldSpray
        internal static IEnumerable<LinkDto> CreateLinksForFieldSprays(IUrlHelper url, Guid fieldId, FieldSprayResourceParameter resourceParameter, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(
                CreateFieldSprayResourceUri(url, fieldId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(
                CreateFieldSprayResourceUri(url, fieldId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPrevious)
            {
                links.Add(new LinkDto(
               CreateFieldSprayResourceUri(url, fieldId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private static string CreateFieldSprayResourceUri(
            IUrlHelper url,
            Guid fieldId,
            FieldSprayResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return GenerateResourceLink(url, "api.spray.get.all",
                    new
                    {
                        fieldId,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return GenerateResourceLink(url, "api.spray.get.all",
                    new
                    {
                        fieldId,
                        fields = resourceParameter.Fields,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.Current:
                default:
                    return GenerateResourceLink(url, "api.spray.get.all",
                    new
                    {
                        fieldId,
                        fields = resourceParameter.Fields,
                        fieldCropPestId = resourceParameter.FieldCropPestId,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
            }
        }

        internal static IEnumerable<LinkDto> CreateLinksForFieldSpray(
           this IUrlHelper url,
           Guid id,
           Guid fieldId,
           string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                GenerateResourceLink(url, "api.spray.get.spraybyid", new { fieldId, id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 GenerateResourceLink(url, "api.spray.get.spraybyid", new { fieldId, id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                GenerateResourceLink(url, "api.spray.delete.spraybyid", new { fieldId, id }),
                "delete_field_spray",
                "DELETE"));

            return links;
        }
        #endregion

        #region Common
        private static string ChangeUrlToApiGatewayHost(IUrlHelper url, string urlReturn)
        {
            if (string.IsNullOrEmpty(urlReturn)) return urlReturn;
            var hasForwadedHeaders = url.ActionContext.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For");
            if (!hasForwadedHeaders) return urlReturn;

            return urlReturn
                .Replace(url.ActionContext.HttpContext.Request.Scheme + "://", "")
                .Replace(
                    url.ActionContext.HttpContext.Request.Host.Value,
                    url.ActionContext.HttpContext.Request.Headers["X-Forwarded-For"].ToString())
                .Replace("/api/", "/api/upr/");
        }

        public static string GenerateResourceLink(IUrlHelper url, string routeName, object parameters)
        {
            var newUrl = url.Link(routeName, parameters);
            return ChangeUrlToApiGatewayHost(url, newUrl);
        }
        #endregion
    }
}