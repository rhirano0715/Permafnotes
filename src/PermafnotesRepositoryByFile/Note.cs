namespace PermafnotesRepositoryByFile;
using System.Text.RegularExpressions;

using PermafnotesDomain.Extensions;

public record Note
{
    public string Title { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Memo { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    public string Reference { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public Note() { }
}
