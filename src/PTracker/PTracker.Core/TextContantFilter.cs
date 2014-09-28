using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class TextContantFilter : IFilter<DocumentLine>
    {
        string _containText;

        public string ContainedText
        {
            get { return _containText; }
        }

        public TextContantFilter(string contantedText)
        {
            this._containText = contantedText;
        }

        #region IFilter<DocumentLine> Members

        public bool Test(DocumentLine obj)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Contains {0}", _containText);
        }

        #endregion
    }
}
