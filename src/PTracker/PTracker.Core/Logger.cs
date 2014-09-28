using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    class Logger :ILogger
    {
        public void Write(string msg)
        {
            //throw new NotImplementedException();
            Console.Write(msg);
        }

        public void WriteLine(string msg)
        {
           // throw new NotImplementedException();
            Console.WriteLine(msg);
        }
    }
}
