using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class ShapedDataWithLinks
    {
        public IEnumerable<IDictionary<string, object>> Value { get; set; }
        public IEnumerable<LinkDto> Links { get; set; }
        public PaginationMetaData PaginationMetaData { get; set; }
    }
}