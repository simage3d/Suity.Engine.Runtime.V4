// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Suity.Helpers
{
    public static class DirectoryUtility
    {
        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo dirInfo, bool recursive = true)
        {
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                yield return fileInfo;
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                {
                    foreach (FileInfo subFileInfo in GetAllFiles(subDirInfo))
                    {
                        yield return subFileInfo;
                    }
                }
            }
        }

        public static IEnumerable<FileInfo> GetAllFiles(string dir, bool recursive = true)
        {
            return GetAllFiles(new DirectoryInfo(dir), recursive);
        }

        public static bool CopyDirectory(string sourcePath, string targetPath, bool overwrite = false, Predicate<string> filter = null)
        {
            if (!Directory.Exists(sourcePath)) return false;
            if (!overwrite && Directory.Exists(targetPath)) return false;

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            foreach (string file in Directory.GetFiles(sourcePath))
            {
                if (filter != null && !filter(file)) continue;
                File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), overwrite);
            }
            foreach (string subDir in Directory.GetDirectories(sourcePath))
            {
                if (filter != null && !filter(subDir)) continue;
                CopyDirectory(subDir, Path.Combine(targetPath, Path.GetFileName(subDir)), overwrite, filter);
            }

            return true;
        }

        public static void CleanUpDirectory(string srcPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
                Logs.LogError(e);
            }
        }

        public static void EnsureDirectory(string fullName)
        {
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
            }

            if (!Directory.Exists(fullName))
            {
                Directory.CreateDirectory(fullName);
            }
        }
    }
}
