﻿using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("AuctionCreatedConsumer: " + context.Message.Id);

            var item = mapper.Map<Item>(context.Message);

            // Error Throw for testing

            if (item.Model == "Foo") throw new ArgumentException("Foo is not a valid model");

            await item.SaveAsync();
        }

    }

}
