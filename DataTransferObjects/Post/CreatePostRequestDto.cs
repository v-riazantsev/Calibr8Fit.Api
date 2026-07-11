using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Post
{
    public record CreatePostRequestDto
    {
        [Required]
        public required string Content { get; init; }
        public List<IFormFile>? Images { get; init; }
    }
}