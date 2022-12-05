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
    internal class FileSystemService : IFileService
    {
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");
        private static string s_cacheName = "cache.json";

        private ILogger _logger;
        private DirectoryInfo _baseDirectory;
        private FileInfo _cache;
        private DirectoryInfo _noteDirectory;

        private FileInfo Cache
        {
            get
            {
                if (this._cache == null)
                {
                    this._cache = new FileInfo(Path.Combine(_baseDirectory.FullName, s_cacheName));
                }

                return this._cache;
            }
        }

        private DirectoryInfo NoteDirectory
        {
            get
            {
                if (this._noteDirectory == null)
                {
                    this._noteDirectory = new DirectoryInfo(Path.Combine(_baseDirectory.FullName, "notes"));
                }

                return this._noteDirectory;
            }
        }

        internal FileSystemService(ILogger logger, string baseDirectoryPathFromRoot)
        {
            this._logger = logger;
            this._baseDirectory = new(baseDirectoryPathFromRoot);

            // TODO: Validate
        }

        Task IFileService.Export(string fileName, string text)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PermafnotesNoteFile>> FetchChildren()
            => this.NoteDirectory.GetFiles()
                .Select(x => new PermafnotesNoteFile(x.Name))
                .ToList();

        public async Task<string> ReadCache()
        {
            if (!this.Cache.Exists)
            {
                await Task.CompletedTask;
                return string.Empty;
            }

            return ReadFileAsString(this.Cache.FullName);
        }

        public async Task<string> ReadNote(string name)
        {
            return this.ReadFileAsString(Path.Combine(this.NoteDirectory.FullName, name));
        }

        public async Task WriteCache(string text)
        {
            using var writer = new StreamWriter(this.Cache.FullName, append: false, encoding: s_encoding);
            writer.Write(text);
        }

        public async Task WriteNote(string fileName, string text)
        {
            if (!this.NoteDirectory.Exists)
                this.NoteDirectory.Create();
            FileInfo outputFile = new (Path.Combine(this.NoteDirectory.ToString(), fileName));
            using var writer = new StreamWriter(outputFile.ToString(), append: false, encoding: s_encoding);
            writer.Write(text);
        }

        private string ReadFileAsString(string filePath)
        {
            using var reader = new StreamReader(filePath, encoding: s_encoding);
            return reader.ReadToEnd();
        }
    }
}
