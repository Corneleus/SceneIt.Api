namespace SceneIt.Api.Services
{
  public sealed class OmdbException : Exception
  {
    public int StatusCode { get; }

    public OmdbException(string message, int statusCode)
      : base(message)
    {
      StatusCode = statusCode;
    }
  }
}
