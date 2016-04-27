using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Tobe.Collection.Helpers {
    public static class StringExtensions {
        public static string TrimSafe(this string value) {
            return value != null ? value.Trim() : null;
        }

        public static string Truncate(this string text, int maxCharacterCount, string ellipsis ="...") {
            var max = maxCharacterCount;
            var index = 0;
            var count = 0;
            var isTag = false;
            var isEndTag = false;
            var isEntity = false;
            var lastTagName = "";
            var tagName = "";
            var isWord = false;
            var lastWordIndex = 0;
            var isAttribute = false;

            while (index < text.Length && count < max) {
                var character = text[index];

                if (!isTag && !isEntity) {
                    switch(character) {
                        case '<':
                            isTag = true;
                            isEndTag = false;
                            isWord = false;
                            break;
                        case '&':
                            isEntity = true;
                            isWord = false;
                            break;
                        default:
                            count++;
                            if((character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z')) {
                                if(!isWord) {
                                    isWord = true;
                                    lastWordIndex = index;
                                }
                            } else if(isWord) {
                                isWord = false;
                            }
                            break;
                    }
                }
                else if(isEntity) {
                    switch (character) {
                        case ';':
                            isEntity = false;
                            break;
                    }
                }
                else {
                    switch (character) {
                        case '/':
                            if(!isAttribute) {
                                isEndTag = true;
                                lastTagName = tagName;
                                tagName = "";
                            }
                            break;
                        case '>':
                            isTag = false;
                            isAttribute = false;
                            lastTagName = tagName;
                            tagName = "";
                            break;
                        case ' ':
                            isAttribute = true;
                            break;
                        default:
                            if (!isAttribute)
                                tagName += character;
                            break;
                    }
                }
                index++;
            }

            string htmlExcerpt;

            if (isWord && lastWordIndex > 0 && index < text.Length && ((text[index] >= 'a' && text[index] <= 'z') || (text[index] >= 'A' && text[index] <= 'Z'))) {
                htmlExcerpt = text.Substring(0, lastWordIndex) + ellipsis;
            } else {
                htmlExcerpt = text.Substring(0, index);
            }

            if (index < text.Length) {
                htmlExcerpt = Regex.Replace(htmlExcerpt, @"(&nbsp;|\s|\t)(&nbsp;|\s|\t)*$", string.Empty);
            }

            if (!isEndTag) {
                var endTag = "</" + lastTagName + ">";
                htmlExcerpt += endTag;
            }

            return htmlExcerpt;
        }
    }
}