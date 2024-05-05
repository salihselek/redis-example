using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Distributed.Caching.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("set")]
        public async Task<IActionResult> Set(string name, string surname)
        {
            DistributedCacheEntryOptions option = new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(5)
            };

            await _distributedCache.SetStringAsync("name", name, options: option);
            await _distributedCache.SetAsync("surname", Encoding.UTF8.GetBytes(surname), options: option);
            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var name = await _distributedCache.GetStringAsync("name");
            var surnameBinary = await _distributedCache.GetAsync("surname");
            var surname = Encoding.UTF8.GetString(surnameBinary);
            return Ok(new
            {
                name,
                surname
            });
        }
    }
}
