using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories
{
    public class AuctionsRepository : IAuctionsRepository
    {
        private readonly AuctionDbContext _context;

        public AuctionsRepository(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task<List<Auction>> GetAllAuctionsAsync()
        {
            var auctions = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return auctions;
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

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(bool, bool)> DeleteAuctionAsync(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return (false, false);

            _context.Auctions.Remove(auction);
            return (true, await _context.SaveChangesAsync() > 0);
        }
    }
}
