using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System;
namespace MSS.API.Common.DistributedEx
{
    //
    // 摘要:
    //     Extension methods for setting data in an Microsoft.Extensions.Caching.Distributed.IDistributedCache.
    public static class DistributedCacheExtensions
    {
        public static long? GetLong(this IDistributedCache cache, string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return RedisHelper.Get<long?>(key);
        }

        public static void SetLong(this IDistributedCache cache, string key, long value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            RedisHelper.Set(key, value);
        }
        
    }
}