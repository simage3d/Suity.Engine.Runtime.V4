// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using ComputerBeacon.Json;
using Suity.Json;
using Suity.NodeQuery;

namespace Suity.NodeQuery
{
    public static class InfoNodeExtensions
    {
        public static void WriteInfo(this INodeWriter writer, string name, string text, Action<INodeWriter> action = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            writer.AddElement(name, w =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    w.SetAttribute("text", text);
                }
                action?.Invoke(w);
            });
        }
        public static void WriteInfo(this INodeWriter writer, string name, string text, string preview, string info = null, TextStatus status = TextStatus.Normal, string icon = null, Action<INodeWriter> action = null)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            writer.AddElement(name, w =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    w.SetAttribute("text", text);
                }
                if (!string.IsNullOrEmpty(preview))
                {
                    w.SetAttribute("preview", preview);
                }
                if (!string.IsNullOrEmpty(info))
                {
                    w.SetAttribute("info", info);
                }
                if (status != TextStatus.Normal)
                {
                    w.SetAttribute("status", status.ToString());
                }
                if (!string.IsNullOrEmpty(icon))
                {
                    w.SetAttribute("icon", icon);
                }
                action?.Invoke(w);
            });
        }


        public static void WriteObjectInfo(this INodeWriter writer, object obj, string name = null)
        {
            if (obj == null)
            {
                writer.WriteInfo(name, name, "null");
            }
            else if (obj is string)
            {
                writer.WriteInfo(name, name, $"\"{obj}\"");
            }
            if (ObjectType.GetClassTypeInfo(obj.GetType()) != null)
            {
                JsonDataWriter jsonWriter = new JsonDataWriter();
                ObjectType.WriteObject(jsonWriter, obj);
                WriteJsonInfo(writer, jsonWriter.Value, name);
            }
            else
            {
                writer.WriteInfo(name, name, obj.ToString());
            }
        }
        public static void WriteJsonInfo(this INodeWriter writer, object obj, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "node";
            }

            if (obj is JsonObject jsonObjects)
            {
                WriteJsonObject(writer, jsonObjects, name);
            }
            else if (obj is JsonArray jsonArraies)
            {
                WriteJsonArray(writer, jsonArraies, name);
            }
            else
            {
                if (obj == null)
                {
                    writer.WriteInfo(name, name, "null", icon: "*CoreIcon|Value");
                }
                else if (obj is string)
                {
                    writer.WriteInfo(name, name, $"\"{obj}\"", icon: "*CoreIcon|Value");
                }
                else
                {
                    writer.WriteInfo(name, name, obj.ToString(), icon: "*CoreIcon|Value");
                }
            }
        }
        public static void WriteJsonObject(this INodeWriter writer, JsonObject obj, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "node";
            }

            var text = obj["@text"];
            if (text == null)
            {
                text = obj["@type"];
            }

            writer.WriteInfo(name, name, text.ToString(), icon: "*CoreIcon|Structure", action: childWriter => 
            {
                foreach (var childObj in obj)
                {
                    string childName = childObj.Key ?? "node";
                    if (childName == "@type" || childName == "@text")
                    {
                        continue;
                    }

                    try
                    {
                        WriteJsonInfo(childWriter, childObj.Value, childName);
                    }
                    catch (Exception)
                    {
                        writer.WriteInfo(childName, childName, "Error", status: TextStatus.Error, icon: "*CoreIcon|Value");
                    }
                }
            });
        }
        public static void WriteJsonArray(this INodeWriter writer, JsonArray array, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "node";
            }

            string text = $"[{array.Count} items]";

            writer.WriteInfo(name, name, text, icon: "*CoreIcon|Array", action: childWriter => 
            {
                for (int i = 0; i < array.Count; i++)
                {
                    try
                    {
                        WriteJsonInfo(childWriter, array[i], $"[{i}]");
                    }
                    catch (Exception)
                    {
                        writer.WriteInfo($"[{i}]", $"[{i}]", "Error", status: TextStatus.Error, icon: "*CoreIcon|Value");
                    }
                }
            });
        }
    }
}
