using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTracker.Core
{
    public interface IFileManager : IObservable<DocumentChangeSet>
    {
        string Path { get; }

        bool IsTracking { get; set; }

        Encoding Encoding { get; set; }
    }
}
