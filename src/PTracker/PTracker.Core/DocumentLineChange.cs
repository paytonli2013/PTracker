using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public class DocumentChange
    {
        public enum ActionEnum {Unknown, NewLine, ReplaceLine, DeleteLine,ResetDocument}

        private ActionEnum _action = ActionEnum.Unknown;
        public ActionEnum Action
        {
            get { return _action; }
        }

        private DocumentLine _line = DocumentLine.Default;

        public DocumentLine Line
        {
            get { return _line; }
            private set { _line = value; }
        }

        public DocumentChange(ActionEnum action)
        {
            _action = action;
        }

        public static DocumentChange NewLine(DocumentLine newLine)
        {
            var change = new DocumentChange(ActionEnum.NewLine) 
            {
                Line = newLine
            };

            return change;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}",_action,Line.LineNumber,Line.Text);
        }
    }
}
