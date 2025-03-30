using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuctionsRepository _repository;

        public AuctionsController(IMapper mapper, IAuctionsRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _repository.GetAllAuctionsAsync();
            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _repository.GetAuctionByIdAsync(id);
            if (auction == null) return NotFound();
            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            auction.Seller = "test";
            var (status, result) = await _repository.CreateAuctionAsync(auction);
            if (status == 0) return BadRequest("");
            return Created("Created Auction", _mapper.Map<AuctionDto>(result));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> CreateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var result = await _repository.UpdateAuctionAsync(id, updateAuctionDto);
            if (!result) return BadRequest("");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var (userExist, status) = await _repository.DeleteAuctionAsync(id);
            if (!userExist) return NotFound();
            return status ? NoContent() : BadRequest();
        }
    }
}
