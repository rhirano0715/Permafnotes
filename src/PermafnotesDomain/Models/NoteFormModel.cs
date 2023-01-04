namespace PermafnotesDomain.Models;

// TODO: move to permafnotes
using System.ComponentModel.DataAnnotations;

public class NoteFormModel
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source is required")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Memo is required")]
    public string Memo { get; set; } = string.Empty;

    // TODO: implement delete duplicate item and empty item
    public IEnumerable<string> Tags { get; set; } = new List<string>();

    [Required(ErrorMessage = "Reference is required")]
    public string Reference { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public override string ToString()
        => $"{Title}, {Source}, {Memo}, {ConvertTagsToString()}, {Reference}";

    internal string ConvertTagsToString()
    {
        return string.Join(',', Tags.Where(x => !string.IsNullOrEmpty(x)));
    }
}
