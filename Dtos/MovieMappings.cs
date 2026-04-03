using SceneIt.Api.Models;

namespace SceneIt.Api.Dtos
{
    public static class MovieMappings
    {
        public static MovieResponseDto ToResponseDto(this Movie movie)
        {
            return new MovieResponseDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Year = movie.Year,
                Rated = movie.Rated,
                Released = movie.Released,
                Runtime = movie.Runtime,
                Genre = movie.Genre,
                Director = movie.Director,
                Writer = movie.Writer,
                Actors = movie.Actors,
                Plot = movie.Plot,
                Language = movie.Language,
                Country = movie.Country,
                Awards = movie.Awards,
                Poster = movie.Poster,
                Metascore = movie.Metascore,
                ImdbRating = movie.ImdbRating,
                ImdbVotes = movie.ImdbVotes,
                ImdbId = movie.ImdbId,
                Type = movie.Type,
                Dvd = movie.Dvd,
                BoxOffice = movie.BoxOffice,
                Production = movie.Production
            };
        }

        public static Movie ToEntity(this CreateMovieRequestDto movie)
        {
            return new Movie
            {
                Title = movie.Title.Trim(),
                Year = movie.Year,
                Rated = movie.Rated,
                Released = movie.Released,
                Runtime = movie.Runtime,
                Genre = movie.Genre,
                Director = movie.Director,
                Writer = movie.Writer,
                Actors = movie.Actors,
                Plot = movie.Plot,
                Language = movie.Language,
                Country = movie.Country,
                Awards = movie.Awards,
                Poster = movie.Poster,
                Metascore = movie.Metascore,
                ImdbRating = movie.ImdbRating,
                ImdbVotes = movie.ImdbVotes,
                ImdbId = movie.ImdbId.Trim(),
                Type = movie.Type,
                Dvd = movie.Dvd,
                BoxOffice = movie.BoxOffice,
                Production = movie.Production
            };
        }

        public static void ApplyToEntity(this CreateMovieRequestDto movie, Movie entity)
        {
            entity.Title = movie.Title.Trim();
            entity.Year = movie.Year;
            entity.Rated = movie.Rated;
            entity.Released = movie.Released;
            entity.Runtime = movie.Runtime;
            entity.Genre = movie.Genre;
            entity.Director = movie.Director;
            entity.Writer = movie.Writer;
            entity.Actors = movie.Actors;
            entity.Plot = movie.Plot;
            entity.Language = movie.Language;
            entity.Country = movie.Country;
            entity.Awards = movie.Awards;
            entity.Poster = movie.Poster;
            entity.Metascore = movie.Metascore;
            entity.ImdbRating = movie.ImdbRating;
            entity.ImdbVotes = movie.ImdbVotes;
            entity.ImdbId = movie.ImdbId.Trim();
            entity.Type = movie.Type;
            entity.Dvd = movie.Dvd;
            entity.BoxOffice = movie.BoxOffice;
            entity.Production = movie.Production;
        }

        public static MovieResponseDto ToResponseDto(this CreateMovieRequestDto movie)
        {
            return new MovieResponseDto
            {
                MovieId = 0,
                Title = movie.Title.Trim(),
                Year = movie.Year,
                Rated = movie.Rated,
                Released = movie.Released,
                Runtime = movie.Runtime,
                Genre = movie.Genre,
                Director = movie.Director,
                Writer = movie.Writer,
                Actors = movie.Actors,
                Plot = movie.Plot,
                Language = movie.Language,
                Country = movie.Country,
                Awards = movie.Awards,
                Poster = movie.Poster,
                Metascore = movie.Metascore,
                ImdbRating = movie.ImdbRating,
                ImdbVotes = movie.ImdbVotes,
                ImdbId = movie.ImdbId.Trim(),
                Type = movie.Type,
                Dvd = movie.Dvd,
                BoxOffice = movie.BoxOffice,
                Production = movie.Production
            };
        }
    }
}
