using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalks.API.Models.Domain
{
    public class Image
    {
        [Key]  // Ensure this is marked as a key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-generate the Id
        public Guid Id { get; set; }

        [NotMapped] // Not store this file inside the database
        public IFormFile File { get; set; }

        public string FileName { get; set; }

        public string? FileDescription { get; set; }

        public string FileExtension { get; set; }

        public long FileSizeInBytes { get; set; }

        public string FilePath { get; set; }
    }
}
