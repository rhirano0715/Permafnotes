namespace PermafnotesDomain.Models;
using System.Text.RegularExpressions;

using PermafnotesDomain.Extensions;

public class NoteListModel
{
    private static Regex s_regexTagDelimiter = new(@",");
    private static IEnumerable<string> s_csvColumns = new List<string>()
    {
        "Title", "Source", "Memo", "Tags", "Reference", "Created",
    };

    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Memo { get; set; } = string.Empty;

    public IEnumerable<NoteTagModel> Tags { get; set; } = new List<NoteTagModel>();

    public string Reference { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.MinValue;

    public NoteListModel() { }

    public NoteListModel(NoteFormModel noteFormModel)
    {
        this.Title = noteFormModel.Title;
        this.Source = noteFormModel.Source;
        this.Memo = noteFormModel.Memo;
        this.Tags = noteFormModel.ConvertTagsToString();
        this.Reference = noteFormModel.Reference;
        if (noteFormModel.Created == DateTime.MinValue)
        {
            this.Created = DateTime.Now;
        }
        else
        {
            this.Created = noteFormModel.Created;
        }
    }

    public IEnumerable<NoteTagModel> SplitTags()
    {
        return Tags;
    }

    public NoteFormModel ToNoteFormModel()
    {
        return new NoteFormModel()
        { 
            Title = this.Title,
            Source = this.Source,
            Memo = this.Memo,
            Tags = Tags.Select(x => x.Name).ToList(),
            Reference = this.Reference,
            Created = this.Created,
        };
    }

    public static string BuildCsvHeader(string delimiter)
        => StringExtensions.Join(delimiter, s_csvColumns);
    

    public string ToCsvLine(string delimiter)
        => StringExtensions.Join(
                delimiter,
                new string[] {
                    this.Title.EscapeDoubleQuote(),
                    this.Source.EscapeDoubleQuote(),
                    this.Memo.EscapeDoubleQuote(),
                    string.Join(",", this.Tags.Select(x => x.Name.EscapeDoubleQuote())),
                    this.Reference.EscapeDoubleQuote(),
                    this.Created.ToString().EscapeDoubleQuote()
                }
            );

    public bool HasUrlReference()
        => Reference.StartsWith("http://") || Reference.StartsWith("https://");

    public override bool Equals(object? obj)
    {
        if (obj is not NoteListModel other)
            return false;

        return Title == other.Title &&
               Source == other.Source &&
               Memo == other.Memo &&
               Tags.SequenceEqual(other.Tags) &&
               Reference == other.Reference &&
               Created == other.Created;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title, Source, Memo, Tags, Reference, Created);
    }
}
