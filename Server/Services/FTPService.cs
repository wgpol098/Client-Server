using System.IO;

namespace Server.Services
{
    class FTPService : IServiceModule
    {
        private string _folderPath;
        public FTPService(string folderPath)
        {
            if(!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            _folderPath = folderPath;
        }

        public string AnswerCommand(string command)
        {
            if(command.Split().Length > 1)
            {
                switch (command.Split()[1])
                {
                    case "send": if (SaveFile(command)) return "File saved successfully!";
                        return "File isn't saved";
                    case "get": return GetFile(command);
                    case "list": return FileList(command);
                    case "help": return Help();
                    default: return "Command is incorrect!\n";
                }
            }
            return "Command is incorrect!\n";
        }

        private string Help()
        {
            return
                "This is ftp help\n" +
                "ftp send -filename - send file\n" +
                "ftp get -filename - download file\n" +
                "ftp list - show available files in server";
        }

        private string FileList(string command)
        {
            string[] fileList = Directory.GetFiles(_folderPath);
            return "FileList:\n" + string.Join("\n", fileList) + "\n";
        }

        private string GetFile(string command)
        {
            string[] stringArray = command.Split();
            if(stringArray.Length > 2)
            {
                string filename = stringArray[2];
                string file = Common.FTP.FileToString(_folderPath + "\\" + filename);
                if(file == null) return "File not exists\n";
                return file;
            }
            return "File is not available!\n";
        }

        private bool SaveFile(string command)
        {
            string[] stringArray = command.Split();
            if (stringArray.Length > 3)
            {
                Common.FTP.StringToFile(stringArray[3], _folderPath + "\\" + stringArray[2]);
                return true;
            }
            return false;
        }
    }
}