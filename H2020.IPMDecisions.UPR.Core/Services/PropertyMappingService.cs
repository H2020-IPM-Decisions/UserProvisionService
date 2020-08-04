using System;
using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _farmMappingService =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" })},
            };

        private Dictionary<string, PropertyMappingValue> _fieldMappingService =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", new PropertyMappingValue(new List<string>() { "Name" })},
            };

        private Dictionary<string, PropertyMappingValue> _fieldObservationMappingService =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "PestEppoCode", new PropertyMappingValue(new List<string>() { "PestEppoCode" })},
                { "CropEppoCode", new PropertyMappingValue(new List<string>() { "CropEppoCode" })},
            };

        private Dictionary<string, PropertyMappingValue> _dataShareMappingService =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "RequestStatus", new PropertyMappingValue(new List<string>() { "RequestStatus" })},
                { "RequesteeName", new PropertyMappingValue(new List<string>() { "Requestee" })},
                { "RequesterName", new PropertyMappingValue(new List<string>() { "Requester" })},
            };

        public IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            this.propertyMappings.Add(new PropertyMapping<FarmDto, Farm>(_farmMappingService));
            this.propertyMappings.Add(new PropertyMapping<FieldDto, Field>(_fieldMappingService));
            this.propertyMappings.Add(new PropertyMapping<FieldObservationDto, FieldObservation>(_fieldObservationMappingService));
            this.propertyMappings.Add(new PropertyMapping<DataShareRequestDto, DataSharingRequest>(_dataShareMappingService));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = this.propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
                return matchingMapping.First()._mappingDictionary;

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }
            return true;
        }
    }
}