using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class UserInformationInternalCall
    {
        public Guid Id { get; set; }
        public List<MyClaim> Claims { get; set; } = new List<MyClaim>();
    }

    public class MyClaim : Claim
    {
        public MyClaim(string type, string value, string valueType, string issuer, string originalIssuer) :
            base(type, value, valueType, issuer, originalIssuer)
        { }
    }
}