using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly ILogger<HackerNewsController> _logger;
        private readonly IConfiguration _config;
        private IMemoryCache _cache;

        public HackerNewsController(ILogger<HackerNewsController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _cache = memoryCache;
        }

        [HttpGet]
        public async Task<IEnumerable<HackerNewsStory>> Get()
        {
            const string cacheKey = "HackerNewsResult";

            _logger.LogInformation("HackerNews-Get: Called");

            // Get Cache slide and absolute expire values from settings file, if not exists use hardcoded default values
            int slideExpireSecs = _config.GetValue<int?>("Cache:SlidingExpiration") ?? 30;
            int absExpireSecs = _config.GetValue<int?>("Cache:AbsoluteExpiration") ?? 300;

            IEnumerable<HackerNewsStory> result;
            try
            {
                // Check if item is already in MemCache, 
                // if it is then return the cached version, if not call the hackernews webservice and place it on cache
                if (!_cache.TryGetValue(cacheKey, out result))
                {
                    _logger.LogDebug("HackerNews-Get: Getting from external webservice");

                    // Call base API
                    result = await HackerNewsClient.GetHackerNewsStories(20);

                    // Add returned item to cache using the already defined expiration values
                    _cache.Set(cacheKey,
                               result,
                               new MemoryCacheEntryOptions()
                               {
                                   SlidingExpiration = TimeSpan.FromSeconds(slideExpireSecs),
                                   AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(absExpireSecs)
                               });
                }
                else
                {
                    _logger.LogDebug("HackerNews-Get: Getting from cache");
                }

                _logger.LogInformation("HackerNews-Get: Returning json item");
                return result.OrderByDescending(x => x.score);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurd while calling the HackerNews API.");
                throw;
            }
        }
    }
}
