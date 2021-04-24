using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FTP
    {
        public static string FileToString(string filePath)
        {
            if (File.Exists(filePath))
            {
                string file = string.Empty;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);
                        ms.Write(bytes, 0, (int)fs.Length);
                        file = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return file;
            }
            return null;
        }

        public static void StringToFile(string file, string fileName)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(file)))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    byte[] bytes = ms.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
        }
    }
}
