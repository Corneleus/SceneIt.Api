using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class UserMediaItem
  {
    public int UserMediaItemId { get; set; }
    public int UserId { get; set; }
    public int MediaItemId { get; set; }
    public bool Owned { get; set; }
    public bool HasSeen { get; set; }
    public bool? Recommend { get; set; }

    [MaxLength(255)]
    public string? RecommendNotes { get; set; }

    public User User { get; set; } = null!;
    public MediaItem MediaItem { get; set; } = null!;
  }
}
