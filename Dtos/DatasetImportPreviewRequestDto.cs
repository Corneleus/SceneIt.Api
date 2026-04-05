using System.ComponentModel.DataAnnotations;

namespace SceneIt.Api.Dtos
{
  public class DatasetImportPreviewRequestDto
  {
    [Required]
    public string DatasetPath { get; set; } = string.Empty;

    public List<string> TitleTypes { get; set; } = [];
    public bool ExcludeAdult { get; set; } = true;
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public bool SkipAlreadyImported { get; set; } = true;
    public bool SkipAlreadyQueued { get; set; } = true;
  }
}
