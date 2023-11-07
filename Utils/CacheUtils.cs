using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace APICacheWithRedis.Utils {
    public class CacheUtils<T> {
        public static async Task<T?> GetCacheDataAsync(string cacheKey, IDistributedCache cache){
            try{
                var cacheData = await cache.GetAsync(cacheKey);
                T? Data;
                if(cacheData != null){
                    var cachedDataString = Encoding.UTF8.GetString(cacheData);
                    Data = JsonConvert.DeserializeObject<T>(cachedDataString);
                    return Data;
                }
                return default;
            }catch(Exception){
                return default;
            }
        }
        public static async Task SetCacheDataAsync(string cacheKey, T Data, IDistributedCache cache, DistributedCacheEntryOptions CacheOptions){
            try{
                var cachedDataString = JsonConvert.SerializeObject(Data);
                var newDataToCache = Encoding.UTF8.GetBytes(cachedDataString);
                //add to cache
                await cache.SetAsync(cacheKey, newDataToCache, CacheOptions);
            }catch(Exception){}
        }
        public static async Task RemoveCacheDataAsync(string cacheKey, IDistributedCache cache){
            try{
                await cache.RemoveAsync(cacheKey);
            }catch(Exception){}
        }
    }
}