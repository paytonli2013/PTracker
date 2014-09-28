using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class DocumentChangeSet
    {
        public DocumentChangeSet(IEnumerable<DocumentLine> newLines)
        {
            _newLines = newLines;
        }

        IEnumerable<DocumentLine> _newLines;

        public IEnumerable<DocumentLine> NewLines
        {
            get { return _newLines; }
        }
    }
}
