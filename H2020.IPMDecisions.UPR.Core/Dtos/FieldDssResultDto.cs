namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultDto : FieldDssResultBaseDto
    {
        public FieldDssResultDto()
        {
            this.DssTaskStatusDto = new DssTaskStatusDto();
        }

        public DssTaskStatusDto DssTaskStatusDto { get; set; }
        public bool IsDisabled { get; set; } = false;
        public bool IsValidInLocation { get; set; } = true;
    }
}