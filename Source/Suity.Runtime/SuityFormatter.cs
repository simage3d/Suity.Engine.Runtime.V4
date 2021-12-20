// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Suity.Collections;
using Suity.NodeQuery;
using Suity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity
{
    /// <summary>
    /// Suity内建格式器
    /// </summary>
    public class SuityFormatter : IRuntimeInitialize
    {
        private static bool _init;

        public static ClassTypeInfo ByteArrayTypeInfo { get; private set; }
        public static ClassTypeInfo TypeInfoDescriptorTypeInfo { get; private set; }
        public static ClassTypeInfo FieldDescriptorTypeInfo { get; private set; }
        public static ClassTypeInfo EmptyResultTypeInfo { get; private set; }
        public static ClassTypeInfo ErrorResultTypeInfo { get; private set; }
        public static ClassTypeInfo ButtonValueTypeInfo { get; private set; }
        public static ClassTypeInfo LabelValueTypeInfo { get; private set; }
        public static ClassTypeInfo ObjectPrototypeTypeInfo { get; private set; }
#if !BRIDGE
        public static ClassTypeInfo RawNodeTypeInfo { get; private set; }
#endif

        public SuityFormatter()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (_init)
            {
                return;
            }
            _init = true;

            ByteArrayTypeInfo = Suity.ObjectType.RegisterClassType(typeof(ByteArray), "ByteArray", null, PacketFormats.Default, ReadByteArray, WriteByteArray, CloneByteArray, ObjectEqualsByteArray, ExchangeByteArray);
            TypeInfoDescriptorTypeInfo = Suity.ObjectType.RegisterClassType(typeof(TypeInfoDescriptor), "TypeInfoDescriptor", null, PacketFormats.Default, ReadTypeInfoDescriptor, WriteTypeInfoDescriptor, CloneTypeInfoDescriptor, ObjectEqualsTypeInfoDescriptor, ExchangeTypeInfoDescriptor);
            FieldDescriptorTypeInfo = Suity.ObjectType.RegisterClassType(typeof(FieldDescriptor), "FieldDescriptor", null, PacketFormats.Default, ReadFieldDescriptor, WriteFieldDescriptor, CloneFieldDescriptor, ObjectEqualsFieldDescriptor, ExchangeFieldDescriptor);
            EmptyResultTypeInfo = Suity.ObjectType.RegisterClassType(typeof(EmptyResult), "EmptyResult", null, PacketFormats.Default, ReadEmptyResult, WriteEmptyResult, CloneEmptyResult, ObjectEqualsEmptyResult, ExchangeEmptyResult);
            ErrorResultTypeInfo = Suity.ObjectType.RegisterClassType(typeof(ErrorResult), "ErrorResult", null, PacketFormats.Default, ReadErrorResult, WriteErrorResult, CloneErrorResult, ObjectEqualsErrorResult, ExchangeErrorResult);
            ButtonValueTypeInfo = Suity.ObjectType.RegisterClassType(typeof(ButtonValue), "ButtonValue", null, PacketFormats.Default, ReadButtonValue, WriteButtonValue, CloneButtonValue, ObjectEqualsButtonValue, ExchangeButtonValue);
            LabelValueTypeInfo = Suity.ObjectType.RegisterClassType(typeof(LabelValue), "LabelValue", null, PacketFormats.Default, ReadLabelValue, WriteLabelValue, CloneLabelValue, ObjectEqualsLabelValue, ExchangeLabelValue);
            ObjectPrototypeTypeInfo = Suity.ObjectType.RegisterClassType(typeof(ObjectPrototype), "ObjectPrototype", null, PacketFormats.Default, ReadObjectPrototype, WriteObjectPrototype, CloneObjectPrototype, ObjectEqualsObjectPrototype, ExchangeObjectPrototype);

#if !BRIDGE
            RawNodeTypeInfo = Suity.ObjectType.RegisterClassType(typeof(RawNode), "RawNode", null, PacketFormats.Default, ReadRawNode, WriteRawNode, CloneRawNode, CompareRawNode, ExchangeRawNode);
#endif
        }

        #region ByteArray
        public static ByteArray ReadByteArray(Suity.IDataReader reader)
        {
            return new ByteArray(reader.ReadBytes());
        }
        public static void WriteByteArray(Suity.IDataWriter writer, object value)
        {
            ByteArray b = (ByteArray)value;

            b.EnsureBufferOffset();
            writer.WriteBytes(b.Buffer, 0, b.Offset);
        }
        public static ByteArray CloneByteArray(object source, object target, bool autoNew)
        {
            if (source == null)
            {
                if (autoNew)
                {
                    return new ByteArray();
                }
                else
                {
                    return null;
                }
            }
            ByteArray bSource = (ByteArray)source;
            var bTarget = target != null ? (ByteArray)target : new ByteArray();
            
            bSource.EnsureBufferOffset();
            bTarget.Offset = bSource.Offset;
            bTarget.EnsureBufferOffset();

            Array.Copy(bSource.Buffer, bTarget.Buffer, bSource.Offset);

            return bTarget;
        }
        public static bool ObjectEqualsByteArray(object obj1, object obj2)
        {
            ByteArray v1 = (ByteArray)obj1;
            ByteArray v2 = (ByteArray)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (v1.Offset != v2.Offset)
                return false;

            return ArrayHelper.ArrayEquals(v1.Buffer, v2.Buffer, 0, v1.Offset);
        }
        public static void ExchangeByteArray(object obj, IExchange ex)
        {
        }
        #endregion

        #region TypeInfoDescriptor
        public static object ReadTypeInfoDescriptor(Suity.IDataReader reader)
        {
            var obj = new TypeInfoDescriptor
            {
                Kind = Suity.ObjectType.ReadString(reader.Node("Kind")),
                Description = Suity.ObjectType.ReadString(reader.Node("Description")),
                Icon = Suity.ObjectType.ReadString(reader.Node("Icon")),
                Category = Suity.ObjectType.ReadString(reader.Node("Category")),
                Side = Suity.ObjectType.ReadString(reader.Node("Side")),
                IsValueType = Suity.ObjectType.ReadBoolean(reader.Node("IsValueType"))
            };
            foreach (var childReader in reader.Nodes("Fields"))
            {
                obj.Fields.Add((FieldDescriptor)Suity.ObjectType.Read(childReader, ReadFieldDescriptor));
            }
            return obj;
        }
        public static void WriteTypeInfoDescriptor(Suity.IDataWriter writer, object value)
        {
            TypeInfoDescriptor obj = (TypeInfoDescriptor)value;
            if (obj == null)
            {
                return;
            }
            Suity.ObjectType.WriteString(writer.Node("Kind"), obj.Kind);
            Suity.ObjectType.WriteString(writer.Node("Description"), obj.Description);
            Suity.ObjectType.WriteString(writer.Node("Icon"), obj.Icon);
            Suity.ObjectType.WriteString(writer.Node("Category"), obj.Category);
            Suity.ObjectType.WriteString(writer.Node("Side"), obj.Side);
            Suity.ObjectType.WriteBoolean(writer.Node("IsValueType"), obj.IsValueType);
            if (obj.Fields != null)
            {
                var childWriterFields = writer.Nodes("Fields", obj.Fields.Count);
                foreach (var childObj in obj.Fields)
                {
                    Suity.ObjectType.Write(childWriterFields.Item(), WriteFieldDescriptor, childObj);
                }
                childWriterFields.Finish();
            }
            else
            {
                writer.Nodes("Fields", 0).Finish();
            }
        }
        public static object CloneTypeInfoDescriptor(object source, object target, bool autoNew)
        {
            if (source == null)
            {
                if (autoNew)
                {
                    return new TypeInfoDescriptor();
                }
                else
                {
                    return null;
                }
            }
            TypeInfoDescriptor value = (TypeInfoDescriptor)source;
            var obj = target != null ? (TypeInfoDescriptor)target : new TypeInfoDescriptor();
            obj.Kind = value.Kind;
            obj.Description = value.Description;
            obj.Icon = value.Icon;
            obj.Category = value.Category;
            obj.Side = value.Side;
            obj.IsValueType = value.IsValueType;
            foreach (var item in value.Fields)
            {
                obj.Fields.Add((FieldDescriptor)CloneFieldDescriptor(item, null, autoNew));
            }
            return obj;
        }
        public static bool ObjectEqualsTypeInfoDescriptor(object obj1, object obj2)
        {
            TypeInfoDescriptor v1 = (TypeInfoDescriptor)obj1;
            TypeInfoDescriptor v2 = (TypeInfoDescriptor)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (v1.Kind != v2.Kind)
                return false;
            if (v1.Description != v2.Description)
                return false;
            if (v1.Icon != v2.Icon)
                return false;
            if (v1.Category != v2.Category)
                return false;
            if (v1.Side != v2.Side)
                return false;
            if (v1.IsValueType != v2.IsValueType)
                return false;
            for (int i = 0; i < v1.Fields.Count; i++)
            {
                if (!ObjectEqualsFieldDescriptor(v1.Fields[i], v2.Fields[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static void ExchangeTypeInfoDescriptor(object obj, Suity.IExchange ex)
        {
            TypeInfoDescriptor value = (TypeInfoDescriptor)obj;
            if (value == null)
            {
                return;
            }
            value.Kind = (string)ex.Exchange("Kind", value.Kind);
            value.Description = (string)ex.Exchange("Description", value.Description);
            value.Icon = (string)ex.Exchange("Icon", value.Icon);
            value.Category = (string)ex.Exchange("Category", value.Category);
            value.Side = (string)ex.Exchange("Side", value.Side);
            value.IsValueType = (bool)ex.Exchange("IsValueType", value.IsValueType);
            value.Fields = (List<FieldDescriptor>)ex.Exchange("Fields", value.Fields);
        }
        #endregion

        #region FieldDescriptor
        public static object ReadFieldDescriptor(Suity.IDataReader reader)
        {
            var obj = new FieldDescriptor
            {
                Name = Suity.ObjectType.ReadString(reader.Node("Name")),
                Description = Suity.ObjectType.ReadString(reader.Node("Description")),
                Type = Suity.ObjectType.ReadString(reader.Node("Type"))
            };
            return obj;
        }
        public static void WriteFieldDescriptor(Suity.IDataWriter writer, object value)
        {
            FieldDescriptor obj = (FieldDescriptor)value;
            if (obj == null)
            {
                return;
            }
            Suity.ObjectType.WriteString(writer.Node("Name"), obj.Name);
            Suity.ObjectType.WriteString(writer.Node("Description"), obj.Description);
            Suity.ObjectType.WriteString(writer.Node("Type"), obj.Type);
        }
        public static object CloneFieldDescriptor(object source, object target, bool autoNew)
        {
            if (source == null)
            {
                if (autoNew)
                {
                    return new FieldDescriptor();
                }
                else
                {
                    return null;
                }
            }
            FieldDescriptor value = (FieldDescriptor)source;
            var obj = target != null ? (FieldDescriptor)target : new FieldDescriptor();
            obj.Name = value.Name;
            obj.Description = value.Description;
            obj.Type = value.Type;
            return obj;
        }
        public static bool ObjectEqualsFieldDescriptor(object obj1, object obj2)
        {
            FieldDescriptor v1 = (FieldDescriptor)obj1;
            FieldDescriptor v2 = (FieldDescriptor)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (v1.Name != v2.Name)
                return false;
            if (v1.Description != v2.Description)
                return false;
            if (v1.Type != v2.Type)
                return false;
            return true;
        }
        public static void ExchangeFieldDescriptor(object obj, Suity.IExchange ex)
        {
            FieldDescriptor value = (FieldDescriptor)obj;
            if (value == null)
            {
                return;
            }
            value.Name = (string)ex.Exchange("Name", value.Name);
            value.Description = (string)ex.Exchange("Description", value.Description);
            value.Type = (string)ex.Exchange("Type", value.Type);
        } 
        #endregion

        #region EmptyResult
        public static EmptyResult ReadEmptyResult(Suity.IDataReader reader)
        {
            return EmptyResult.Empty;
        }
        public static void WriteEmptyResult(Suity.IDataWriter writer, object value)
        {
        }
        public static EmptyResult CloneEmptyResult(object source, object target, bool autoNew)
        {
            return EmptyResult.Empty;
        }
        public static bool ObjectEqualsEmptyResult(object obj1, object obj2)
        {
            EmptyResult v1 = (EmptyResult)obj1;
            EmptyResult v2 = (EmptyResult)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            return true;
        }
        public static void ExchangeEmptyResult(object obj, IExchange ex)
        {
        }
        #endregion

        #region ErrorResult
        public static ErrorResult ReadErrorResult(Suity.IDataReader reader)
        {
            var obj = new ErrorResult
            {
                StatusCode = Suity.ObjectType.ReadString(reader.Node("StatusCode")),
                Message = Suity.ObjectType.ReadString(reader.Node("Message")),
                Location = Suity.ObjectType.ReadString(reader.Node("Location"))
            };
            return obj;
        }
        public static void WriteErrorResult(Suity.IDataWriter writer, object value)
        {
            ErrorResult obj = value as ErrorResult;
            if (obj == null)
            {
                return;
            }
            Suity.ObjectType.WriteString(writer.Node("StatusCode"), obj.StatusCode);
            Suity.ObjectType.WriteString(writer.Node("Message"), obj.Message);
            Suity.ObjectType.WriteString(writer.Node("Location"), obj.Location);
        }
        public static ErrorResult CloneErrorResult(object source, object target, bool autoNew)
        {
            if (source == null)
            {
                if (autoNew)
                {
                    return new ErrorResult();
                }
                else
                {
                    return null;
                }
            }
            ErrorResult value = source as ErrorResult;
            var obj = target != null ? (ErrorResult)target : new ErrorResult();
            obj.StatusCode = value.StatusCode;
            obj.Message = value.Message;
            obj.Location = value.Location;
            return obj;
        }
        public static bool ObjectEqualsErrorResult(object obj1, object obj2)
        {
            ErrorResult v1 = (ErrorResult)obj1;
            ErrorResult v2 = (ErrorResult)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (v1.StatusCode != v2.StatusCode)
                return false;
            if (v1.Message != v2.Message)
                return false;
            if (v1.Location != v2.Location)
                return false;
            return true;
        }
        public static void ExchangeErrorResult(object obj, IExchange ex)
        {
            if (obj is ErrorResult result)
            {
                result.StatusCode = (string)ex.Exchange("StatusCode", result.StatusCode);
                result.Message = (string)ex.Exchange("Message", result.Message);
                result.Location = (string)ex.Exchange("Location", result.Location);
            }
        }
        #endregion

        #region ButtonValue
        public static ButtonValue ReadButtonValue(Suity.IDataReader reader)
        {
            bool isClicked = reader.Node("IsClicked")?.ReadBoolean() ?? false;

            return isClicked ? ButtonValue.Clicked : ButtonValue.Empty;
        }
        public static void WriteButtonValue(Suity.IDataWriter writer, object value)
        {
            ButtonValue button = (ButtonValue)value;

            writer.Node("IsClicked").WriteBoolean(button.IsClicked);
        }
        public static ButtonValue CloneButtonValue(object source, object target, bool autoNew)
        {
            return source as ButtonValue;
        }
        public static bool ObjectEqualsButtonValue(object obj1, object obj2)
        {
            ButtonValue v1 = (ButtonValue)obj1;
            ButtonValue v2 = (ButtonValue)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            return v1.IsClicked == v2.IsClicked;
        }
        public static void ExchangeButtonValue(object obj, IExchange ex)
        {
        }
        #endregion

        #region LabelValue
        public static LabelValue ReadLabelValue(Suity.IDataReader reader)
        {
            return LabelValue.Empty;
        }
        public static void WriteLabelValue(Suity.IDataWriter writer, object value)
        {
        }
        public static LabelValue CloneLabelValue(object source, object target, bool autoNew)
        {
            return LabelValue.Empty;
        }
        public static bool ObjectEqualsLabelValue(object obj1, object obj2)
        {
            LabelValue v1 = (LabelValue)obj1;
            LabelValue v2 = (LabelValue)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            return true;
        }
        public static void ExchangeLabelValue(object obj, IExchange ex)
        {
        }
        #endregion

        #region ObjectPrototype
        public static ObjectPrototype ReadObjectPrototype(Suity.IDataReader reader)
        {
            return ObjectType.ReadObject(reader) as ObjectPrototype;
        }
        public static void WriteObjectPrototype(Suity.IDataWriter writer, object value)
        {
            ObjectType.WriteObject(writer, value);
        }
        public static ObjectPrototype CloneObjectPrototype(object source, object target, bool autoNew)
        {
            return ObjectType.CloneObject(source, target, autoNew) as ObjectPrototype;
        }
        public static bool ObjectEqualsObjectPrototype(object obj1, object obj2)
        {
            return ObjectType.ObjectEquals(obj1, obj2);
        }
        public static void ExchangeObjectPrototype(object obj, IExchange ex)
        {
            ObjectType.ExchangeObject(obj, ex);
        } 
        #endregion
#if !BRIDGE
        #region RawNode
        public static RawNode ReadRawNode(IDataReader reader)
        {
            RawNode node = new RawNode(reader.Node("Name")?.ReadString(), reader.Node("Value")?.ReadString());

            foreach (var childReader in reader.Nodes("Attributes"))
            {
                node.SetAttribute(childReader.Node("Name")?.ReadString(), childReader.Node("Value")?.ReadString());
            }

            foreach (var childReader in reader.Nodes("ChildNodes"))
            {
                node.AddNode(ReadRawNode(childReader));
            }

            return node;
        }
        public static void WriteRawNode(IDataWriter writer, object value)
        {
            RawNode node = value as RawNode;
            if (node == null)
            {
                return;
            }
            writer.Node("Name").WriteString(node.NodeName);
            writer.Node("Value").WriteString(node.NodeValue);

            if (node.AttributeCount > 0)
            {
                var childWriterActions = writer.Nodes("Attributes", node.ChildCount);
                foreach (var attr in node.Attributes)
                {
                    var childWriter = childWriterActions.Item();
                    childWriter.Node("Name").WriteString(attr.Key);
                    childWriter.Node("Value").WriteString(attr.Value);
                }
                childWriterActions.Finish();
            }

            if (node.ChildCount > 0)
            {
                var childWriterActions = writer.Nodes("ChildNodes", node.ChildCount);
                foreach (var childNode in node.Nodes())
                {
                    WriteRawNode(childWriterActions.Item(), childNode);
                }
                childWriterActions.Finish();
            }
        }
        public static RawNode CloneRawNode(object source, object target, bool autoNew)
        {
            if (source == null)
            {
                if (autoNew)
                {
                    return new RawNode();
                }
                else
                {
                    return null;
                }
            }
            RawNode value = source as RawNode;
            var obj = target != null ? (RawNode)target : new RawNode();
            obj.ClonePropertyFrom(value);
            return obj;
        }
        public static bool CompareRawNode(object obj1, object obj2)
        {
            RawNode v1 = (RawNode)obj1;
            RawNode v2 = (RawNode)obj2;
            if (v1 == null && v2 == null)
            {
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }

            return v1.Equals(v2);
        }
        public static void ExchangeRawNode(object obj, IExchange ex)
        {
            if (obj is RawNode result)
            {
                result.NodeName = (string)ex.Exchange("StatusCode", result.NodeName);
                result.NodeValue = (string)ex.Exchange("Message", result.NodeValue);
            }
        } 
        #endregion
#endif
    }
}
