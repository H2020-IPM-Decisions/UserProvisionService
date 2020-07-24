namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class ChildrenResourceParameter
    {
        public string Fields { get; set; }
        public int PageNumber { get; set; } = 1;
        const int maxPageSize = 10;
        private int _pageSize = 5;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}