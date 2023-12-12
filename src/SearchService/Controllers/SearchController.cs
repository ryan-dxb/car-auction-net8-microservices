using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItem([FromQuery] string? q = null)
        {
            var query = DB.Find<Item>();

            query.Sort(x => x.Ascending(a => a.Make));

            if (!string.IsNullOrEmpty(q))
            {
                query.Match(Search.Full, q).SortByTextScore();
            }

            var result = await query.ExecuteAsync();

            return Ok(result);
        }
    }
}
