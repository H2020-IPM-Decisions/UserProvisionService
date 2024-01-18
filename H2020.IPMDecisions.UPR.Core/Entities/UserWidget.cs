using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class UserWidget
    {
        public Guid UserId { get; set; }
        public UserProfile UserProfile { get; set; }

        public int WidgetId { get; set; }
        public Widget Widget { get; set; }

        public bool Allowed { get; set; }
    }
}