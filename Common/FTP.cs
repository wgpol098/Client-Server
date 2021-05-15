using System;
using System.IO;

namespace Common
{
    public class FTP
    {
        public static string FileToString(string filePath)
        {
            if (File.Exists(filePath))
            {
                string file = string.Empty;
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    file = Convert.ToBase64String(bytes);
                }
                return file;
            }
            return null;
        }

        public static void StringToFile(string file, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                byte[] bytes = Convert.FromBase64String(file);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }
    }
}
