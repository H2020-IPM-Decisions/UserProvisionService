using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary
                ?? throw new System.ArgumentNullException(nameof(mappingDictionary));
        }
    }
}