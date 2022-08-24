namespace PermafnotesDomain.Models;

using System.ComponentModel;
using System.Text.RegularExpressions;

public record NoteListModel
{
    public string Title { get; set; }

    public string Source { get; set; }

    public string Memo { get; set; }

    public string Tags { get; set; }

    public string Reference { get; set; }

    public DateTime Created { get; set; }

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
