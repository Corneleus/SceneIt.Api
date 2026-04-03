using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SceneIt.Api.Data;
using SceneIt.Api.Interfaces;

namespace SceneIt.Api.Services
{
  public class ImportAutomationService : BackgroundService
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImportAutomationService> _logger;
    private readonly ImportAutomationOptions _options;

    public ImportAutomationService(
      IServiceScopeFactory scopeFactory,
      IOptions<ImportAutomationOptions> options,
      ILogger<ImportAutomationService> logger)
    {
      _scopeFactory = scopeFactory;
      _logger = logger;
      _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      if (!_options.Enabled)
      {
        _logger.LogInformation("Import automation is disabled.");
        return;
      }

      if (_options.RunOnStartup)
      {
        await RunScheduledImportAsync(stoppingToken);
      }

      var intervalMinutes = Math.Max(1, _options.IntervalMinutes);
      using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));

      while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
      {
        await RunScheduledImportAsync(stoppingToken);
      }
    }

    private async Task RunScheduledImportAsync(CancellationToken cancellationToken)
    {
      try
      {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SceneItDbContext>();
        var importService = scope.ServiceProvider.GetRequiredService<IMovieImportService>();

        var todayUtc = DateTime.UtcNow.Date;
        var importedToday = await context.ImportRuns
          .Where(run => run.StartedAtUtc >= todayUtc)
          .SumAsync(run => run.ImportedCount, cancellationToken);

        var remainingToday = Math.Max(0, _options.MaxImportsPerDay - importedToday);

        if (remainingToday <= 0)
        {
          _logger.LogInformation("Import automation skipped: daily cap of {DailyCap} already reached.", _options.MaxImportsPerDay);
          return;
        }

        var runSize = Math.Min(Math.Max(1, _options.MaxCountPerRun), remainingToday);
        var result = await importService.RunBatchAsync(runSize, cancellationToken);

        _logger.LogInformation(
          "Import automation run completed. Attempted: {Attempted}, Imported: {Imported}, Duplicate: {Duplicate}, Failed: {Failed}, RemainingToday: {RemainingToday}.",
          result.AttemptedCount,
          result.ImportedCount,
          result.DuplicateCount,
          result.FailedCount,
          Math.Max(0, remainingToday - result.ImportedCount));
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        _logger.LogInformation("Import automation is stopping.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Import automation run failed.");
      }
    }
  }
}
