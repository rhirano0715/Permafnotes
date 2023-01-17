namespace PermafnotesDomain.Models;

using AntDesign;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using PermafnotesDomain.Extensions;

public record NoteListModel
{
    private static Regex s_regexTagDelimiter = new(@",");
    private static IEnumerable<string> s_csvColumns = new List<string>()
    {
        "Title", "Source", "Memo", "Tags", "Reference", "Created",
    };

    public string Title { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Memo { get; set; } = string.Empty;

    public string Tags { get; set; } = string.Empty;

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
        return s_regexTagDelimiter.Split(Tags)
            .Select(x => new NoteTagModel(x.Trim()))
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            ;
    }

    public NoteFormModel ToNoteFormModel()
    {
        return new NoteFormModel()
        { 
            Title = this.Title,
            Source = this.Source,
            Memo = this.Memo,
            Tags = s_regexTagDelimiter.Split(this.Tags).Select(x => x.Trim()).ToList(),
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
                    this.Tags.EscapeDoubleQuote(),
                    this.Reference.EscapeDoubleQuote(),
                    this.Created.ToString().EscapeDoubleQuote()
                }
            );
}
