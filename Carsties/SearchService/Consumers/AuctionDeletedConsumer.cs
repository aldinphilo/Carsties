using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionDeletedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming auction deleted : " + context.Message.Id);
            try
            {
                var item = _mapper.Map<Item>(context.Message);
                var result = await DB.DeleteAsync<Item>(context.Message.Id);
                if (!result.IsAcknowledged)
                    throw new MessageException(typeof(AuctionDeleted), "Problem deleting Auction");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
