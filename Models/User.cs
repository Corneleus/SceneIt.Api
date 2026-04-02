using System.Collections.Generic;

namespace SceneIt.Api.Models
{
  public class User
  {
    public int UserId { get; set; }
    public required string Name { get; set; }

    // Navigation property
    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
  }
}
