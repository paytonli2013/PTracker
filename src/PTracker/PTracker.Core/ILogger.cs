using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public interface ILogger
    {
        void Write(string msg);
        void WriteLine(string msg);
    }
}
