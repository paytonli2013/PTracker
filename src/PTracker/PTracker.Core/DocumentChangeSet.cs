using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class DocumentChangeSet
    {
        public DocumentChangeSet(IEnumerable<DocumentChange> changes)
        {
            _changes = changes;
        }

        IEnumerable<DocumentChange> _changes;

        public IEnumerable<DocumentChange> NewLines
        {
            get { return _changes; }
        }

        public override string ToString()
        {
            return ToString(_changes);
        }

        static string ToString(IEnumerable<DocumentChange> changes)
        {
            if (changes == null || !changes.Any())
                return string.Empty;
               
            StringBuilder sb = new StringBuilder();
            foreach (var change in changes)
            {
                sb.AppendLine(change.ToString());
            }
            return sb.ToString();
        }
    }
}
