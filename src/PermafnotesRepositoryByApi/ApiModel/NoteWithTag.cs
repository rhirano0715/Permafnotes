using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermafnotesRepositoryByApi.ApiModel
{
    public class NoteWithTag
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public DateTimeOffset Created_At { get; set; }
        public DateTimeOffset? Updated_At { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
