using System;
using System.IO;

namespace Server
{
    class FilesListener : IListener
    {
        private string _folderName;
        private FileSystemWatcher watcher;
        private CommunicatorD _onConnect;

        public FilesListener(string folderpath)
        {
            _folderName = folderpath;
            if(!Directory.Exists(_folderName)) Directory.CreateDirectory(_folderName);
            watcher = new FileSystemWatcher(_folderName);
        }
        public void Start(CommunicatorD onConnect)
        {
            _onConnect = onConnect;
            Console.WriteLine("[Files Listener] - START!");
            string[] fileList = Directory.GetFiles(_folderName);

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;
            watcher.Filter = "*.txt";
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"[Files Listener] Changed: {e.FullPath}");
            if (e.ChangeType == WatcherChangeTypes.Changed) _onConnect(new FilesCommunicator(e.FullPath));
        }

        public void Stop() => watcher.Dispose();
    }

    class FilesCommunicator : ICommunicator
    {
        private string _fileName;

        public FilesCommunicator(string fileName) => _fileName = fileName;
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            string _oldFileName = _fileName;
            using (var sr = new StreamReader(_fileName))
            {
                using (var sw = new StreamWriter(_fileName.Replace(".txt",".odp")))
                {
                    while (!sr.EndOfStream) sw.WriteLine(onCommand(sr.ReadLine()));
                }
            }

            if (!Directory.Exists("archive")) Directory.CreateDirectory("archive");
            File.Move(_oldFileName, $"archive\\ {DateTime.Now.ToFileTime()}_{Path.GetFileName(_oldFileName)}");
        }

        public void Stop() => Console.WriteLine("[Files] - STOP!");
    }
}
