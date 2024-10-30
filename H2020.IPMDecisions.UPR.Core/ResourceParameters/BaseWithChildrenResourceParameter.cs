namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public abstract class BaseResourceWithChildrenParameter : BaseResourceParameter
    {
        public virtual int ChildPageSize { get; set; } = 10;
        public int ChildPageNumber { get; set; } = 1;
    }
}