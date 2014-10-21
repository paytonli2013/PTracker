using PTracker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTracker.Console
{
    class Program :IObserver<DocumentChangeSet>
    {
        static void Main(string[] args)
        {
            if(args.Length>0)
            {
                IFileManager fm = new FileManager(args[0]);
                Thread.Sleep(100);
                fm.Subscribe(new Program());
            }
        }

        #region IObserver<DocumentChangeSet> Members

        public void OnCompleted()
        {
            System.Console.WriteLine("Completed");
        }

        public void OnError(Exception error)
        {
            System.Console.WriteLine("Error:");
            System.Console.WriteLine(error);
        }

        public void OnNext(DocumentChangeSet value)
        {
            System.Console.WriteLine(value.ToString());
        }

        #endregion
    }
}
