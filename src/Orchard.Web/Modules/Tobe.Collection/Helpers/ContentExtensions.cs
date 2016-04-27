using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;

namespace Tobe.Collection.Helpers {
    public static class ContentExtensions {
        public static IEnumerable<T> GetContentFields<T>(this IContent content, string fieldName = null) where T : ContentField {
            var query =
                from part in content.ContentItem.Parts
                from field in part.Fields
                where field.FieldDefinition.Name == typeof(T).Name
                select (T)field;

            if (!String.IsNullOrWhiteSpace(fieldName)) {
                query = from field in query where field.Name == fieldName select field;
            }

            return query;
        }

        public static T GetContentField<T>(this IContent content, string fieldName = null) where T : ContentField {
            return GetContentFields<T>(content, fieldName).FirstOrDefault();
        }
    }
}