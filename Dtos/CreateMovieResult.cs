namespace SceneIt.Api.Dtos
{
    public class CreateMovieResult
    {
        public required MovieResponseDto Movie { get; init; }
        public bool Created { get; init; }
    }
}
