using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Graph;

using PermafnotesDomain.Models;

namespace PermafnotesDomain.Services
{
    public class NoteService
    {
        private static string s_noteFileDateTimeFormat = "yyyyMMddHHmmssfffffff";
        private static string s_notesPathFromRoot = @"Application/Permafnotes/notes";
        private static Encoding s_encoding = Encoding.GetEncoding("UTF-8");

        private GraphServiceClient _graphServiceClient;

        public NoteService(GraphServiceClient graphServiceClient)
        {
            this._graphServiceClient = graphServiceClient;
        }

        public async Task Add(NoteListModel noteListModel)
        {
            string uploadPath = $"{s_notesPathFromRoot}/{DateTime.Now.ToString(s_noteFileDateTimeFormat)}.json";
            JsonSerializerOptions options = new()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.
                Create(System.Text.Unicode.UnicodeRanges.All)
            };

            string uploadText = JsonSerializer.Serialize<NoteListModel>(noteListModel, options);

            var stream = new MemoryStream(
                s_encoding.GetBytes(uploadText)
            );

            var uploadedItem = await _graphServiceClient.Drive.Root
                .ItemWithPath(uploadPath).Content.Request()
                .PutAsync<DriveItem>(stream);
        }

        public async Task<IEnumerable<NoteListModel>> FetchAll()
        {
            List<NoteListModel> result = new();
            IDriveItemChildrenCollectionPage children = await _graphServiceClient.Me.Drive.Root
                .ItemWithPath(s_notesPathFromRoot).Children
                .Request().GetAsync();

            foreach (DriveItem child in children)
            {
                MemoryStream ms = new();
                Stream stream = await _graphServiceClient.Me.Drive.Root
                    .ItemWithPath($"{s_notesPathFromRoot}/{child.Name}").Content
                    .Request().GetAsync();

                await stream.CopyToAsync(ms);

                string text = s_encoding.GetString(ms.ToArray());
                NoteListModel model = JsonSerializer.Deserialize<NoteListModel>(text);
                result.Add(model);
            }

            return result;
        }
    }
}
