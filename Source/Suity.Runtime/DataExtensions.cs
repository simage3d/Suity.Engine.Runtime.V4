// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Suity.Helpers.Conversion;

namespace Suity
{
    public static class DataExtensions
    {
        public static DataCollection LoadJsonFile(string fileName, bool compressed)
        {
            if (compressed)
            {
                byte[] data = File.ReadAllBytes(fileName);
                byte[] uncompressed = LZ4.LZ4Codec.Unpickle(data, 0, data.Length);

                string json = Encoding.UTF8.GetString(uncompressed);
                return DataCollection.LoadJson(json);
            }
            else
            {
                string json = null;
                using (FileStream fs = File.OpenRead(fileName))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = sr.ReadToEnd();
                }
                return DataCollection.LoadJson(json);
            }
        }
        public static void LoadFromFile(string fileName, bool compressed, DataConflictMode mode = DataConflictMode.Default)
        {
            DataStorage.CurrentLayer.LoadFromFile(fileName, compressed, mode);
        }
        public static DataCollection LoadCollectionCompressed(byte[] data, DataConflictMode mode = DataConflictMode.Default)
        {
            return DataStorage.CurrentLayer.LoadCollectionCompressed(data, mode);
        }
        public static DataCollection LoadCollectionFromFile(string fileName, bool compressed, DataConflictMode mode = DataConflictMode.Default)
        {
            return DataStorage.CurrentLayer.LoadCollectionFromFile(fileName, compressed, mode);
        }

        public static void LoadCompressed(byte[] data, DataConflictMode mode = DataConflictMode.Default)
        {
            DataStorage.CurrentLayer.LoadCompressed(data, mode);
        }

        public static void LoadCompressed(this DataLayer layer, byte[] data, DataConflictMode mode = DataConflictMode.Default)
        {
            byte[] uncompressed = LZ4.LZ4Codec.Unpickle(data, 0, data.Length);

            string json = Encoding.UTF8.GetString(uncompressed);
            layer.Load(json, mode);
        }
        public static void LoadFromFile(this DataLayer layer, string fileName, bool compressed, DataConflictMode mode = DataConflictMode.Default)
        {
            if (compressed)
            {
                byte[] data = File.ReadAllBytes(fileName);
                int size = EndianBitConverter.Little.ToInt32(data, 0);
                byte[] uncompressed = new byte[size];
                LZ4.LZ4Codec.Decode(data, 4, data.Length - 4, uncompressed, 0, size);
                string json = Encoding.UTF8.GetString(uncompressed);
                layer.Load(json, mode);
            }
            else
            {
                string json = null;
                using (FileStream fs = File.OpenRead(fileName))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = sr.ReadToEnd();
                }
                layer.Load(json, mode);
            }
        }
        public static DataCollection LoadCollectionCompressed(this DataLayer layer, byte[] data, DataConflictMode mode = DataConflictMode.Default)
        {
            byte[] uncompressed = LZ4.LZ4Codec.Unpickle(data, 0, data.Length);
            string json = Encoding.UTF8.GetString(uncompressed);
            return layer.LoadCollection(json, mode);
        }
        public static DataCollection LoadCollectionFromFile(this DataLayer layer, string fileName, bool compressed, DataConflictMode mode = DataConflictMode.Default)
        {
            DataCollection container = LoadJsonFile(fileName, compressed);
            layer.AddCollection(container, mode);
            return container;
        }
    }
}
