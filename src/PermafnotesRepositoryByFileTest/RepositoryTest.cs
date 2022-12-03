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
        internal static readonly string s_baseDirectoryPath = @"..\..\..\TestData";

        [SetUp]
        public void Setup()
        {
        }

        public class FetchAllTest
        {
            private static readonly string s_fetchAllTestBaseDirectoryPath = Path.Combine(s_baseDirectoryPath, "FetchAllTest");

            [SetUp]
            public void Setup()
            {
            }

            private FileInfo CreateCacheJsonFileInfo(DirectoryInfo direcotryInfo)
                => new(Path.Combine(direcotryInfo.ToString(), "cache.json"));

            [Test]
            public async Task WhenNotExistsCache()
            {
                // Arrange
                ILogger logger = new MockLogger();

                DirectoryInfo baseDir = new(Path.Combine(s_fetchAllTestBaseDirectoryPath, "WhenNotExistsCache"));
                DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
                FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));
                if (cachePath.Exists)
                    cachePath.Delete();

                Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, inputAndActualDir.ToString());

                // Act
                var actual = await repository.FetchAll();

                // Assert
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
                Assert.That(actual, Is.EqualTo(expected));

                DirectoryInfo expectedDir = new(Path.Combine(baseDir.ToString(), "Expected"));
                FileAssert.AreEqual(CreateCacheJsonFileInfo(expectedDir), CreateCacheJsonFileInfo(inputAndActualDir));
            }

            [Test]
            public async Task WhenExistsCache()
            {
                // Arrange
                ILogger logger = new MockLogger();

                DirectoryInfo baseDir = new(Path.Combine(s_fetchAllTestBaseDirectoryPath, "WhenExistsCache"));
                DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
                FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));
                if (cachePath.Exists)
                    cachePath.Delete();

                Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, inputAndActualDir.ToString());

                // Act
                var actual = await repository.FetchAll();

                // Assert
                IEnumerable<NoteListModel> expected = new List<NoteListModel>()
                {
                    new NoteListModel(){
                        Title = "Title1",
                        Source = "Source1",
                        Memo = "Memo1",
                        Tags = "Tag1",
                        Reference = "Reference1",
                        Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00")
                    },
                    new NoteListModel(){
                        Title = "Title2",
                        Source = "Source2",
                        Memo = "Memo2",
                        Tags = "Tag2",
                        Reference = "Reference2",
                        Created = DateTime.Parse("2022-11-23T20:12:55.268+09:00")
                    }
                }.OrderByDescending(x => x.Created);
                Assert.That(actual, Is.EqualTo(expected), "FetchAll's return don't match the expected");

                DirectoryInfo expectedDir = new(Path.Combine(baseDir.ToString(), "Expected"));
                FileAssert.AreEqual(CreateCacheJsonFileInfo(expectedDir), CreateCacheJsonFileInfo(inputAndActualDir), "Result's cache.json dont't match the expected");
            }
        }
    }
}