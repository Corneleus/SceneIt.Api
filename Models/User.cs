using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class User
  {
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    public ICollection<UserMediaItem> UserMediaItems { get; set; } = new List<UserMediaItem>();
  }
}
