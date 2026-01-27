using SceneIt.Api.Models;
using System.ComponentModel.DataAnnotations;

public class Movie
{
  [Key]
  public int MovieId {  get; set; } // primary key

  public required string Title { get; set; }
  public string? Year { get; set; }
  public string? Rated { get; set; }
  public DateTime? Released { get; set; }
  public string? Runtime { get; set; }
  public string? Genre { get; set; }
  public string? Director { get; set; }
  public string? Writer { get; set; }
  public string? Actors { get; set; }
  public string? Plot { get; set; }
  public string? Language { get; set; }
  public string? Country { get; set; }
  public string? Awards { get; set; }
  public string? Poster { get; set; }
  public string? Metascore { get; set; }
  public string? ImdbRating { get; set; }
  public string ImdbId { get; set; } = string.Empty;
  public string? Type { get; set; }
  public string? Dvd { get; set; }
  public string? BoxOffice { get; set; }
  public string? Production { get; set; }

  public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
}
