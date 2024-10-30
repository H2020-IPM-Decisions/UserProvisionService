namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class FieldResourceParameter : BaseResourceWithChildrenParameter
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
                    base.OrderBy = "Name";
                }
                base.OrderBy = value;
            }
        }

        const int maxChildPageSize = 10;
        private int _childPageSize = 5;
        public override int ChildPageSize
        {
            get { return _childPageSize; }
            set { _childPageSize = (value > maxChildPageSize) ? maxChildPageSize : value; }
        }
    }
}