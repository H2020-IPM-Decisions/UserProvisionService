using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class Widget
    {
        [Key]
        public WidgetOption Id { get; set; }
        public string Description { get; set; }

        public ICollection<UserWidget> UserWidgets { get; set; }
    }
}