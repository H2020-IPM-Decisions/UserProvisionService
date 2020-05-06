namespace H2020.IPMDecisions.UPR.Core.Services
{
    public interface IPropertyCheckerService
    {
        bool TypeHasProperties<T>(string fields, bool mustHaveIdField = false);
    }
}