namespace PermafnotesDomain.Models;

public record NoteListModel
{
    public string Title { get; init; }

    public string Source { get; init; }

    public string Memo { get; init; }

    public string Tags { get; init; }

    public string Reference { get; init; }

    public DateTime Created { get; init; }

    public NoteListModel() { }

    public NoteListModel(NoteFormModel noteFormModel)
    {
        this.Title = noteFormModel.Title;
        this.Source = noteFormModel.Source;
        this.Memo = noteFormModel.Memo;
        this.Tags = noteFormModel.Tags;
        this.Reference = noteFormModel.Reference;
        this.Created = DateTime.Now;
    }
}
