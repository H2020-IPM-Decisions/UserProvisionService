namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class ChildrenResourceParameter : BaseResourceParameter
    {
        const int maxPageSize = 10;
        private int _pageSize = 5;
        public override int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}