using Microsoft.Extensions.Logging;
using PermafnotesDomain.Models;

namespace PermafnotesRepositoryByFile
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

    public class RepositoryTest
    {
        internal static readonly string s_baseDirectoryPath = @"..\..\..\TestData";

        [SetUp]
        public void Setup()
        {
        }

        private FileInfo CreateNoteFileInfo(DirectoryInfo directoryInfo, string noteFileName)
            => new(Path.Combine(directoryInfo.ToString(), noteFileName));

        public class SelectAllTagsTest
        {
            private async IAsyncEnumerable<NoteTagModel> Expected()
            {
                yield return new NoteTagModel("Tag1");
            }

            [Test]
            public async Task WhenAlreadyLoadedNotes()
            {
                // Arrange
                MockLogger logger = new MockLogger();

                DirectoryInfo baseDir = new(Path.Combine(s_baseDirectoryPath, "SelectAllTagsTest/WhenAlreadyLoadedNotes"));
                DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
                FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));
                if (cachePath.Exists)
                    cachePath.Delete();

                Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, inputAndActualDir.ToString());
                await repository.FetchAll();

                // Act
                var actual = repository.SelectAllTags();

                // Assert
                var expected = new List<NoteTagModel>()
                {
                    new NoteTagModel("Tag1")
                };

                var i = 0;
                await foreach (var a in repository.SelectAllTags())
                {
                    Assert.That(a, Is.EqualTo(expected[i++]), "SelectAllTags's return don't match the expected");
                }

                Assert.That(i, Is.EqualTo(expected.Count), "SelectAllTags's return don't match the expected");
            }

            [Test]
            public async Task WhenNotAlreadyLoadedNotesLoadNotesFromCacheBeforeSelectAllTags()
            {
                // Arrange
                MockLogger logger = new MockLogger();

                DirectoryInfo baseDir = new(Path.Combine(s_baseDirectoryPath, "SelectAllTagsTest/WhenNotAlreadyLoadedNotesLoadNotesFromCacheBeforeSelectAllTags"));
                DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
                FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));

                Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, inputAndActualDir.ToString());

                // Act
                var actual = repository.SelectAllTags();

                // Assert
                var expected = new List<NoteTagModel>()
                {
                    new NoteTagModel("Tag1")
                };

                var i = 0;
                await foreach (var a in repository.SelectAllTags())
                {
                    Assert.That(a, Is.EqualTo(expected[i++]), "SelectAllTags's return don't match the expected");
                }

                Assert.That(i, Is.EqualTo(expected.Count), "SelectAllTags's return don't match the expected");
            }
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
                MockLogger logger = new MockLogger();

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
                        Tags = new List<NoteTagModel> () { new NoteTagModel("Tag1") },
                        Reference = "Reference1",
                        Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00")
                    }
                }.OrderByDescending(x => x.Created);
                Assert.That(actual, Is.EqualTo(expected));

                IEnumerable<string> logExpected = new List<string>()
                {
                    "Debug Fetch start 202208232012552680000.json",
                    "Warning 202208232012552680000.json is not exists in cache. Loading this.",
                };
                Assert.That(logger.History, Is.EqualTo(logExpected));

                DirectoryInfo expectedDir = new(Path.Combine(baseDir.ToString(), "Expected"));
                var actualString = File.ReadAllText(CreateCacheJsonFileInfo(inputAndActualDir).FullName);
                var expectedString = File.ReadAllText(CreateCacheJsonFileInfo(expectedDir).FullName);
                Assert.That(actualString, Is.EqualTo(expectedString));
            }

            [Test]
            public async Task WhenExistsCache()
            {
                // Arrange
                MockLogger logger = new MockLogger();

                DirectoryInfo baseDir = new(Path.Combine(s_fetchAllTestBaseDirectoryPath, "WhenExistsCache"));
                DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
                FileInfo inputcachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.input.json"));
                FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));
                if (cachePath.Exists)
                    cachePath.Delete();
                inputcachePath.CopyTo(cachePath.ToString());

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
                        Tags = new List<NoteTagModel> () { new NoteTagModel("Tag1") },
                        Reference = "Reference1",
                        Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00")
                    },
                    new NoteListModel(){
                        Title = "Title2",
                        Source = "Source2",
                        Memo = "Memo2",
                        Tags = new List<NoteTagModel> () { new NoteTagModel("Tag2") },
                        Reference = "Reference2",
                        Created = DateTime.Parse("2022-11-23T20:12:55.268+09:00")
                    }
                }.OrderByDescending(x => x.Created);
                Assert.That(actual, Is.EqualTo(expected), "FetchAll's return don't match the expected");


                IEnumerable<string> logExpected = new List<string>()
                {
                    "Debug Fetch start 202208232012552680000.json",
                    "Debug 202208232012552680000.json's cache is exists",
                    "Debug Fetch start 202211232012552680000.json",
                    "Warning 202211232012552680000.json is not exists in cache. Loading this.",
                };
                Assert.That(logger.History, Is.EqualTo(logExpected));

                DirectoryInfo expectedDir = new(Path.Combine(baseDir.ToString(), "Expected"));
                var actualString = File.ReadAllText(CreateCacheJsonFileInfo(inputAndActualDir).FullName);
                var expectedString = File.ReadAllText(CreateCacheJsonFileInfo(expectedDir).FullName);
                Assert.That(actualString, Is.EqualTo(expectedString), "Result's cache.json dont't match the expected");
            }
        }

        [Test]
        public async Task AddTest()
        {
            // Arrange
            MockLogger logger = new MockLogger();

            DirectoryInfo baseDir = new(Path.Combine(s_baseDirectoryPath, "AddTest"));
            DirectoryInfo inputAndActualDir = new(Path.Combine(baseDir.ToString(), "InputAndActual"));
            FileInfo cachePath = new(Path.Combine(inputAndActualDir.ToString(), "cache.json"));
            if (cachePath.Exists)
                cachePath.Delete();

            Repositoy repository = Repositoy.CreateRepositoryUsingFileSystem(logger, inputAndActualDir.ToString());

            NoteListModel noteListModel = new()
            {
                Title = "Title1",
                Source = "Source1",
                Memo = "Memo1",
                Tags = new List<NoteTagModel>() { new NoteTagModel("Tag1")} ,
                Reference = "Reference1",
                Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00"),
            };

            // Act
            var actual = await repository.Add(noteListModel);

            // Assert
            IEnumerable<NoteListModel> expected = new List<NoteListModel>()
            {
                new NoteListModel(){
                    Title = "Title1",
                    Source = "Source1",
                    Memo = "Memo1",
                    Tags = new List<NoteTagModel>() { new NoteTagModel("Tag1")} ,
                    Reference = "Reference1",
                    Created = DateTime.Parse("2022-08-23T20:12:55.268+09:00")
                }
            }.OrderByDescending(x => x.Created);
            Assert.That(actual, Is.EqualTo(expected), "FetchAll's return don't match the expected");

            DirectoryInfo actualDir = new(Path.Combine(inputAndActualDir.ToString(), "notes"));
            DirectoryInfo expectedDir = new(Path.Combine(baseDir.ToString(), "Expected/notes"));
            string expectedNoteFileName = "202208232012552680000.json";
            var actualString = File.ReadAllText(CreateNoteFileInfo(actualDir, expectedNoteFileName).FullName);
            var expectedString = File.ReadAllText(CreateNoteFileInfo(expectedDir, expectedNoteFileName).FullName);
            Assert.That(actualString, Is.EqualTo(expectedString));
        }

    }
}