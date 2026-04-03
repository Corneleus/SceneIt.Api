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
