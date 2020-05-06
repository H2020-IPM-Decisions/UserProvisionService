using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Revert { get; private set; }
        public PropertyMappingValue(IEnumerable<string> destinationProperties,
            bool revert = false)
        {
            DestinationProperties = destinationProperties
                ?? throw new System.ArgumentNullException(nameof(destinationProperties));
            this.Revert = revert;
        }        
    }
}