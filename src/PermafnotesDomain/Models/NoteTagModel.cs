using System.Text.RegularExpressions;

namespace PermafnotesDomain.Models;

public record NoteTagModel
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public NoteTagModel() { }

    public NoteTagModel(string name)
    {
        this.Name = name;
    }

    public NoteTagModel(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }
}
