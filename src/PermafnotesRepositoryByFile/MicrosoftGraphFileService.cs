using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using PermafnotesDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermafnotesRepositoryByFile
{
    internal record PermafnotesNoteFile(string Name);
    internal class MicrosoftGraphFileService : IFileService
    {
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");
        private static string s_cacheName = "cache.json";

        private GraphServiceClient _graphServiceClient;
        private ILogger _logger;
        private string _baseDirectoryPathFromRoot = @"Application/Permafnotes";

        private string NotePathFromRoot
        {
            get
            {
                return $@"{this._baseDirectoryPathFromRoot}/notes";
            }
        }
        private string CacheDirectoryFromRoot
        {
            get
            {
                return this._baseDirectoryPathFromRoot;
            }
        }

        private string CachePathFromRoot
        {
            get
            {
                return $@"{this._baseDirectoryPathFromRoot}/{s_cacheName}";
            }
        }

        private string ExportDestinationPathFromRoot
        {
            get
            {
                return $@"{this._baseDirectoryPathFromRoot}/exports";
            }
        }


        internal MicrosoftGraphFileService(GraphServiceClient graphServiceClient, ILogger logger, string baseDirectoryPathFromRoot = @"")
        {
            this._graphServiceClient = graphServiceClient;
            this._logger = logger;

            if (string.IsNullOrEmpty(baseDirectoryPathFromRoot))
                return;

            this._baseDirectoryPathFromRoot = baseDirectoryPathFromRoot;
        }

        public async Task<IEnumerable<PermafnotesNoteFile>> FetchChildren()
        {
            var response = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(this.NotePathFromRoot).Children
                .Request()
                .GetAsync();

            var result = response.Select(x => new PermafnotesNoteFile(x.Name)).ToList();
            var npr = response.NextPageRequest;
            while(npr is not null)
            {
                var next = await npr.GetAsync();
                result.AddRange(next.Select(x => new PermafnotesNoteFile(x.Name)));
                npr = next.NextPageRequest;
            }

            return result;
        }

        public async Task<string> ReadNote(string name)
        {
            return await this.ReadFile($"{this.NotePathFromRoot}/{name}");
        }

        public async Task<string> ReadCache()
        {
            if (!(await ExistsPath(this.CacheDirectoryFromRoot, s_cacheName)))
                return string.Empty;

            return await this.ReadFile(this.CachePathFromRoot);
        }

        private async Task<string> ReadFile(string path)
        {
            using MemoryStream ms = new();
            using Stream stream = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(path).Content
                .Request().GetAsync();
            await stream.CopyToAsync(ms);

            string text = s_encoding.GetString(ms.ToArray());
            return text;
        }

        public async Task WriteNote(string fileName, string text)
        {
            await this.WriteFile($@"{this.NotePathFromRoot}/{fileName}", text);
        }

        public async Task WriteCache(string text)
        {
            await this.WriteFile(this.CachePathFromRoot, text);
        }

        public async Task Export(string fileName, string text)
        {
            await this.WriteFile($@"{this.ExportDestinationPathFromRoot}/{fileName}", text);
        }

        private async Task WriteFile(string pathFromRoot, string text)
        {
            using var stream = new MemoryStream(
                s_encoding.GetBytes(text)
            );

            var uploadedItem = await _graphServiceClient.Drive.Root
                .ItemWithPath(pathFromRoot).Content.Request()
                .PutAsync<DriveItem>(stream);
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
