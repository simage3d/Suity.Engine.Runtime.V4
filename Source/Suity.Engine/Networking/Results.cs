// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Suity.Networking
{
    [Serializable]
    public class HtmlResult
    {
        public string Text;

        public HtmlResult()
        {
        }
        public HtmlResult(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }

        public static implicit operator HtmlResult(string text)
        {
            return new HtmlResult(text);
        }
    }

    public class FileResult
    {
        public string FileName;

        public FileResult()
        {
        }
        public FileResult(string fileName)
        {
            FileName = fileName;
        }

        public override string ToString()
        {
            return FileName;
        }
    }

    public class StreamResult
    {
        public string ContentType;
        public Func<Stream> StreamFactory;

        public StreamResult()
        {
        }
        public StreamResult(string contentType, Func<Stream> streamFactory)
        {
            ContentType = contentType;
            StreamFactory = streamFactory;
        }

    }
}
