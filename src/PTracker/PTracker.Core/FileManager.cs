using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace PTracker.Core
{
    public class FileManager : IFileManager
    {
        string _path;
        bool _isTracking;
        Encoding _encoding;
        TaskFactory _callerTaskFactory;
        TaskFactory _defaultTaskFactory;
        ILogger _logger;
        int _lineNumber;
        volatile bool _isLoading;
        volatile bool _needUpdate;
        BlockingCollection<DocumentLine> _lines = new BlockingCollection<DocumentLine>();
        DateTime _loadTime;
        long _position = 0;
        public int Length
        {
            get { return _lineNumber; }
        }
        FileSystemWatcher _fsw;
        FileInfo _fi;
        public FileManager(string path, bool isTracking = true, Encoding encoding = null, TaskScheduler tScheduler = null, ILogger logger = null)
        {
            _logger = logger ?? new Logger();
            _callerTaskFactory = new TaskFactory(tScheduler ?? TaskScheduler.FromCurrentSynchronizationContext());
            _defaultTaskFactory = new TaskFactory(tScheduler ?? TaskScheduler.Default);

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            if (!File.Exists(path))
            {
                throw new Exception(string.Format("file ont found. path: {0}", path));
            }

            _path = path;
            _fi = new FileInfo(_path);
            _isTracking = isTracking;
            _encoding = encoding ?? Encoding.Unicode;
            _defaultTaskFactory.StartNew(Load);
            _logger.WriteLine(string.Format("FileManager constructed. path: {0}", _path));
           
            _fsw = new FileSystemWatcher(_fi.DirectoryName);
            _fsw.Filter = _fi.Name;
            _fsw.Changed += new FileSystemEventHandler(FileChanged);
            _fsw.NotifyFilter = NotifyFilters.LastWrite;
        }

        void FileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                HandleRename(e.Name);
            }

            if (_isLoading)
            {
                _needUpdate = true;
                return;
            }

            _defaultTaskFactory.StartNew(Update);
        }

        private void HandleRename(string name)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            Thread.Sleep(10);
            using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs, _encoding))
            {
                fs.Position = _position;
                while (sr.Peek() > 0)
                {
                    var line = new DocumentLine(_lineNumber);
                    line.Text = sr.ReadLine();
                    Interlocked.Increment(ref _lineNumber);
                    _lines.Add(line);
                }
                _position = fs.Position;
            }
        }

        private void Load()
        {
            try
            {
                _isLoading = true;
                _loadTime = DateTime.Now;
                _logger.WriteLine(string.Format("Loading file. path: {0}", _path));

                Interlocked.Exchange(ref _lineNumber, 0);
                using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, _encoding))
                {
                    while (sr.Peek() > 0)
                    {
                        var line = new DocumentLine(_lineNumber);
                        line.Text = sr.ReadLine();
                        Interlocked.Increment(ref _lineNumber);
                        _lines.Add(line);
                    }
                    _position = fs.Position;
                }
                if (_needUpdate)
                    Update();
                _logger.WriteLine(string.Format("Loaded. path: {0}", _path));
            }
            catch (Exception ex)
            {
                ex = new Exception(string.Format("Error occuerd when loading file: {0}.", _path), ex);
                _logger.WriteLine(string.Format("Error occuerd when loading file: {0}. error: {1}", _path, ex));
                throw ex;
            }
            finally
            {
                _isLoading = false;
            }
        }

        public string Path
        {
            get { return _path; }
        }

        public bool IsTracking
        {
            get
            {
                return _isTracking;
            }
            set
            {
                if (_isTracking != value)
                {
                    _isTracking = value;
                }
            }
        }

        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                if (_encoding != value)
                {
                    _encoding = value;
                }
            }
        }

        List<Subscribtion> _subscribtions = new List<Subscribtion>();

        public IDisposable Subscribe(IObserver<DocumentChangeSet> observer)
        {
            return Subscribe(observer, null);
        }

        public IDisposable Subscribe(IObserver<DocumentChangeSet> observer, IFilter<DocumentLine> filter)
        {
            var subscribtion = new Subscribtion(this, observer, filter);
            _subscribtions.Add(subscribtion);
            return subscribtion;
        }

        internal class Subscribtion : IDisposable
        {
            private FileManager _fileManager;

            IFilter<DocumentLine> _filter;

            public IFilter<DocumentLine> Filter
            {
                get { return _filter; }
            }

            IObserver<DocumentChangeSet> _observer;

            public IObserver<DocumentChangeSet> Observer
            {
                get { return _observer; }
            }

            public Subscribtion(FileManager fileManager, IObserver<DocumentChangeSet> observer, IFilter<DocumentLine> filter = null)
            {
                // TODO: Complete member initialization
                this._fileManager = fileManager;
                this._observer = observer;
                this._filter = filter;
            }

            bool isDisposed;
            public void Dispose()
            {
                if (isDisposed)
                    return;

                _fileManager._subscribtions.Remove(this);
                _fileManager = null;
                isDisposed = true;
            }
        }
    }
}
