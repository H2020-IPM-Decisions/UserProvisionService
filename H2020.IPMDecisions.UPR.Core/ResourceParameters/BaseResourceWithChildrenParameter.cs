namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public abstract class BaseResourceWithChildrenParameter : BaseResourceParameter
    {
        public int ChildPageNumber { get; set; } = 1;
        const int maxChildPageSize = 5;
        private int _childPageSize = 5;
        public int ChildPageSize
        {
            get { return _childPageSize; }
            set { _childPageSize = (value > maxChildPageSize) ? maxChildPageSize : value; }
        }
    }
}