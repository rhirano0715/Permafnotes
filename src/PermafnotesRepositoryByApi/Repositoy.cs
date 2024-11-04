using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using PermafnotesDomain.Models;
using PermafnotesDomain.Services;
using PermafnotesRepositoryByApi.ApiModel;

namespace PermafnotesRepositoryByApi
{
    public class Repositoy : IPermafnotesRepository
    {
        private readonly ILogger _logger;
        private readonly ApiClient _apiClient;
        private IEnumerable<NoteListModel> _cacheNoteListModels;
        private IEnumerable<NoteTagModel> _cacheNoteTagModels;

        private Repositoy(ApiClient apiClient, ILogger logger)
        {
            this._apiClient = apiClient;
            this._logger = logger;
        }

        public static Repositoy CreateRepository(Uri baseUri, ILogger logger)
        {
            var apiClient = new ApiClient(baseUri, logger);
            return new Repositoy(apiClient, logger);
        }

        #region CREATE

        public async Task<IEnumerable<NoteListModel>> Add(NoteListModel input)
        {
            NoteWithTag newNote = new NoteWithTag
            {
                Title = input.Title,
                Reference = input.Reference,
                Source = input.Source,
                Memo = input.Memo,
                Tags = SelectTags(input.Tags).ToList()
            };

            await _apiClient.CreateNoteAsync(newNote);

            return await FetchAll();
        }
        #endregion

        #region READ
        public async Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false)
        {
            var notes = await _apiClient.GetNotesAsync();

            _cacheNoteListModels = notes.Select(note => new NoteListModel
            {
                Id = note.Id,
                Title = note.Title,
                Reference = note.Reference,
                Source = note.Source,
                Memo = note.Memo,
                Created = note.Created_At.UtcDateTime,
                Tags = note.Tags.Select(tag => new NoteTagModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Description = tag.Description
                }).ToList()
            }).OrderByDescending(x => x.Id);

            return _cacheNoteListModels;
        }

        private IEnumerable<Tag> SelectTags(IEnumerable<NoteTagModel> tags)
        {
            //SelectAllTags().GetAsyncEnumerator().MoveNextAsync().GetAwaiter().GetResult();

            List<Tag> result = new ();
            foreach (var tag in tags)
            {
                if (_cacheNoteTagModels.Any(x => x.Name == tag.Name))
                {
                    var exists = _cacheNoteTagModels.FirstOrDefault(x => x.Name == tag.Name);
                    if (!(exists is null))
                    {
                        result.Add(new Tag()
                        {
                            Id = exists.Id,
                            Name = exists.Name,
                            Description = exists.Description
                        });

                        continue;
                    }
                }

                result.Add(new Tag
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Description = tag.Description
                });
            }

            return result;
        }

        public async IAsyncEnumerable<NoteTagModel> SelectAllTags()
        {
            var result = await _apiClient.GetTagsAsync();
            _cacheNoteTagModels = result.Select(tag => new NoteTagModel
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description
            });

            foreach (var tag in _cacheNoteTagModels)
            {
                yield return tag;
            }
        }
        #endregion

        #region UPDATE
        public async Task<IEnumerable<NoteListModel>> Update(NoteListModel input)
        {
            NoteWithTag newNote = new NoteWithTag
            {
                Id = input.Id,
                Title = input.Title,
                Reference = input.Reference,
                Source = input.Source,
                Memo = input.Memo,
                Tags = SelectTags(input.Tags).ToList()
            };

            await _apiClient.UpdateNoteAsync(input.Id, newNote);

            return await FetchAll();
        }
        #endregion

        #region OTHER
        public Task Export(IEnumerable<NoteListModel> records, string delimiter = "\t")
        {
            throw new NotImplementedException();
        }


        public Task Import(byte[] inputBuffers)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
