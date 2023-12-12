using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext auctionDbContext;
        private readonly IMapper mapper;

        public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper)
        {
            this.auctionDbContext = auctionDbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
        {
            var auctions = await auctionDbContext.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return mapper.Map<List<AuctionDTO>>(auctions);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById([FromRoute] Guid id)
        {
            var auction = await auctionDbContext.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return mapper.Map<AuctionDTO>(auction);
        }
    }
}
