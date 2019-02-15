using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ObjectDumping.Internal
{
    internal static class StringExtensions
    {
        internal static string ToLowerFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            var a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

        private static readonly Dictionary<string, string> EscapeMapping = new Dictionary<string, string>()
        {
            { "\'", "\\\'" }, //  Single quote
            { "\"", "\\\"" }, //  Double quote
            { "\\\\", @"\\" }, // Backslash
            { "\a", @"\a" }, // Alert (ASCII 7)
            { "\b", @"\b" }, // Backspace (ASCII 8)
            { "\f", @"\f" }, // Form feed (ASCII 12)
            { "\n", @"\n" }, // New line (ASCII 10)
            { "\r", @"\r" }, // Carriage return (ASCII 13)
            { "\t", @"\t" }, // Horizontal tab (ASCII 9)
            { "\v", @"\v" }, // Vertical quote (ASCII 11)
            { "\0", @"\0" }, // Empty space (ASCII 0)
        };

        private static readonly Regex EscapeRegex = new Regex(string.Join("|", EscapeMapping.Keys.ToArray()));

        /// <summary>
        /// Escapes the escape sequences in given string <paramref name="s"/>.
        /// </summary>
        public static string Escape(this string s)
        {
            return EscapeRegex.Replace(s, EscapeMatchEval);
        }

        private static string EscapeMatchEval(Match m)
        {
            if (EscapeMapping.ContainsKey(m.Value))
            {
                return EscapeMapping[m.Value];
            }

            return EscapeMapping[Regex.Escape(m.Value)];
        }
    }
}
