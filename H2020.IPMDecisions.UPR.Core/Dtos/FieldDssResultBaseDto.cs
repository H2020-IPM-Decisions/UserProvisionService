using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FieldDssResultBaseDto
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
        public string FarmName { get; set; }
        public Guid FieldId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsValid { get; set; }
        public string CropEppoCode { get; set; }
        public IDictionary<string, string> CropLanguages { get; set; }
        public string PestEppoCode { get; set; }
        public IDictionary<string, string> PestLanguages { get; set; }
        public string DssId { get; set; }
        public string DssName { get; set; }
        public string DssModelId { get; set; }
        public string DssModelName { get; set; }
        public string DssVersion { get; set; }
        public string DssModelVersion { get; set; }
        public string DssExecutionType { get; set; }
        public string DssFullResult { get; set; }
        public string DssDescription { get; set; }
        public string DssPurpose { get; set; }
        public string DssSource { get; set; }
        public string DssEndPoint { get; set; }
        public string DssLogoUrl { get; set; }
        public int WarningStatus { get; set; }
        public string WarningStatusRepresentation { get; set; }
        public string WarningMessage { get; set; }
        public int? ResultMessageType { get; set; }
        public string ResultMessage { get; set; }
        public IEnumerable<string> ValidatedSpatialCountries { get; set; }
        public IEnumerable<DssModelAuthorsDto> Authors { get; set; }
    }
}