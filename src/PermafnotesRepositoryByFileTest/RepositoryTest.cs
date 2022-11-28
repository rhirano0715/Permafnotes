using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using PermafnotesDomain.Models;
using System;

namespace PermafnotesRepositoryByFile
{
    class MockLogger : ILogger
    {
        private List<string> _history = new();

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            this._history.Add($"{logLevel} {formatter(state, exception)}");
        }
    }

    public class RepositoryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        public class FetchAllTest
        {
            [Test]
            public async Task WhenNotExistsCache()
            {
                // TODO: dynamic
                string baseDirectoryPath = @"..\..\..\TestData\Input";
                string cachePath = Path.Combine(baseDirectoryPath, "cache.json");
                if (File.Exists(cachePath))
                    File.Delete(cachePath);

                ILogger logger = new MockLogger();
                Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, baseDirectoryPath);

                IEnumerable<NoteListModel> expected = new List<NoteListModel>()
                {
                    new NoteListModel(){
                        Title = "Title1",
                        Source = "Source1",
                        Memo = "Memo1",
                        Tags = "Tag1",
                        Reference = "Reference1",
                        Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00")
                    }
                }.OrderByDescending(x => x.Created);

                var actual = await repository.FetchAll();

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}