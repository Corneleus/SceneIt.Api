namespace SceneIt.Api.Dtos
{
    public class CreateMediaItemResult
    {
        public required MediaItemResponseDto MediaItem { get; init; }
        public bool Created { get; init; }
    }
}
