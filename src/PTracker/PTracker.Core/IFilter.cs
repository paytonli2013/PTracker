using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public interface IFilter<T> where T : class
    {
        bool Test(T obj);
    }
}
