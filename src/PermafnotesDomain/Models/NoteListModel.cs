namespace PermafnotesDomain.Models;

using System.ComponentModel;
using System.Text.RegularExpressions;

public record NoteListModel
{
    public string Title { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Memo { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

    public string Reference { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    private Regex _regexTagDelimiter = new(@",");

    public NoteListModel() { }

    public NoteListModel(NoteFormModel noteFormModel)
    {
        this.Title = noteFormModel.Title;
        this.Source = noteFormModel.Source;
        this.Memo = noteFormModel.Memo;
        this.Tags = noteFormModel.ConvertTagsToString();
        this.Reference = noteFormModel.Reference;
        this.Created = DateTime.Now;
    }

    public override string ToString()
        => $"{Title}, {Source}, {Memo}, {Tags}, {Reference}, {Created}";

    public IEnumerable<NoteTagModel> SplitTags()
    {
        return _regexTagDelimiter.Split(Tags).Select(x => new NoteTagModel(x.Trim()));
    }
}
