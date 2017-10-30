using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例监视器。
    /// </summary>
    internal class CaseWatcher : IDisposable
    {
        #region Fields

        private readonly Dictionary<String, Action> _callbacks = new Dictionary<String, Action>();

        private readonly FileSystemWatcher _watcher;

        #endregion

        #region Constructors

        public CaseWatcher(Case @case)
        {
            Token = @case.Token;
            _watcher = CreateFilSystemWatcher(Path.GetDirectoryName(@case.Path));
        }

        #endregion

        #region Properties

        public Guid Token { get; }

        public Boolean Enable
        {
            get => _watcher.EnableRaisingEvents;
            set => _watcher.EnableRaisingEvents = value;
        }

        #endregion

        #region Methods

        #region Public

        public void Register(String path, Action callback)
        {
            lock (_callbacks)
            {
                if (_callbacks.ContainsKey(path))
                {
                    _callbacks[path] = callback;
                }
                else
                {
                    _callbacks.Add(path, callback);
                }
            }
        }

        public void Unregister(String path)
        {
            lock (_callbacks)
            {
                _callbacks.Remove(path);
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
            _callbacks.Clear();
        }

        #endregion

        #region Private

        private FileSystemWatcher CreateFilSystemWatcher(String directory)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(directory);
            watcher.BeginInit();
            watcher.Deleted += (o, e) =>
            {
                lock (_callbacks)
                {
                    if (_callbacks.ContainsKey(e.FullPath))
                    {
                        _callbacks[e.FullPath]();
                    }
                }
            };
            watcher.NotifyFilter = NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.EndInit();
            return watcher;
        }

        #endregion

        #endregion
    }
}
