namespace H2020.IPMDecisions.UPR.Core.Configurations
{
    public class DeIsipTokenInformation : IDeIsipTokenInformation
    {
        public string TokenEndpoint { get; set; }
        public string TokenType { get; set; }

        public int ExpiresInSeconds { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
