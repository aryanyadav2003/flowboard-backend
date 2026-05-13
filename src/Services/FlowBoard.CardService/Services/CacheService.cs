using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace FlowBoard.CardService.Services;

public class CacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache  = cache;
        _logger = logger;
    }

    // ── Get from cache ────────────────────────────────────────
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cached = await _cache.GetStringAsync(key);
            if (cached == null) return default;
            return JsonSerializer.Deserialize<T>(cached);
        }
        catch (Exception ex)
        {
            // If Redis is down, log and continue without cache
            _logger.LogWarning("Cache GET failed for {Key}: {Error}",
                key, ex.Message);
            return default;
        }
    }

    // ── Set in cache ──────────────────────────────────────────
    public async Task SetAsync<T>(string key, T value,
        TimeSpan? expiry = null)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };
            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Cache SET failed for {Key}: {Error}",
                key, ex.Message);
        }
    }

    // ── Remove from cache ─────────────────────────────────────
    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Cache REMOVE failed for {Key}: {Error}",
                key, ex.Message);
        }
    }

    // ── Cache Keys ────────────────────────────────────────────
    public static string CardKey(int cardId)
        => $"card:{cardId}";

    public static string CardsByListKey(int listId)
        => $"cards:list:{listId}";

    public static string CardsByBoardKey(int boardId)
        => $"cards:board:{boardId}";
}