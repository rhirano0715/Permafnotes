using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PermafnotesRepositoryByApi.ApiModel;
using System;
using Microsoft.Extensions.Logging;

namespace PermafnotesRepositoryByApi
{
    public class ApiClient
    {
        // TODO: Impl Auth
        private readonly Uri _baseUri;
        private readonly ILogger _logger;

        public ApiClient(Uri baseUri, ILogger logger)
        {
            _baseUri = baseUri;
            _logger = logger;
        }

        public async Task<IEnumerable<NoteWithTag>> GetNotesAsync()
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = _baseUri };
                var request = new HttpRequestMessage(HttpMethod.Get, "/Note");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<IEnumerable<NoteWithTag>>();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notes.");
                throw;
            }
        }

        public async Task<NoteWithTag> GetNoteAsync(long id)
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = _baseUri };

                var request = new HttpRequestMessage(HttpMethod.Get, $"/Note/{id}");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<NoteWithTag>();
                return data;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to get note. ID={id}");
                throw;
            }
        }

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = _baseUri };
                var request = new HttpRequestMessage(HttpMethod.Get, "/Tag");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<IEnumerable<Tag>>();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notes.");
                throw;
            }
        }

        public async Task<NoteWithTag> CreateNoteAsync(NoteWithTag newNote)
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = _baseUri };
                var request = new HttpRequestMessage(HttpMethod.Post, "/Note")
                {
                    Content = JsonContent.Create(newNote)
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<NoteWithTag>();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create note.");
                throw;
            }
        }

        public async Task<NoteWithTag> UpdateNoteAsync(long id, NoteWithTag updatedNote)
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = _baseUri };
                var request = new HttpRequestMessage(HttpMethod.Put, $"/Note/{id}")
                {
                    Content = JsonContent.Create(updatedNote)
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<NoteWithTag>();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update note. ID={id}");
                throw;
            }
        }
    }
}
