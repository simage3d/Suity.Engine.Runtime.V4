// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.IO;

namespace Suity.Helpers
{
    public static class PathUtility
    {
        readonly static char[] Slashes = new[] { '\\', '/' };

        public static string MakeRalativePath(this string path, string basePath)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(basePath))
            {
                return path;
            }

            if (!basePath.EndsWith("/") && !basePath.EndsWith("\\"))
            {
                basePath += '/';
            }

            if (!Uri.TryCreate(path, UriKind.Absolute, out Uri pathUri))
            {
                return string.Empty;
            }

            Uri baseUri = new Uri(basePath);

            // 不可以使用 IsBaseOf，因为可以解析为 ../
            try
            {
                Uri resultUri = baseUri.MakeRelativeUri(pathUri);
                return Uri.UnescapeDataString(resultUri.ToString());
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string MakeFullPath(this string path, string basePath)
        {
            if (string.IsNullOrEmpty(basePath)) return path;

            if (!basePath.EndsWith("/") && !basePath.EndsWith("\\"))
            {
                basePath += '/';
            }

            Uri baseUri = new Uri(basePath);
            Uri resultUri = new Uri(baseUri, path);
            string result = Uri.UnescapeDataString(resultUri.ToString());

            if (result.StartsWith("file:///"))
            {
                return result.Substring(8, result.Length - 8);
            }
            else
            {
                return result;
            }
        }


        public static string NormalizeDirectoryName(this string path)
        {
            path = path.Replace('/', '\\');
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }

            return path;
        }

        public static string GetPathTerminal(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.TrimEnd('\\', '/');

            int index = path.LastIndexOfAny(Slashes);
            if (index >= 0)
            {
                return path.Substring(index + 1, path.Length - index - 1);
            }
            else
            {
                return path;
            }
        }
        public static string GetPathRoot(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.TrimStart('\\', '/');

            int index = path.IndexOfAny(Slashes);
            if (index >= 0)
            {
                return path.Substring(0, index);
            }
            else
            {
                return path;
            }
        }

        public static string GetParentPath(this string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.TrimEnd('\\', '/');

            int index = path.LastIndexOfAny(Slashes);
            if (index >= 0)
            {
                return path.Substring(0, index);
            }
            else
            {
                return string.Empty;
            }
        }


        public static string GetPathLowId(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            //转反斜杠,去末端斜杠,转小写
            return path.Replace('\\', '/').TrimEnd('\\', '/').ToLower();
        }
        public static string GetPathId(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            //转反斜杠,去末端斜杠,转小写
            return path.Replace('\\', '/').TrimEnd('\\', '/');
        }


        public static string PathAppend(this string path, string suffixPath)
        {
            return Path.Combine(path, suffixPath);
        }
        public static string PathPreppend(this string path, string prefixPath)
        {
            return Path.Combine(prefixPath, path);
        }

        public static bool IsPathValid(this string path)
        {
            bool valid = false;
            try { new FileInfo(path); valid = true; } catch (Exception) { }
            return valid;
        }
        public static bool FileExists(this string path)
        {
            return File.Exists(path);
        }
        public static bool DirectoryExists(this string path)
        {
            return Directory.Exists(path);
        }
        public static bool FileOrDirectoryExists(this string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        /// <summary>
        /// 忽略大小写比较字符串是否相等
        /// </summary>
        /// <param name="str"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IgnoreCaseEquals(this string str, string other)
        {
            return string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略大小写判断文件名的扩展名是否指定的扩展名
        /// </summary>
        /// <param name="str"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool FileExtensionEquals(this string str, string extension)
        {
            string ext = Path.GetExtension(str);
            return string.Equals(ext, extension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 移除文件名的扩展名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveExtension(this string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.Length > 0)
            {
                return path.RemoveFromLast(ext.Length);
            }
            else
            {
                return path;
            }
        }
    }
}
