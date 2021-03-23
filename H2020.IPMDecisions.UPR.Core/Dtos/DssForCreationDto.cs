namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    abstract public class DssForCreationDto
    {
        public abstract string DssId { get; set; }
        public abstract string DssModelId { get; set; }
        public abstract string DssParameters { get; set; }
        public abstract string DssExecutionType { get; set; }
        public abstract string DssVersion { get; set; }
    }
}