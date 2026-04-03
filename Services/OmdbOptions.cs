namespace SceneIt.Api.Services
{
  public sealed class OmdbOptions
  {
    public string BaseUrl { get; set; } = "https://www.omdbapi.com/";
    public string ApiKey { get; set; } = string.Empty;
  }
}
