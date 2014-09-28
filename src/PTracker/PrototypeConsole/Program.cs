using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace PrototypeConsole
{
    class Program
    {
        static BlockingCollection<string> list = new BlockingCollection<string>();
        static FileSystemWatcher _fsw;
        static string path = "test.txt";
        static long position = 0;
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                while (sr.Peek() > 0)
                {
                    var str = sr.ReadLine();
                    list.Add(str);
                    Console.WriteLine(str);
                }
                position = fs.Position;
            }
            sw.Stop();
            FileInfo _fi = new FileInfo(path);
            _fsw = new FileSystemWatcher(_fi.DirectoryName);
            _fsw.Filter = _fi.Name;
            _fsw.Changed += new FileSystemEventHandler(FileChanged);
            _fsw.NotifyFilter = NotifyFilters.LastWrite;

            //Start monitoring.
            _fsw.EnableRaisingEvents = true;
            Console.Read();
        }

        static void FileChanged(object sender, FileSystemEventArgs e)
        {
            _fsw.EnableRaisingEvents = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Thread.Sleep(10);
            using (FileStream fs = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                fs.Position = position;
                while (sr.Peek() > 0)
                {
                    var str = sr.ReadLine();
                    if (!string.IsNullOrEmpty(str) && str != Environment.NewLine)
                    {
                        list.Add(str);
                        Console.WriteLine(str);
                    }
                }
                position = fs.Position;
            }
            sw.Stop();
            _fsw.EnableRaisingEvents = true;
        }

    }
}
