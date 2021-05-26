using System;
using System.IO;

namespace Server
{
    //TODO: To może być obsługiwane za pomocą usługi FTP. Można wysyłać i odbierać dane
    class FilesListener : IListener
    {
        private string _folderName;
        private FileSystemWatcher watcher;
        private CommunicatorD _onConnect;

        public FilesListener(string folderpath)
        {
            _folderName = folderpath;
            //Sprwadzanie czy folder istnieje - jeśli tak to dopiero działaj
            // jeśli nie to stwórz folder
            watcher = new FileSystemWatcher(folderpath);
        }
        public void Start(CommunicatorD onConnect)
        {
            _onConnect = onConnect;
            Console.WriteLine("[Files Listener] - START!");
            string[] fileList = Directory.GetFiles(_folderName);

            for(int i = 0; i < fileList.Length; i++)
            {
                Console.WriteLine(fileList[i]);
            }

            watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Changed += OnChanged;
            watcher.Filter = "*.txt";

            watcher.EnableRaisingEvents = true;
        }

        //Kiedy plik się zmienia, bądź zostaje utworzony
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            
            Console.WriteLine($"Changed: {e.FullPath}");

            if (e.ChangeType == WatcherChangeTypes.Changed) _onConnect(new FilesCommunicator(e.FullPath));
        }

        public void Stop()
        {
            watcher.Dispose();
        }
    }

    class FilesCommunicator : ICommunicator
    {
        private string _fileName;

        public FilesCommunicator(string fileName) => _fileName = fileName;
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            //Czytanie pliku linia po linii i przetwarzanie komend i tworzenie nowego pliku

            using (var sr = new StreamReader(_fileName))
            {
                using (var sw = new StreamWriter(_fileName.Replace(".txt",".odp")))
                {
                    while (!sr.EndOfStream)
                    {
                        Console.Write(onCommand(sr.ReadLine()));
                        sw.Write(onCommand(sr.ReadLine()));
                    }
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine("[Files] - STOP!");
        }
    }
}
