using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItem(
            [FromQuery] SearchParams searchParams)

        {
            var query = DB.PagedSearch<Item, Item>();


            if (!string.IsNullOrEmpty(searchParams.Query))
            {
                query.Match(Search.Full, searchParams.Query).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)).Sort(x => x.Ascending(x => x.Model)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEndsAt))
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEndsAt < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEndsAt < DateTime.UtcNow.AddHours(6)
                    && x.AuctionEndsAt > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEndsAt > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }

            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

            var result = await query.ExecuteAsync();

            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount,
            });
        }
    }
}
