// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.IO;

namespace Suity.Helpers
{
    public static class StreamHelper
    {
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }
        public static string ReadAllText(this Stream input)
        {
            StreamReader reader = new StreamReader(input);
            return reader.ReadToEnd();
        }
        public static byte[] ReadAllBytes(this Stream input)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                input.CopyTo(stream, 32768);
                return stream.ToArray();
            }
        }
    }
}
