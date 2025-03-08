using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IRegionRepository
    {
        // Since it is Async method return type is Task<>, it will return the list of regions from db
        Task<List<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(Guid id); // the returned region can be null
        Task<Region> CreateAsync(Region region);
        Task<Region?> UpdateAsync(Guid id, Region region);
        Task<Region?>  DeleteAsync(Guid id);
    }
}
