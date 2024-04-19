namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDto : FarmBaseDto
    {
        public bool IsShared { get; set; } = false;
        public bool Owner { get; set; } = true;
        public string AdvisorName { get; set; }
        public string OwnerName { get; set; }
    }
}