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
        TaskFactory _defaultTaskFactory;
        ILogger _logger;
        int _lineNumber;
        volatile bool _isLoading;
        volatile bool _needUpdate;
        volatile bool _isUpdating;
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
            _isUpdating = true;

            try
            {
                var linesAdded = new List<DocumentLine>();
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
                        linesAdded.Add(line);
                    }
                    _position = fs.Position;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine(ex.Message);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void NotifySubscriber(IEnumerable<DocumentLine> lines)
        {
            if (lines == null || !lines.Any())
                return;
            
            List<DocumentChange> changes = new List<DocumentChange>();

            foreach (var line in lines)
            {
                changes.Add(DocumentChange.NewLine(line));
            }

            var changeSet = new DocumentChangeSet(changes);

            foreach (var subscribtion in _subscribtions)
            {
                subscribtion.Observer.OnNext(changeSet);
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

        List<Subscription> _subscribtions = new List<Subscription>();

        public IDisposable Subscribe(IObserver<DocumentChangeSet> observer)
        {
            return Subscribe(observer, null);
        }

        public IDisposable Subscribe(IObserver<DocumentChangeSet> observer, IFilter<DocumentLine> filter)
        {
            var subscribtion = new Subscription(this, observer, filter);
            _subscribtions.Add(subscribtion);
            return subscribtion;
        }

        internal class Subscription : IDisposable
        {
            TaskFactory _callerTaskFactory;

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

            public Subscription(FileManager fileManager, IObserver<DocumentChangeSet> observer, IFilter<DocumentLine> filter = null,TaskScheduler tScheduler=null)
            {
                // TODO: Complete member initialization
                this._fileManager = fileManager;
                this._observer = observer;
                this._filter = filter;

                _callerTaskFactory = new TaskFactory(tScheduler ?? TaskScheduler.FromCurrentSynchronizationContext());
            }

            public void OnNext(DocumentChangeSet changeSet)
            {
                Action<object> action = (status)=>
                {
                    Tuple<IObserver<DocumentChangeSet>, DocumentChangeSet> payload = (Tuple<IObserver<DocumentChangeSet>, DocumentChangeSet>)status;
                    payload.Item1.OnNext(payload.Item2);
                };
                _callerTaskFactory.StartNew(action, Tuple.Create<IObserver<DocumentChangeSet>, DocumentChangeSet>(_observer,changeSet));
            }

            bool isDisposed;
            public void Dispose()
            {
                if (isDisposed)
                    return;

                _fileManager._subscribtions.Remove(this);
                _fileManager = null;
                _observer = null;
                _filter = null;
                isDisposed = true;
            }
        }
    }
}
