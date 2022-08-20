using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Graph;

using CsvHelper;

using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public class NoteService
    {
        private static string s_noteFileDateTimeFormat = "yyyyMMddHHmmssfffffff";
        private static string s_permafnotesBaseFolderPathFromRoot = @"Application/Permafnotes";
        private static string s_notesPathFromRoot = $@"{s_permafnotesBaseFolderPathFromRoot}/notes";
        private static string s_exportDestinationFolderPathFromRoot = $@"{s_permafnotesBaseFolderPathFromRoot}/exports";
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");

        private GraphServiceClient _graphServiceClient;

        public NoteService(GraphServiceClient graphServiceClient)
        {
            this._graphServiceClient = graphServiceClient;
        }

        public async Task Add(NoteFormModel input)
        {
            NoteListModel noteListModel = new(input);
            string uploadPath = $"{s_notesPathFromRoot}/{DateTime.Now.ToString(s_noteFileDateTimeFormat)}.json";
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<NoteListModel>(noteListModel, options);

            await this.PutTextFile(uploadPath, uploadText);
        }

        public async Task<IEnumerable<NoteListModel>> FetchAll()
        {
            List<NoteListModel> result = new();
            IDriveItemChildrenCollectionPage children = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(s_notesPathFromRoot).Children
                .Request().GetAsync();

            foreach (DriveItem child in children)
            {
                using MemoryStream ms = new();
                using Stream stream = await _graphServiceClient.Me.Drive.Root
                    .ItemWithPath($"{s_notesPathFromRoot}/{child.Name}").Content
                    .Request().GetAsync();

                await stream.CopyToAsync(ms);

                string text = s_encoding.GetString(ms.ToArray());
                NoteListModel model = JsonSerializer.Deserialize<NoteListModel>(text);
                result.Add(model);
            }

            return result;
        }

        public async Task Export(IEnumerable<NoteListModel> records)
        {
            string lineFormat = "\"{0}\"\t\"{1}\"\t\"{2}\"\t\"{3}\"\t\"{4}\"\t\"{5}\"\n";
            StringBuilder sb = new(string.Format(lineFormat, "Title", "Source", "Memo", "Tags", "Reference", "Created"));
            foreach (var record in records)
            {
                sb.Append(string.Format(lineFormat, record.Title, record.Source, record.Memo, record.Tags, record.Reference, record.Created));
            }

            string uploadPath = $"{s_exportDestinationFolderPathFromRoot}/{DateTime.Now.ToString(s_noteFileDateTimeFormat)}.tsv";

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

        private async Task PutTextFile(string pathFromRoot, string text)
        {
            using var stream = new MemoryStream(
                s_encoding.GetBytes(text)
            );

            var uploadedItem = await _graphServiceClient.Drive.Root
                .ItemWithPath(pathFromRoot).Content.Request()
                .PutAsync<DriveItem>(stream);
        }
    }
}
