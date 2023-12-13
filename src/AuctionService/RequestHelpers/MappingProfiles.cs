using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Mapping from Auction to AuctionDTO and then to Item (nested mapping)
            CreateMap<Auction, AuctionDTO>().IncludeMembers(x => x.Item);

            CreateMap<Item, AuctionDTO>();

            // Mappping from CreateAuctionDTO to Auction and then to Item (nested mapping)
            CreateMap<CreateAuctionDTO, Auction>().ForMember(d => d.Item, opt => opt.MapFrom(s => s));

            CreateMap<CreateAuctionDTO, Item>();

            // Mapping for Contracts
            CreateMap<AuctionDTO, AuctionCreated>();
        }
    }
}
