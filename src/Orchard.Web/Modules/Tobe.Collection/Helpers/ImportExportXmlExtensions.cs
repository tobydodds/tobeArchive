using System;
using System.Xml;
using System.Xml.Linq;
using Orchard.ContentManagement;

namespace Tobe.Collection.Helpers {
    public static class ImportExportXmlExtensions {
        public static XAttribute AddAttribute(this XElement element, string attributeName, object value) {
            if (value == null)
                return null;

            var attribute = new XAttribute(attributeName, value);
            element.Add(attribute);
            return attribute;
        }

        public static XAttribute AddAttribute(this XElement element, string attributeName, string value) {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            return AddAttribute(element, attributeName, (object)value);
        }

        public static XAttribute AddAttribute(this XElement element, string attributeName, TimeSpan? value) {
            if (value == null)
                return null;

            var attribute = new XAttribute(attributeName, XmlConvert.ToString(value.Value));
            element.Add(attribute);
            return attribute;
        }

        public static XElement AddElement(this XElement parentElement, string childElementName, string content) {
            if (String.IsNullOrWhiteSpace(content))
                return null;

            var element = new XElement(childElementName, content);
            parentElement.Add(element);
            return element;
        }

        public static void ImportAttribute<T>(this XElement element, string attributeName, Action<T> importAction) {
            var attributeValue = element.Attr<T>(attributeName);

            if (ReferenceEquals(attributeValue, default(T)))
                return;

            importAction(attributeValue);
        }

        public static TReturn ImportAttribute<T, TReturn>(this XElement element, string attributeName, Func<T, TReturn> importAction) {
            var attributeValue = element.Attr<T>(attributeName);

            if (ReferenceEquals(attributeValue, default(T)))
                return default(TReturn);

            return importAction(attributeValue);
        }

        public static void ImportElement(this XElement element, string elementName, Action<string> importAction) {
            var elementValue = element.El(elementName);

            if (String.IsNullOrWhiteSpace(elementValue))
                return;

            importAction(elementValue);
        }
    }
}