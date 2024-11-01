namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class FieldCropPestResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;
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
    }
}