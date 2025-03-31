using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories
{
    public class AuctionsRepository : IAuctionsRepository
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsRepository(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public IQueryable GetAllAuctionsAsync(string date)
        {

            var query = _context.Auctions
                .OrderBy(x => x.Item.Make)
                .AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return query;
        }

        public async Task<Auction> GetAuctionByIdAsync(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
            return auction;
        }

        public async Task<(int, Auction)> CreateAuctionAsync(Auction auction)
        {
            _context.Auctions.Add(auction);
            var newAction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAction));
            var result = await _context.SaveChangesAsync();
            return (result, auction);
        }

        public async Task<bool> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuction)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return false;


            auction.Item.Make = updateAuction.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuction.Model ?? auction.Item.Model;
            auction.Item.Colour = updateAuction.Colour ?? auction.Item.Colour;
            auction.Item.Mileage = updateAuction.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuction.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(bool, bool)> DeleteAuctionAsync(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return (false, false);

            _context.Auctions.Remove(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionDeleted>(new { id = auction.Id.ToString() }));

            return (true, await _context.SaveChangesAsync() > 0);
        }
    }
}
