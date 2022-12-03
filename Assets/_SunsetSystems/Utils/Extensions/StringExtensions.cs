using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SunsetSystems.Utils
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ':' || c == '[' || c == ']' || c == '(' || c == ')')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string ToCamelCase(this string str)
        {
            string[] words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            string leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });
            string[] tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word[1..])
                .ToArray();
            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }

        public static string ToPascalCase(this string str)
        {
            string[] words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
            words = words
                .Select(word => char.ToUpper(word[0]) + word[1..])
                .ToArray();
            return $"{string.Join(string.Empty, words)}";
        }
    }
}
