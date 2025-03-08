using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NZWalksDbContext _dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, NZWalksDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public async Task<Image> Upload(Image image)
        {
            // Folder path to the api solution in your computer
            // root directory of the application + Images folder + image name + image extension
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", 
                $"{image.FileName}{image.FileExtension}");

            // Upload image to local path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // https://localhost:1213/images/image.jpg    Api should provide us the images online
            // Use _httpContextAccessor to create a path to the image that we just uploaded.

            // Scheme -> whether it is Http or Https
            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath; // Get the image from the running application


            // Add Image to the Images table
            await _dbContext.Images.AddAsync(image);
            await _dbContext.SaveChangesAsync();

            return image;
        }
    }
}
