using System;
using H2020.IPMDecisions.UPR.BLL.Helpers;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public interface IJsonStringLocalizerProvider
    {
        IJsonStringLocalizer Create(Type resourceSource);
        IJsonStringLocalizer Create(string baseName, string location);
    }
}