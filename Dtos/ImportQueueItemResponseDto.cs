namespace SceneIt.Api.Dtos
{
  public class ImportQueueItemResponseDto
  {
    public int ImportQueueId { get; set; }
    public required string ImdbId { get; set; }
    public string? Title { get; set; }
    public string? Year { get; set; }
    public string? Rated { get; set; }
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
    public string? ImdbVotes { get; set; }
    public string? Type { get; set; }
    public string? Dvd { get; set; }
    public string? BoxOffice { get; set; }
    public string? Production { get; set; }
    public required string Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? LastAttemptedAtUtc { get; set; }
    public DateTime? ImportedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
  }
}
