namespace PermafnotesDomain.Models;

using System.ComponentModel.DataAnnotations;

public class NoteFormModel
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Source is required")]
    public string Source { get; set; } = string.Empty;

    [Required(ErrorMessage = "Memo is required")]
    public string Memo { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    [Required(ErrorMessage = "Reference is required")]
    public string Reference { get; set; } = string.Empty;
}
