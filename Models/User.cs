using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Models
{
  public class User
  {
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
  }
}
