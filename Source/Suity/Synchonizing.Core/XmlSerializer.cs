// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Suity.NodeQuery;

namespace Suity.Synchonizing.Core
{
    public static class XmlSerializer
    {
        public static void SerializeToFile(object obj, string fileName, SyncIntent intent = SyncIntent.Serialize)
        {
            XmlNodeWriter writer = new XmlNodeWriter("Item");
            Serializer.Serialize(obj, writer, intent);
            writer.SaveToFile(fileName);
        }

        public static void DeserializeFromFile(object obj, string fileName)
        {
            var reader = XmlNodeReader.FromFile(fileName, false);
            Serializer.Deserialize(obj, reader);
        }

    }
}
