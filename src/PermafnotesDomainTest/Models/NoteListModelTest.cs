namespace PermafnotesDomain.Models
{
    public class NoteListModelTest
    {
        [SetUp]
        public void Setup()
        {
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
        }
    }
}
