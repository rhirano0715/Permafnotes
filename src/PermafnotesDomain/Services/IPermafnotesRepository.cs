using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public interface IPermafnotesRepository
    {
        Task<IEnumerable<NoteListModel>> Add(NoteFormModel input);
        Task Export(IEnumerable<NoteListModel> records);
        Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false);
        Task Import(byte[] inputBuffers);
        IEnumerable<NoteTagModel> SelectAllTags();
    }
}