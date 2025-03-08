using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/Walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWalkRepository _walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            _mapper = mapper;
            _walkRepository = walkRepository;
        }

        // CREATE WALK
        // POST: /api/Walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // Map DTO to Domain Model
            var walkDomainModel = _mapper.Map<Walk>(addWalkRequestDto);

            walkDomainModel = await _walkRepository.CreateAsync(walkDomainModel);

            var walkDto = _mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);

        }

        // GET Walks
        // GET: /api/Walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10       Name'lerde Track kelimesi olanları döndür
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            List<Walk> walksDomainModel = await _walkRepository.GetAllAsync(filterOn, filterQuery, 
                sortBy, isAscending ?? true, 
                pageNumber, pageSize);

            var walkDto = _mapper.Map<List<WalkDto>>(walksDomainModel);

            return Ok(walkDto);
        }

        // Get Walk by Id
        // GET: /api/Walks/id
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await _walkRepository.GetByIdAsync(id);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            var walkDto = _mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }

        // Update Walk By id
        // PUT: /api/Walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {

            var walkDomainModel = _mapper.Map<Walk>(updateWalkRequestDto);

            walkDomainModel = await _walkRepository.UpdateAsync(id, walkDomainModel);

            if (walkDomainModel == null)
            { return NotFound(); }

            var walkDto = _mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);


        }

        // Delete Walk by id
        // DELETE: /api/Walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomainModel = await _walkRepository.DeleteAsync(id);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            var walkDto = _mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }
    }
}
