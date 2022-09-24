using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

using CsvHelper;

using PermafnotesDomain.Models;
using PermafnotesDomain.Services;

namespace PermafnotesRepositoryByMicrosoftGraph
{
    public class Repositoy : IPermafnotesRepository
    {
        private static string s_noteFileDateTimeFormat = "yyyyMMddHHmmssfffffff";
        private static string s_cacheName = "cache.json";
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");

        private GraphServiceClient _graphServiceClient;
        private ILogger<NoteService> _logger;

        private string _permafnotesBaseFolderPathFromRoot = string.Empty;
        private string _notesPathFromRoot = string.Empty;
        private string _exportDestinationFolderPathFromRoot = string.Empty;
        private string _cachePathFromRoot = string.Empty;

        public Repositoy(GraphServiceClient graphServiceClient, ILogger<NoteService> logger, string permafnotesBaseFolderPathFromRoot= @"Application/Permafnotes")
        {
            this._graphServiceClient = graphServiceClient;
            this._logger = logger;
            this._permafnotesBaseFolderPathFromRoot = permafnotesBaseFolderPathFromRoot;
            this._notesPathFromRoot = $@"{_permafnotesBaseFolderPathFromRoot}/notes";
            this._exportDestinationFolderPathFromRoot = $@"{_permafnotesBaseFolderPathFromRoot}/exports";
            this._cachePathFromRoot = $@"{_permafnotesBaseFolderPathFromRoot}/{s_cacheName}";

    }

    public async Task<IEnumerable<NoteListModel>> Add(NoteFormModel input, IEnumerable<NoteListModel>? noteRecords = null)
        {
            NoteListModel noteListModel = new(input);
            string uploadPath = $"{_notesPathFromRoot}/{noteListModel.Created.ToString(s_noteFileDateTimeFormat)}.json";
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<NoteListModel>(noteListModel, options);

            await this.PutTextFile(uploadPath, uploadText);

            if (noteRecords is null)
                return new List<NoteListModel>() { noteListModel };

            var result = noteRecords.ToList();
            result.Add(noteListModel);
            return result.OrderByDescending(x => x.Created).ToList();
        }

        public async Task<IEnumerable<NoteListModel>> FetchAll(bool onlyCache = false)
        {
            List<NoteListModel> result = await LoadCache();
            if (onlyCache)
                return result;

            IDriveItemChildrenCollectionPage children = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(_notesPathFromRoot).Children
                .Request().GetAsync();

            foreach (DriveItem child in children)
            {
                _logger.LogDebug($"Fetch start {child.Name}");
                if (result.Any(x => $"{x.Created.ToString(s_noteFileDateTimeFormat)}.json" == child.Name))
                {
                    _logger.LogDebug($"{child.Name}'s cache is exists");
                    continue;
                }

                _logger.LogWarning($"{child.Name} is not exists in cache. Loading this.");
                using MemoryStream ms = new();
                using Stream stream = await _graphServiceClient.Me.Drive.Root
                    .ItemWithPath($"{_notesPathFromRoot}/{child.Name}").Content
                    .Request().GetAsync();

                await stream.CopyToAsync(ms);

                string text = s_encoding.GetString(ms.ToArray());
                NoteListModel? model = JsonSerializer.Deserialize<NoteListModel>(text);
                if (model is null)
                {
                    continue;
                }
                result.Add(model);
            }

            await SaveCache(result);

            return result.OrderByDescending(x => x.Created).ToList();
        }

        public async Task Export(IEnumerable<NoteListModel> records)
        {
            string lineFormat = "\"{0}\"\t\"{1}\"\t\"{2}\"\t\"{3}\"\t\"{4}\"\t\"{5}\"\n";
            StringBuilder sb = new(string.Format(lineFormat, "Title", "Source", "Memo", "Tags", "Reference", "Created"));
            foreach (var record in records)
            {
                sb.Append(string.Format(lineFormat, record.Title, record.Source, record.Memo, record.Tags, record.Reference, record.Created));
            }

            string uploadPath = $"{_exportDestinationFolderPathFromRoot}/{DateTime.Now.ToString(s_noteFileDateTimeFormat)}.tsv";

            await this.PutTextFile(uploadPath, sb.ToString());
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

        public List<NoteTagModel> SelectAllTags(IEnumerable<NoteListModel> noteListModels)
        {
            List<NoteTagModel> result = new();
            foreach (var m in noteListModels)
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

        private async Task PutTextFile(string pathFromRoot, string text)
        {
            using var stream = new MemoryStream(
                s_encoding.GetBytes(text)
            );

            var uploadedItem = await _graphServiceClient.Drive.Root
                .ItemWithPath(pathFromRoot).Content.Request()
                .PutAsync<DriveItem>(stream);
        }

        private async Task<List<NoteListModel>> LoadCache()
        {
            _logger.LogDebug($"Loading cache {_cachePathFromRoot}");
            if (!(await ExistsPath(_permafnotesBaseFolderPathFromRoot, s_cacheName)))
                return new List<NoteListModel>();

            using MemoryStream ms = new();
            using Stream stream = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(_cachePathFromRoot).Content
                .Request().GetAsync();

            await stream.CopyToAsync(ms);

            string text = s_encoding.GetString(ms.ToArray());
            List<NoteListModel>? result = JsonSerializer.Deserialize<List<NoteListModel>>(text);
            if (result is null)
                return new List<NoteListModel>();

            return result.OrderByDescending(x => x.Created).ToList();
        }

        private async Task SaveCache(IEnumerable<NoteListModel> noteListModels)
        {
            _logger.LogDebug($"Saving cache {_cachePathFromRoot}");
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<IEnumerable<NoteListModel>>(noteListModels, options);

            await this.PutTextFile(_cachePathFromRoot, uploadText);
        }

        private async Task<bool> ExistsPath(string folderPath, string name)
        {
            IDriveItemChildrenCollectionPage children = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(folderPath).Children
                .Request().GetAsync();
            return children.Any(x => x.Name == name);
        }
    }
}
