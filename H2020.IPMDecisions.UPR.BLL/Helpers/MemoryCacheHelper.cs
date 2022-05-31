using System;
using Microsoft.Extensions.Caching.Memory;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class MemoryCacheHelper
    {
        public static MemoryCacheEntryOptions CreateMemoryCacheEntryOptionsDays(int expirationDays, CacheItemPriority cachePriority = CacheItemPriority.Normal)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddDays(expirationDays),
                Priority = cachePriority
            };
        }

        public static MemoryCacheEntryOptions CreateMemoryCacheEntryOptionsMinutes(int expirationMinutes, CacheItemPriority cachePriority = CacheItemPriority.Normal)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(expirationMinutes),
                Priority = cachePriority
            };
        }
    }
}