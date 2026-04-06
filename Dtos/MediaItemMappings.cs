using SceneIt.Api.Models;

namespace SceneIt.Api.Dtos
{
    public static class MediaItemMappings
    {
        public static MediaItemResponseDto ToMediaItemResponseDto(this MediaItem mediaItem)
        {
            return new MediaItemResponseDto
            {
                MediaItemId = mediaItem.MediaItemId,
                Title = mediaItem.Title,
                Year = mediaItem.Year,
                Rated = mediaItem.Rated,
                Released = mediaItem.Released,
                Runtime = mediaItem.Runtime,
                Genre = mediaItem.Genre,
                Director = mediaItem.Director,
                Writer = mediaItem.Writer,
                Actors = mediaItem.Actors,
                Plot = mediaItem.Plot,
                Language = mediaItem.Language,
                Country = mediaItem.Country,
                Awards = mediaItem.Awards,
                Poster = mediaItem.Poster,
                Metascore = mediaItem.Metascore,
                ImdbRating = mediaItem.ImdbRating,
                ImdbVotes = mediaItem.ImdbVotes,
                ImdbId = mediaItem.ImdbId,
                Type = mediaItem.Type,
                Dvd = mediaItem.Dvd,
                BoxOffice = mediaItem.BoxOffice,
                Production = mediaItem.Production
            };
        }

        public static MediaItem ToEntity(this CreateMediaItemRequestDto mediaItem)
        {
            return new MediaItem
            {
                Title = mediaItem.Title.Trim(),
                Year = mediaItem.Year,
                Rated = mediaItem.Rated,
                Released = mediaItem.Released,
                Runtime = mediaItem.Runtime,
                Genre = mediaItem.Genre,
                Director = mediaItem.Director,
                Writer = mediaItem.Writer,
                Actors = mediaItem.Actors,
                Plot = mediaItem.Plot,
                Language = mediaItem.Language,
                Country = mediaItem.Country,
                Awards = mediaItem.Awards,
                Poster = mediaItem.Poster,
                Metascore = mediaItem.Metascore,
                ImdbRating = mediaItem.ImdbRating,
                ImdbVotes = mediaItem.ImdbVotes,
                ImdbId = mediaItem.ImdbId.Trim(),
                Type = mediaItem.Type,
                Dvd = mediaItem.Dvd,
                BoxOffice = mediaItem.BoxOffice,
                Production = mediaItem.Production
            };
        }

        public static void ApplyToEntity(this CreateMediaItemRequestDto mediaItem, MediaItem entity)
        {
            entity.Title = mediaItem.Title.Trim();
            entity.Year = mediaItem.Year;
            entity.Rated = mediaItem.Rated;
            entity.Released = mediaItem.Released;
            entity.Runtime = mediaItem.Runtime;
            entity.Genre = mediaItem.Genre;
            entity.Director = mediaItem.Director;
            entity.Writer = mediaItem.Writer;
            entity.Actors = mediaItem.Actors;
            entity.Plot = mediaItem.Plot;
            entity.Language = mediaItem.Language;
            entity.Country = mediaItem.Country;
            entity.Awards = mediaItem.Awards;
            entity.Poster = mediaItem.Poster;
            entity.Metascore = mediaItem.Metascore;
            entity.ImdbRating = mediaItem.ImdbRating;
            entity.ImdbVotes = mediaItem.ImdbVotes;
            entity.ImdbId = mediaItem.ImdbId.Trim();
            entity.Type = mediaItem.Type;
            entity.Dvd = mediaItem.Dvd;
            entity.BoxOffice = mediaItem.BoxOffice;
            entity.Production = mediaItem.Production;
        }

        public static MediaItemResponseDto ToMediaItemResponseDto(this CreateMediaItemRequestDto mediaItem)
        {
            return new MediaItemResponseDto
            {
                MediaItemId = 0,
                Title = mediaItem.Title.Trim(),
                Year = mediaItem.Year,
                Rated = mediaItem.Rated,
                Released = mediaItem.Released,
                Runtime = mediaItem.Runtime,
                Genre = mediaItem.Genre,
                Director = mediaItem.Director,
                Writer = mediaItem.Writer,
                Actors = mediaItem.Actors,
                Plot = mediaItem.Plot,
                Language = mediaItem.Language,
                Country = mediaItem.Country,
                Awards = mediaItem.Awards,
                Poster = mediaItem.Poster,
                Metascore = mediaItem.Metascore,
                ImdbRating = mediaItem.ImdbRating,
                ImdbVotes = mediaItem.ImdbVotes,
                ImdbId = mediaItem.ImdbId.Trim(),
                Type = mediaItem.Type,
                Dvd = mediaItem.Dvd,
                BoxOffice = mediaItem.BoxOffice,
                Production = mediaItem.Production
            };
        }
    }
}
