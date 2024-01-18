namespace H2020.IPMDecisions.UPR.Core.ResourceParameters
{
    public abstract class BaseResourceParameter
    {
        public string SearchQuery { get; set; }
        public virtual int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public virtual string OrderBy { get; set; }
        public string Fields { get; set; }
    }
}