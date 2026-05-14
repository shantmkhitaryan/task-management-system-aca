using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedisTestController : ControllerBase
{
    private readonly RedisCacheService _redisService;

    public RedisTestController(RedisCacheService redisService)
    {
        _redisService = redisService;
    }

    
    [HttpPost("save")]
    public async Task<IActionResult> SaveData([FromBody] SaveDataRequest request)
    {
        var data = new
        {
            Message = request.Message,
            SavedAt = DateTime.UtcNow,
            ExpiresIn = "30 seconds"
        };

        await _redisService.SetAsync(request.Key, data, 30);

        return Ok(new 
        { 
            message = "Data saved in Redis for 30 seconds",
            key = request.Key,
            data = data
        });
    }

    
    [HttpGet("get/{key}")]
    public async Task<IActionResult> GetData(string key)
    {
        var data = await _redisService.GetAsync<object>(key);
        
        if (data == null)
        {
            return Ok(new { message = "Data not found or expired (30 seconds passed)" });
        }

        return Ok(new { key, data });
    }

    
    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> DeleteData(string key)
    {
        await _redisService.RemoveAsync(key);
        return Ok(new { message = $"Data with key '{key}' removed from Redis" });
    }
}

public class SaveDataRequest
{
    public string Key { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}