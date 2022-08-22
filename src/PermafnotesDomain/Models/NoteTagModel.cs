using System.Text.RegularExpressions;

namespace PermafnotesDomain.Models;

public record NoteTagModel
{
    public string Name { get; init; }

    public NoteTagModel() { }

    public NoteTagModel(string name)
    {
        this.Name = name;
    }
}
