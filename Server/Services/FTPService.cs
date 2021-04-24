using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //Sprwadzanie jakie dodano parametrt
            //send
            //get
            //do tego nazwa pliku

            if(command.Split().Length > 1)
            {
                switch (command.Split()[1])
                {
                    case "send": if (SaveFile(command)) return "File saved successfully!";
                        return "File isn't saved";
                    case "get": return GetFile(command);
                    case "list": return FileList(command);
                    default: return "Command is incorrect!\n";
                }
            }
            return "Command is incorrect!\n";
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
            return "ERROR!\n";
        }


        //To jest do przebudowania, jak klient będzie mógł wysyłać pliki

        private bool SaveFile(string command)
        {
            string[] stringArray = command.Split();
            if (stringArray.Length > 3)
            {
                string filename = _folderPath + "\\" + stringArray[2];

                string file = stringArray[3];

                Console.WriteLine(file.Length);
                Common.FTP.StringToFile(file, filename);

                return true;
            }
            return false;
        }
    }
}