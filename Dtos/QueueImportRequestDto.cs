using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Dtos
{
  public class QueueImportRequestDto
  {
    [Required]
    [MinLength(1)]
    public List<ImportQueueItemDto> Items { get; set; } = [];
  }
}
