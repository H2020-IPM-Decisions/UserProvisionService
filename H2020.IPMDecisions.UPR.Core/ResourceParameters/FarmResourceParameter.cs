namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public class FarmResourceParameter : BaseResourceWithChildrenParameter
    {
        public string GroupBy { get; set; }
        const int maxPageSize = 20;
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
             
        const int maxChildPageSize = 10;
        private int _childPageSize = 5;
        public override int ChildPageSize
        {
            get { return _childPageSize; }
            set { _childPageSize = (value > maxChildPageSize) ? maxChildPageSize : value; }
        }
    }
}