// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
#if !BRIDGE
using System;
using System.IO;
using System.Xml;


namespace Suity.NodeQuery
{
    public class XmlNodeWriter : MarshalByRefObject, INodeWriter
    {
        readonly XmlDocument _doc = new XmlDocument();

        XmlElement _currentElement;

        public XmlNodeWriter(string rootName, bool decalaration = true)
        {
            if (decalaration)
            {
                XmlDeclaration xmlDeclaration = _doc.CreateXmlDeclaration("1.0", "utf-8", null);
                _doc.AppendChild(xmlDeclaration);
            }
            _doc.AppendChild(_doc.CreateElement(rootName));
            _currentElement = _doc.DocumentElement;
        }

        public void BeginElement(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException();
            XmlElement newElement = _doc.CreateElement(name);
            _currentElement.AppendChild(newElement);
            _currentElement = newElement;
        }
        public void AddElement(string name, string innerText)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException();
            XmlElement newElement = _doc.CreateElement(name);
            _currentElement.AppendChild(newElement);
            newElement.InnerText = innerText;
        }
        public void AddElement(string name, Action<INodeWriter> action)
        {
            BeginElement(name);
            try
            {
                action(this);
            }
            finally
            {
                EndElement(name);
            }
        }
        public void SetValue(string innerText)
        {
            _currentElement.InnerText = innerText;
        }
        public void EndElement(string name)
        {
            if (_currentElement.Name != name) throw new InvalidOperationException();
            _currentElement = _currentElement.ParentNode as XmlElement;
        }
        public void EndElement()
        {
            _currentElement = _currentElement.ParentNode as XmlElement;
        }
        public void RevertElement()
        {
            if (_currentElement == _doc.DocumentElement) return;
            XmlElement e = _currentElement;
            _currentElement = _currentElement.ParentNode as XmlElement;
            _currentElement?.RemoveChild(e);
        }

        public void SetAttribute(string name, object valueToString)
        {
            _currentElement.SetAttribute(name, valueToString != null ? valueToString.ToString() : "");
        }

        public int ChildNodeCount { get { return _currentElement.ChildNodes.Count; } }
        public int AttributeCount { get { return _currentElement.Attributes.Count; } }
        public string InnerText { get { return _currentElement.InnerText; } }
        public bool IsEmptyElement { get { return ChildNodeCount == 0 && AttributeCount == 0 && string.IsNullOrEmpty(InnerText); } }

        public void SaveToFile(string path)
        {
            if (_currentElement != _doc.DocumentElement)
            {
                throw new InvalidOperationException("Document not end");
            }

            _doc.Save(path);
        }
        public void SaveToStream(Stream stream)
        {
            if (_currentElement != _doc.DocumentElement)
            {
                throw new InvalidOperationException("Document not end");
            }

            _doc.Save(stream);
        }

        public XmlDocument GetDocument()
        {
            return _doc;
        }

        public override string ToString()
        {
            StringWriter writer = new StringWriter();
            _doc.Save(writer);
            return writer.ToString();
        }

    }
}
#endif