using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public interface IPermafnotesRepository
    {
        Task<IEnumerable<NoteListModel>> Add(NoteListModel input);
        Task Export(IEnumerable<NoteListModel> records, string delimiter = "\t");
        Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false);
        Task Import(byte[] inputBuffers);
        IAsyncEnumerable<NoteTagModel> SelectAllTags();
    }
}