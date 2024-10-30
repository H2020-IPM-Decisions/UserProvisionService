namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class DataShareResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 50;
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
                    base.OrderBy = "RequestStatus";
                }

                base.OrderBy = value;
            }
        }
        public string RequestStatus { get; set; }
    }
}