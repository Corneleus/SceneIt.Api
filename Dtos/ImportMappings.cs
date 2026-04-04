using SceneIt.Api.Models;

namespace SceneIt.Api.Dtos
{
  public static class ImportMappings
  {
    public static ImportQueueItemResponseDto ToResponseDto(this ImportQueue item)
    {
      return new ImportQueueItemResponseDto
      {
        ImportQueueId = item.ImportQueueId,
        ImdbId = item.ImdbId,
        Title = item.Title,
        Year = item.Year,
        Rated = item.Rated,
        Runtime = item.Runtime,
        Genre = item.Genre,
        Director = item.Director,
        Writer = item.Writer,
        Actors = item.Actors,
        Plot = item.Plot,
        Language = item.Language,
        Country = item.Country,
        Awards = item.Awards,
        Poster = item.Poster,
        Metascore = item.Metascore,
        ImdbRating = item.ImdbRating,
        ImdbVotes = item.ImdbVotes,
        Type = item.Type,
        Dvd = item.Dvd,
        BoxOffice = item.BoxOffice,
        Production = item.Production,
        Status = item.Status.ToString(),
        AttemptCount = item.AttemptCount,
        LastAttemptedAtUtc = item.LastAttemptedAtUtc,
        ImportedAtUtc = item.ImportedAtUtc,
        ErrorMessage = item.ErrorMessage
      };
    }

    public static ImportRunResultDto ToResponseDto(this ImportRun run)
    {
      return new ImportRunResultDto
      {
        ImportRunId = run.ImportRunId,
        StartedAtUtc = run.StartedAtUtc,
        CompletedAtUtc = run.CompletedAtUtc,
        RequestedLimit = run.RequestedLimit,
        AttemptedCount = run.AttemptedCount,
        ImportedCount = run.ImportedCount,
        DuplicateCount = run.DuplicateCount,
        FailedCount = run.FailedCount,
        Notes = run.Notes
      };
    }
  }
}
