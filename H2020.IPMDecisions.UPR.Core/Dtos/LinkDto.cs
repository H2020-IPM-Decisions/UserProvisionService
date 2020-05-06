namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class LinkDto
    {
        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        public string Href { get; private set; }
        public string Rel { get; private set; }
        public string Method { get; private set; }
    }
}