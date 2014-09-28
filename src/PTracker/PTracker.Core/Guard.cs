using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public static class Guard
    {
        public static void Check<T>(T t, string name) where T :class
        {
            if (t == null)
                throw new ArgumentNullException(name);
        }
    }
}
