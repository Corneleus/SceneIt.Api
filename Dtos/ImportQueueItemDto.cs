using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Dtos
{
  public class ImportQueueItemDto
  {
    [Required]
    public required string ImdbId { get; set; }

    public string? Title { get; set; }
  }
}
