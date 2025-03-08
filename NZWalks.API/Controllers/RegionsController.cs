using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localgost:1234/api/Regions -> pointing to RegionsController
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionsController> _logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper, ILogger<RegionsController> logger) // using the dependency injection
        {
            _dbContext = dbContext;
            _regionRepository = regionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public NZWalksDbContext DbContext { get; }
        public IRegionRepository RegionRepository { get; }
        public IMapper Mapper { get; }

        // GET ALL REGIONS
        // GET: https://localgost:1234/api/Regions
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        // return type for async method is Task<>
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get data from database - Domain Models
                var regionsDomain = await _regionRepository.GetAllAsync();


                _logger.LogInformation($"Finished GetAllRegions request with data : {JsonSerializer.Serialize(regionsDomain)}");

                var regionsDto = _mapper.Map<List<RegionDto>>(regionsDomain);
                // return DTOs
                return Ok(regionsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }

        // GET REGION BY ID
        // GET: https://localgost:1234/api/Regions/{id}
        [HttpGet]
        [Route("{id:Guid}")] // maps to the method parameter
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = _dbContext.Regions.Find(id); // Find method only takes the primary key

            var regionDomain = await _regionRepository.GetByIdAsync(id);

            if (regionDomain == null) { 
                return NotFound();
            }

            // Map Region Domain Model to Region DTO
            //RegionDto regionDto = new RegionDto()
            //{
            //    Id = regionDomain.Id,
            //    Name = regionDomain.Name,
            //    Code = regionDomain.Code,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};

            var regionDto = _mapper.Map<RegionDto>(regionDomain);

            return Ok(regionDto);
        }

        // Create a new region
        // POST: https://localgost:1234/api/Regions
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        //post method -> annotate with FromBody, we receive a body from the client
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

            // Use Domain Model to create region
            regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);


            // Map domain model back to dto
            //var regionsDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};

            var regionsDto = _mapper.Map<RegionDto>(regionDomainModel);

            // in POST method return 201
            return CreatedAtAction(nameof(GetById), new { id = regionsDto.Id }, regionsDto);

            /*     nameof(GetById), new {id = regionsDto.Id} -> Providing the location (URL) where the client can retrieve
             * the newly created object later. Include location header for best practices.
             */



        }

        // Update region
        // PUT: https://localgost:1234/api/Regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }


            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            // Always pass Dtos to the client.
            return Ok(regionDto);

        }

        // Delete a region
        // DELETE: https://localgost:1234/api/Regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) { 
            var regionDomainModel = await _regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Return deleted region back to the client -> OPTIONAL
            // so, map domain model to Dto for sending
            //var regionDto = new RegionDto()
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }
    }
}
