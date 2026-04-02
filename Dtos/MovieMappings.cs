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
    }
}
