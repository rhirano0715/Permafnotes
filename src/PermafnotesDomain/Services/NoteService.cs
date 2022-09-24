using System.Text;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public class NoteService : IPermafnotesRepository
    {
        private IPermafnotesRepository _repository;
        private ILogger<NoteService> _logger;

        public NoteService(IPermafnotesRepository repository, ILogger<NoteService> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }

        public async Task<IEnumerable<NoteListModel>> Add(NoteFormModel input, IEnumerable<NoteListModel>? noteRecords = null)
        {
            return await this._repository.Add(input, noteRecords);
        }

        public async Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false)
        {
            return await this._repository.FetchAll(onlyCache);
        }

        public async Task Export(IEnumerable<NoteListModel> records)
        {
            await this._repository.Export(records);
        }

        public async Task Import(byte[] inputBuffers)
        {
            await this._repository.Import(inputBuffers);
        }

        public List<NoteTagModel> SelectAllTags(IEnumerable<NoteListModel> noteListModels)
        {
            return this._repository.SelectAllTags(noteListModels);
        }
    }
}
