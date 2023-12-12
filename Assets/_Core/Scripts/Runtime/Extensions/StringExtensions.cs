using UnityEngine;

namespace UrbanFox
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static string Bold(this string text)
        {
            return $"<b>{text}</b>";
        }

        public static string Italic(this string text)
        {
            return $"<i>{text}</i>";
        }

        public static string Size(this string text, int fontSize)
        {
            return $"<size={fontSize}>{text}</size>";
        }

        public static string Color(this string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
        }

        public static string Capitalize(this string text)
        {
            if (text.IsNullOrEmpty())
            {
                return text;
            }
            if (text.Length <= 1)
            {
                return text.ToUpper();
            }
            return text[0].ToString().ToUpper() + text.Substring(1);
        }

        public static string ToXMLAttribute(this string text)
        {
            return text.Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace(">", "&gt;")
                .Replace("<", "&lt;");
        }

        public static string FromXMLAttribute(this string text)
        {
            return text.Replace("&amp;", "&")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'")
                .Replace("&gt;", ">")
                .Replace("&lt;", "<");
        }
    }
}
