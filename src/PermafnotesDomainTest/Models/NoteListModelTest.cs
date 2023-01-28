namespace PermafnotesDomain.Models
{
    public class NoteListModelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BuildCsvHeaderTest()
        {
            Assert.That(NoteListModel.BuildCsvHeader("\t"), Is.EqualTo("\"Title\"\t\"Source\"\t\"Memo\"\t\"Tags\"\t\"Reference\"\t\"Created\""));
        }

        [Test]
        public void ToCsvLineTest()
        {
            NoteListModel model = new NoteListModel() { 
                Title = "Title01",
                Source = "Source01",
                Memo = "Memo01",
                Tags = "Tag01,Tag02,Tag03,Tag04",
                Reference = "Reference01",
                Created = DateTime.MinValue,
            };

            string expected = "\"Title01\"\t\"Source01\"\t\"Memo01\"\t\"Tag01,Tag02,Tag03,Tag04\"\t\"Reference01\"\t\"1/1/0001 12:00:00 AM\"";
            string actual = model.ToCsvLine("\t");
            Assert.That(actual, Is.EqualTo(expected));
        }
        public class SplitTagsTest
        {
            [Test]
            public void WhenTagsIsEmptyThenReturnEmpty()
            {
                NoteListModel model = new NoteListModel();
                IEnumerable<NoteTagModel> expected = new List<NoteTagModel>();
                Assert.That(model.SplitTags(), Is.EqualTo(expected));
            }

            [Test]
            public void WhenTagsHasOneTagThenReturnEnumerableNoteTagModel()
            {
                NoteListModel model = new NoteListModel() { Tags = "Tag01"};
                IEnumerable<NoteTagModel> expected = new List<NoteTagModel>() { new NoteTagModel("Tag01") };
                Assert.That(model.SplitTags(), Is.EqualTo(expected));
            }
            [Test]
            public void WhenTagsHasMoreThanOneTagThenReturnEnumerableNoteTagModel()
            {
                NoteListModel model = new NoteListModel() { Tags = "Tag01, Tag02" };
                IEnumerable<NoteTagModel> expected = new List<NoteTagModel>() {
                    new NoteTagModel("Tag01"),
                    new NoteTagModel("Tag02"),
                };
                Assert.That(model.SplitTags(), Is.EqualTo(expected));
            }
            [Test]
            public void IgnoreEmptyElements()
            {
                NoteListModel model = new NoteListModel() { Tags = "Tag01, , Tag02" };
                IEnumerable<NoteTagModel> expected = new List<NoteTagModel>() {
                    new NoteTagModel("Tag01"),
                    new NoteTagModel("Tag02"),
                };
                Assert.That(model.SplitTags(), Is.EqualTo(expected));
            }
            [Test]
            public void DeleteSpacesBeforeAndAfterTag()
            {
                NoteListModel model = new NoteListModel() { Tags = " Tag01, Tag02,Tag03 , Tag04 " };
                IEnumerable<NoteTagModel> expected = new List<NoteTagModel>() {
                    new NoteTagModel("Tag01"),
                    new NoteTagModel("Tag02"),
                    new NoteTagModel("Tag03"),
                    new NoteTagModel("Tag04"),
                };
                Assert.That(model.SplitTags(), Is.EqualTo(expected));
            }
        }
    }
}
