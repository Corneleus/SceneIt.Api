namespace SceneIt.Api.Services
{
  public static class MediaKindMatcher
  {
    public static bool Matches(string? type, string? kind)
    {
      if (string.IsNullOrWhiteSpace(kind))
      {
        return true;
      }

      var normalizedKind = Normalize(kind);
      var normalizedType = Normalize(type);

      return normalizedKind switch
      {
        "movie" => normalizedType == "movie",
        "series" => normalizedType is "series" or "tvseries" or "tvminiseries" or "miniseries",
        "videogame" => normalizedType is "videogame" or "game",
        _ => false,
      };
    }

    private static string Normalize(string? value)
    {
      return (value ?? string.Empty)
        .Trim()
        .Replace(" ", string.Empty)
        .Replace("-", string.Empty)
        .ToLowerInvariant();
    }
  }
}
