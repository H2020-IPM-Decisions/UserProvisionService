namespace H2020.IPMDecisions.UPR.Core.Configurations
{
    public interface IDeIsipTokenInformation
    {
        string TokenEndpoint { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
    }
}
