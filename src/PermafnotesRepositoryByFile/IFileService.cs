using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermafnotesRepositoryByFile
{
    interface IFileService
    {
        internal Task<IEnumerable<PermafnotesNoteFile>> FetchChildren();
        internal Task<string> ReadNote(string name);
        internal Task<string> ReadCache();
        internal Task WriteNote(string fileName, string text);
        internal Task WriteCache(string text);
        internal Task Export(string fileName, string text);
    }
}
