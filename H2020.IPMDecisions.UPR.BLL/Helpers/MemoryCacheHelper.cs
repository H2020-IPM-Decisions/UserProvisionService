using System;
using Microsoft.Extensions.Caching.Memory;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class MemoryCacheHelper
    {
        public static MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(int expirationDays, CacheItemPriority cachePriority = CacheItemPriority.Normal)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddDays(expirationDays),
                Priority = cachePriority
            };
        }
    }
}