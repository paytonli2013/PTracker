using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class DocumentLine
    {
        public DocumentLine(int lineNumber)
        {
            _lineNumber = lineNumber;
        }

        int _lineNumber;

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        static DocumentLine _default = new DocumentLine(0);
        public static DocumentLine Default { get {return _default;} }
    }
}
