using Microsoft.Extensions.Logging;
using PermafnotesRepositoryByApi;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Graph;
//using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace PermafnotesRepositoryByApiTest
{
    class MockLogger : ILogger
    {
        public IEnumerable<string> History
        {
            get
            {
                foreach (var h in _history)
                    yield return h;
            }
        }

        private List<string> _history = new();

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            this._history.Add($"{logLevel} {formatter(state, exception)}");
        }
    }

    public class ApiClientTests
    {
        private IConfiguration _configuration;
        private Uri _apiBaseUri = new (@"https://permafnotes-api-acfwcxhresg8bhba.japaneast-01.azurewebsites.net/");
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Users\r-h-0\source\repos\github\rhirano0715\permafnotes\src\PermafnotesRepositoryByApi\appsettings.dev.json");
            _configuration = builder.Build();

            _logger = new MockLogger();
        }

        [Test]
        public async Task TestGetNotesAsync()
        {
            var apiClient = new ApiClient(_apiBaseUri, _logger);
            var notes = await apiClient.GetNotesAsync();
            Assert.That(notes.First().Id, Is.EqualTo(1));
            Assert.That(notes.Count(), Is.AtLeast(1));
        }

        [Test]
        public async Task TestGetNoteAsync()
        {
            var apiClient = new ApiClient(_apiBaseUri, _logger);
            var notes = await apiClient.GetNoteAsync(1);
            Assert.That(notes.Id, Is.EqualTo(1));
        }
    }
}