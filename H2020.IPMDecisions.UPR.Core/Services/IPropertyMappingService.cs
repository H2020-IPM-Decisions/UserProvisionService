using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}