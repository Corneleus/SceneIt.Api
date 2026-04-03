namespace SceneIt.Api.Services
{
  public class ImportAutomationOptions
  {
    public bool Enabled { get; set; } = true;
    public bool RunOnStartup { get; set; } = true;
    public int IntervalMinutes { get; set; } = 1440;
    public int MaxImportsPerDay { get; set; } = 100;
    public int MaxCountPerRun { get; set; } = 100;
  }
}
