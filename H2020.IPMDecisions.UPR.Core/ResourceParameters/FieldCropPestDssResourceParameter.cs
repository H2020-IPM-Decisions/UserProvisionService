using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class FieldCropPestDssResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 20;
        private int _pageSize = 10;
        public override int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        [BindRequired]
        public Guid FieldCropPestId { get; set; }
    }
}