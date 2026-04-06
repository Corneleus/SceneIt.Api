namespace SceneIt.Api.Services
{
  public static class MediaKindMatcher
  {
    private static readonly string[] MovieTokens = ["movie"];
    private static readonly string[] SeriesTokens = ["series", "tvseries", "tvminiseries", "miniseries"];
    private static readonly string[] VideoGameTokens = ["videogame", "game"];

    public static bool Matches(string? type, string? kind)
    {
      if (string.IsNullOrWhiteSpace(kind))
      {
        return true;
      }

      var normalizedKind = NormalizeToken(kind);
      var normalizedType = NormalizeToken(type);

      return normalizedKind switch
      {
        "movie" => MovieTokens.Contains(normalizedType),
        "series" => SeriesTokens.Contains(normalizedType),
        "videogame" => VideoGameTokens.Contains(normalizedType),
        _ => false,
      };
    }

    public static string NormalizeToken(string? value)
    {
      return (value ?? string.Empty)
        .Trim()
        .Replace(" ", string.Empty)
        .Replace("-", string.Empty)
        .ToLowerInvariant();
    }

    public static IReadOnlyList<string> GetTypeTokens(string? kind)
    {
      var normalizedKind = NormalizeToken(kind);

      return normalizedKind switch
      {
        "" => [],
        "movie" => MovieTokens,
        "series" => SeriesTokens,
        "videogame" => VideoGameTokens,
        _ => [],
      };
    }
  }
}
