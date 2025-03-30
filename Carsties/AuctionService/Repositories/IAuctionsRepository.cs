using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.Repositories
{
    public interface IAuctionsRepository
    {
        Task<List<Auction>> GetAllAuctionsAsync();
        Task<Auction> GetAuctionByIdAsync(Guid id);
        Task<(int, Auction)> CreateAuctionAsync(Auction auction);
        Task<bool> UpdateAuctionAsync(Guid id, UpdateAuctionDto updateAuction);
        Task<(bool, bool)> DeleteAuctionAsync(Guid id);
    }
}
