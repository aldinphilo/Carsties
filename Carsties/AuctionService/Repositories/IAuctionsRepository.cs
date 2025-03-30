using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.Repositories
{
    public interface IAuctionsRepository
    {
        IQueryable GetAllAuctionsAsync(string date);
        Task<Auction> GetAuctionByIdAsync(Guid id);
        Task<(int, Auction)> CreateAuctionAsync(Auction auction);
        Task<bool> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuction);
        Task<(bool, bool)> DeleteAuctionAsync(Guid id);
    }
}
