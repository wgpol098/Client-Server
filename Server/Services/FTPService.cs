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
                    default: return "Command is incorrect!\n";
                }
            }
            return "Command is incorrect!\n";
        }

        private bool SaveFile(string command)
        {
            if (command.Split().Length > 3)
            {
                string[] stringArray = command.Split();

                string filename = stringArray[2];
                //Sprwadzanie czy taki plik istnieje 
                //Jeśli jest odpowiednia opcja w komendzie to nadpisujemy plik
                //Na razie będę nadpisyuwał plik pomimo wszystko -- dobre rozwiązanie do testów

                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    writer.Write(command);
                }

                return true;
            }
            return false;
        }
    }
}