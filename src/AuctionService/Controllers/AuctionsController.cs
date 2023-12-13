using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
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
        private readonly IPublishEndpoint publishEndpoint;

        public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this.auctionDbContext = auctionDbContext;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions([FromQuery] string? date)
        {
            var query = auctionDbContext.Auctions.OrderBy(x => x.Item).AsQueryable();

            if (!string.IsNullOrWhiteSpace(date))
            {
                query = query.Where(x => x.LastUpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDTO>(mapper.ConfigurationProvider).ToListAsync();
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

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction([FromBody] CreateAuctionDTO createAuctionDTO)
        {
            var auction = mapper.Map<Auction>(createAuctionDTO);

            // TODO:Add Current User as seller
            auction.Seller = "Test";

            auctionDbContext.Auctions.Add(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            var newAuction = mapper.Map<AuctionDTO>(auction);

            await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

            if (!result)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, mapper.Map<AuctionDTO>(auction));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDTO updateAuctionDTO)
        {
            Console.WriteLine(id);

            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }


            // TODO: Check seller is current user
            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;


            var result = await auctionDbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAuction([FromRoute] Guid id)
        {
            var auction = await auctionDbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            // TODO: check seller is current user

            auctionDbContext.Remove(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
