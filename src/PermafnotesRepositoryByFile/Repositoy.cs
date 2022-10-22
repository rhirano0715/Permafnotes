using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

using CsvHelper;

using PermafnotesDomain.Models;
using PermafnotesDomain.Services;

namespace PermafnotesRepositoryByFile
{
    public class Repositoy : IPermafnotesRepository
    {
        private static string s_noteFileDateTimeFormat = "yyyyMMddHHmmssfffffff";
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");

        private IFileService _fileService;
        private ILogger<NoteService> _logger;

        private IEnumerable<NoteListModel> _noteRecords = new List<NoteListModel>();

        public static Repositoy CreateRepository(GraphServiceClient graphServiceClient, ILogger<NoteService> logger)
        {
            return new Repositoy(new MicrosoftGraphFile(graphServiceClient, logger), logger);
        }

        internal Repositoy(IFileService fileService, ILogger<NoteService> logger)
        {
            this._fileService = fileService;
            this._logger = logger;
        }

        public async Task<IEnumerable<NoteListModel>> Add(NoteFormModel input)
        {
            NoteListModel noteListModel = new(input);

            await PutNoteListModel(noteListModel);

            this.AddToNoteRecords(noteListModel);

            return this.OrderByDescendingNoteRecords();
        }

        public async Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false)
        {
            this._noteRecords = await LoadCache();
            if (onlyCache)
                return this.OrderByDescendingNoteRecords();

            var children = await this._fileService.FetchChildren();
            List<NoteListModel> result = this._noteRecords.ToList();
            foreach (PermafnotesNoteFile child in children)
            {
                _logger.LogDebug($"Fetch start {child.Name}");
                if (result.Any(x => $"{x.Created.ToString(s_noteFileDateTimeFormat)}.json" == child.Name))
                {
                    _logger.LogDebug($"{child.Name}'s cache is exists");
                    continue;
                }

                _logger.LogWarning($"{child.Name} is not exists in cache. Loading this.");
                string text = await this._fileService.ReadNote(child.Name);
                NoteListModel? model = JsonSerializer.Deserialize<NoteListModel>(text);
                if (model is null)
                {
                    continue;
                }
                result.Add(model);
            }

            await SaveCache(result);
            this._noteRecords = result;

            return this.OrderByDescendingNoteRecords();
        }

        public async Task Export(IEnumerable<NoteListModel> records)
        {
            string lineFormat = "\"{0}\"\t\"{1}\"\t\"{2}\"\t\"{3}\"\t\"{4}\"\t\"{5}\"\n";
            StringBuilder sb = new(string.Format(lineFormat, "Title", "Source", "Memo", "Tags", "Reference", "Created"));
            foreach (var record in records)
            {
                sb.Append(string.Format(lineFormat, record.Title, record.Source, record.Memo, record.Tags, record.Reference, record.Created));
            }

            string uploadName = $"{DateTime.Now.ToString(s_noteFileDateTimeFormat)}.tsv";

            await this._fileService.Export(uploadName, sb.ToString());
        }

        public async Task Import(byte[] inputBuffers)
        {
            using MemoryStream ms = new(inputBuffers);
            using StreamReader sr = new(ms, s_encoding);
            using CsvReader csv = new(sr, CultureInfo.InvariantCulture);

            foreach (var record in csv.GetRecords<NoteFormModel>())
            {
                await this.Add(record);
            }
        }

        public IEnumerable<NoteTagModel> SelectAllTags()
        {
            List<NoteTagModel> result = new();
            foreach (var m in this._noteRecords)
            {
                foreach (var t in m.SplitTags())
                {
                    if (result.Any(x => x.Name == t.Name))
                        continue;
                    result.Add(t);
                }
            }

            return result;
        }

        private IEnumerable<NoteListModel> OrderByDescendingNoteRecords()
        {
            return this._noteRecords.OrderByDescending(x => x.Created);
        }

        private async Task PutNoteListModel(NoteListModel noteListModel)
        {
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<NoteListModel>(noteListModel, options);
            string uploadName = $"{noteListModel.Created.ToString(s_noteFileDateTimeFormat)}.json";

            await this._fileService.WriteNote(uploadName, uploadText);
        }

        private void AddToNoteRecords(NoteListModel noteListModel)
        {
            if (this._noteRecords is null)
            {
                this._noteRecords = new List<NoteListModel>();
            }

            var result = this._noteRecords.ToList();
            result.Add(noteListModel);
            this._noteRecords = result;
        }

        private async Task<IEnumerable<NoteListModel>> LoadCache()
        {
            string text = await this._fileService.ReadCache();
            if (string.IsNullOrEmpty(text))
                return this.OrderByDescendingNoteRecords();

            List<NoteListModel>? result = JsonSerializer.Deserialize<List<NoteListModel>>(text);
            if (result is null)
                return this.OrderByDescendingNoteRecords();

            return result;
        }

        private async Task SaveCache(IEnumerable<NoteListModel> noteListModels)
        {
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<IEnumerable<NoteListModel>>(noteListModels, options);

            await this._fileService.WriteCache(uploadText);
        }
    }
}
