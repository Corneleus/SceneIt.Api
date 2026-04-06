using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SceneIt.Api.Dtos;
using SceneIt.Api.Interfaces;

namespace SceneIt.Api.Services
{
  public class OmdbImportClient : IOmdbImportClient
  {
    private readonly HttpClient _httpClient;
    private readonly OmdbOptions _options;

    public OmdbImportClient(HttpClient httpClient, IOptions<OmdbOptions> options)
    {
      _httpClient = httpClient;
      _options = options.Value;
    }

    public async Task<CreateMediaItemRequestDto?> LookupByImdbIdAsync(string imdbId, CancellationToken cancellationToken = default)
    {
      var apiKey = _options.ApiKey;

      if (string.IsNullOrWhiteSpace(apiKey))
      {
        throw new OmdbException("OMDb API key is not configured.", StatusCodes.Status503ServiceUnavailable);
      }

      var normalizedImdbId = imdbId.Trim();
      var response = await _httpClient.GetFromJsonAsync<OmdbMovieResponse>(
        $"?apikey={Uri.EscapeDataString(apiKey)}&i={Uri.EscapeDataString(normalizedImdbId)}",
        cancellationToken);

      if (response is null)
      {
        return null;
      }

      if (string.Equals(response.Response, "False", StringComparison.OrdinalIgnoreCase))
      {
        if (string.Equals(response.Error, "Movie not found!", StringComparison.OrdinalIgnoreCase))
        {
          return null;
        }

        throw new OmdbException(
          response.Error ?? "OMDb lookup failed.",
          StatusCodes.Status502BadGateway);
      }

      if (string.IsNullOrWhiteSpace(response.Title) || string.IsNullOrWhiteSpace(response.ImdbId))
      {
        return null;
      }

      return new CreateMediaItemRequestDto
      {
        Title = response.Title.Trim(),
        Year = Normalize(response.Year),
        Rated = Normalize(response.Rated),
        Released = ParseReleased(response.Released),
        Runtime = Normalize(response.Runtime),
        Genre = Normalize(response.Genre),
        Director = Normalize(response.Director),
        Writer = Normalize(response.Writer),
        Actors = Normalize(response.Actors),
        Plot = Normalize(response.Plot),
        Language = Normalize(response.Language),
        Country = Normalize(response.Country),
        Awards = Normalize(response.Awards),
        Poster = Normalize(response.Poster),
        Metascore = Normalize(response.Metascore),
        ImdbRating = Normalize(response.ImdbRating),
        ImdbVotes = Normalize(response.ImdbVotes),
        ImdbId = response.ImdbId.Trim(),
        Type = Normalize(response.Type),
        Dvd = Normalize(response.Dvd),
        BoxOffice = Normalize(response.BoxOffice),
        Production = Normalize(response.Production)
      };
    }

    public async Task<IReadOnlyList<MediaItemResponseDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
      var apiKey = _options.ApiKey;

      if (string.IsNullOrWhiteSpace(apiKey))
      {
        throw new OmdbException("OMDb API key is not configured.", StatusCodes.Status503ServiceUnavailable);
      }

      if (string.IsNullOrWhiteSpace(query))
      {
        return [];
      }

      var response = await _httpClient.GetFromJsonAsync<OmdbSearchResponse>(
        $"?apikey={Uri.EscapeDataString(apiKey)}&s={Uri.EscapeDataString(query.Trim())}",
        cancellationToken);

      if (response is null)
      {
        return [];
      }

      if (string.Equals(response.Response, "False", StringComparison.OrdinalIgnoreCase))
      {
        if (string.Equals(response.Error, "Movie not found!", StringComparison.OrdinalIgnoreCase))
        {
          return [];
        }

        throw new OmdbException(
          response.Error ?? "OMDb search failed.",
          StatusCodes.Status502BadGateway);
      }

      if (response.Search is null)
      {
        return [];
      }

      return response.Search
        .Where(item => !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.ImdbId))
        .Select(item => new MediaItemResponseDto
        {
          MediaItemId = 0,
          Title = item.Title!.Trim(),
          Year = Normalize(item.Year),
          Poster = Normalize(item.Poster),
          ImdbId = item.ImdbId!.Trim(),
          Type = Normalize(item.Type)
        })
        .ToList();
    }

    private static string? Normalize(string? value)
    {
      if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "N/A", StringComparison.OrdinalIgnoreCase))
      {
        return null;
      }

      return value.Trim();
    }

    private static DateTime? ParseReleased(string? value)
    {
      var normalized = Normalize(value);

      if (normalized is null)
      {
        return null;
      }

      return DateTime.TryParse(normalized, out var parsed) ? parsed.Date : null;
    }

    private sealed class OmdbMovieResponse
    {
      public string? Title { get; set; }
      public string? Year { get; set; }
      public string? Rated { get; set; }
      public string? Released { get; set; }
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

      [JsonPropertyName("imdbID")]
      public string? ImdbId { get; set; }

      public string? Type { get; set; }

      [JsonPropertyName("DVD")]
      public string? Dvd { get; set; }

      public string? BoxOffice { get; set; }
      public string? Production { get; set; }
      public string? Response { get; set; }
      public string? Error { get; set; }
    }

    private sealed class OmdbSearchResponse
    {
      public List<OmdbSearchItem>? Search { get; set; }
      public string? Response { get; set; }
      public string? Error { get; set; }
    }

    private sealed class OmdbSearchItem
    {
      public string? Title { get; set; }
      public string? Year { get; set; }
      public string? Type { get; set; }
      public string? Poster { get; set; }

      [JsonPropertyName("imdbID")]
      public string? ImdbId { get; set; }
    }
  }
}
