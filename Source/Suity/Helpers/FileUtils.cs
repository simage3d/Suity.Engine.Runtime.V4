// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Suity.Helpers
{
    public static class FileUtils
    {
        public static string Read(string fileName)
        {

            return Read(fileName, Encoding.UTF8);

        }

        public static string Read(string fileName, Encoding encoding)
        {

            if (!File.Exists(fileName)) throw new IOException("File not found:" + fileName);

            using (FileStream fs = File.OpenRead(fileName))
            {
                StreamReader sr = new StreamReader(fs, encoding);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string data = sr.ReadToEnd();
                sr.Close();

                return data;
            }
        }

        public static void Write(string fileName, string code)
        {
            Write(fileName, code, Encoding.UTF8);
        }

        public static void Write(string fileName, string code, Encoding encoding)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            string dir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (Stream stream = File.OpenWrite(fileName))
            {
                StreamWriter writer = new StreamWriter(stream, encoding);
                writer.Write(code);
                writer.Close();
            }
        }

        public static string GetAutoNewFileName(string fileName, string extension)
        {
            string newFileName = $"{fileName}.{extension}";

            if (File.Exists(newFileName))
            {
                int index = 1;
                while (true)
                {
                    newFileName = $"{fileName}{index}.{extension}";

                    if (!File.Exists(newFileName))
                    {
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            return newFileName;
        }
    }
}
