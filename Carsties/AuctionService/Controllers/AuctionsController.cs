using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuctionsRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(IMapper mapper, IAuctionsRepository repository, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        {
            var auctions = _repository.GetAllAuctionsAsync(date);
            return await auctions.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _repository.GetAuctionByIdAsync(id);
            if (auction == null) return NotFound();
            return _mapper.Map<AuctionDto>(auction);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAuction(CreateAuctionDto auctionDto)
        {
            try
            {
                var auction = _mapper.Map<Auction>(auctionDto);
                auction.Seller = User.Identity.Name;
                var (status, result) = await _repository.CreateAuctionAsync(auction);
                if (status == 0) return BadRequest("");
                var auctionResult = _mapper.Map<AuctionDto>(result);
                return Created("Created Auction", auctionResult);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var (result, status) = await _repository.UpdateAuctionAsync(id, updateAuctionDto, User.Identity.Name);
            if (!status) return Forbid();
            if (!result) return BadRequest("");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var (userExist, status, userAllowed) = await _repository.DeleteAuctionAsync(id, User.Identity.Name);
            if (!userAllowed) return Forbid();
            if (!userExist) return NotFound();
            return status ? NoContent() : BadRequest();
        }
    }
}
