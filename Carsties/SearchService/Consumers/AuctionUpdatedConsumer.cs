﻿using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming auction is updated : "+context.Message.Id);
            try
            {
                var item = _mapper.Map<Item>(context.Message);
                var result = await DB.Update<Item>()
                    .Match(a => a.ID == context.Message.Id)
                    .ModifyOnly(x => new
                    {
                        x.Colour,
                        x.Make,
                        x.Model,
                        x.Year,
                        x.Mileage
                    }, item)
                    .ExecuteAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
