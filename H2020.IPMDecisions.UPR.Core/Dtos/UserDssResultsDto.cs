using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserDssResultsDto
    {
        public IEnumerable<FieldDssResultDto> UserFieldDssResultDto { get; set; }
        public IEnumerable<LinkDssDto> UserLinkDssDto { get; set; }
    }
}