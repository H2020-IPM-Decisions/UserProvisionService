using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class FieldObservationResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 20;
        private int _pageSize = 25;
        public override int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public override string OrderBy
        {
            get => base.OrderBy;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.OrderBy = "Name";
                }
                base.OrderBy = value;
            }
        }

        [BindRequired]
        public Guid FieldCropPestId { get; set; }
    }
}