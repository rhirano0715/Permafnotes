using System.Text;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public class NoteService
    {
        private IPermafnotesRepository _repository;
        private ILogger<NoteService> _logger;

        public NoteService(IPermafnotesRepository repository, ILogger<NoteService> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }

        public async Task<IEnumerable<NoteListModel>> Add(NoteFormModel input)
            => await this._repository.Add(new NoteListModel(input));

        public async Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false)
            => await this._repository.FetchAll(onlyCache);

        public async Task Export(IEnumerable<NoteListModel> records)
            => await this._repository.Export(records);

        public async Task Import(byte[] inputBuffers)
            => await this._repository.Import(inputBuffers);

        public async Task<IEnumerable<NoteTagModel>> SelectAllTags()
            => await this._repository.SelectAllTags();
    }
}
