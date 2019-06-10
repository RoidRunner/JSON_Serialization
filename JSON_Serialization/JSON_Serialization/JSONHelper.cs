using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JSON
{
    internal enum ParseStates
    {
        DetectCollectionType,
        ExpectingFieldOrScopeEnd,
        ParsingFieldIdentifier,
        ExpectingColonOrComma,
        ParsingStringValue,
        ParsingNumberValue,
        SeekingValue
    }


    public static class JSONHelper
    {
        internal static readonly CultureInfo Culture = new CultureInfo("en-us");

        internal static string NULL = "null";
        internal const string TRUE = "true";
        internal const string FALSE = "false";

        #region strings

        private static Dictionary<string, char> JSONtoFORMAT = new Dictionary<string, char>(8);
        private static Dictionary<char, string> FORMATtoJSON = new Dictionary<char, string>(8);

        static JSONHelper()
        {
            JSONtoFORMAT.Add("\\\"", '\"');
            JSONtoFORMAT.Add("\\\\", '\\');
            JSONtoFORMAT.Add("\\/", '/');
            JSONtoFORMAT.Add("\\b", '\b');
            JSONtoFORMAT.Add("\\f", '\f');
            JSONtoFORMAT.Add("\\n", '\n');
            JSONtoFORMAT.Add("\\r", '\r');
            JSONtoFORMAT.Add("\\t", '\t');

            foreach (KeyValuePair<string, char> value in JSONtoFORMAT)
            {
                FORMATtoJSON.Add(value.Value, value.Key);
            }
        }

        public static string GetSafelyFormattedString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            char[] result = new char[str.Length * 2];

            int offset = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (FORMATtoJSON.TryGetValue(str[i], out string jsonFormat))
                {
                    for (int j = 0; j < jsonFormat.Length; j++)
                    {
                        result[i + offset] = jsonFormat[j];
                        offset++;
                    }
                    offset--;
                }
                else
                {
                    result[i + offset] = str[i];
                }
            }

            return new string(result).TrimEnd('\0');
        }

        public static string GetOriginalFormat(string str)
        {
            char[] result = new char[str.Length];

            int offset = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (i < str.Length - 1)
                {
                    if (JSONtoFORMAT.TryGetValue(str.Substring(i, 2), out char originalFormat))
                    {
                        result[i + offset] = originalFormat;
                        offset--;
                        i++;
                    }
                    else
                    {
                        result[i + offset] = str[i];
                    }
                }
                else if (i < str.Length)
                {
                    result[i + offset] = str[i];
                }
            }

            return new string(result).TrimEnd('\0');
        }

        #endregion
        #region parsing

        public static readonly char[] WHITESPACE_CHARS = new char[] { ' ', '\r', '\n', '\t', '\uFEFF', '\u0009' };
        public static readonly List<char> WHITESPACE_CHARS_LIST = new List<char>(WHITESPACE_CHARS);

        public static string TrimStringForParsing(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            return str.Trim(WHITESPACE_CHARS);
        }

        public static bool IsWhiteSpace(char c)
        {
            return WHITESPACE_CHARS_LIST.Contains(c);
        }

        public static string GetContextExcerpt(string str, int index)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                int substrStart = Math.Max(0, index - 10);
                return str.Substring(substrStart, Math.Min(str.Length - substrStart, 11)).RemoveAll('\n');
            }
        }

        internal enum StructureType
        {
            Container,
            Array,
            Object,
            Field
        }

        internal static string GetErrorMessageString(string str, int index, StructureType structure, string errormessage, Stack<JSONContainer> containerStack)
        {
            int lineNumber = str.Occurences('\n', index);
            int lineLocation;
            if (lineNumber > 0) {
                lineLocation = index - str.Substring(0, index + 1).LastIndexOf('\n');
            }
            else
            {
                lineLocation = index;
            }

            return $"Error at line {lineNumber}, position {lineLocation} (Context: \"{GetContextExcerpt(str, index)}\") parsing {structure} - {errormessage}\n" +
                $"Container Stack:\n{string.Join('\n', containerStack)}";
        }

        public static List<string> SplitFields(string str)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(str))
            {
                int startindex = 0;
                bool inString = false;
                for (int i = 0; i < str.Length; i++)
                {
                    char current = str[i];
                    if (str[i] == '\"')
                    {
                        inString = !inString;
                    }
                    else if (!inString)
                        switch (str[i])
                        {
                            case ',':
                                if (i > startindex + 1)
                                {
                                    result.Add(str.Substring(startindex, i - startindex));
                                }
                                startindex = i + 1;
                                break;
                            case '[':
                            case '{':
                                int nextIndex = getNextIndexInScope(str, i + 1);
                                if (nextIndex > 0)
                                {
                                    if (nextIndex - startindex > 0)
                                    {
                                        result.Add(str.Substring(startindex, nextIndex - startindex));
                                    }
                                    startindex = nextIndex + 1;
                                    i = nextIndex + 1;
                                }
                                break;
                            case ']':
                            case '}':
                                i = str.Length;
                                break;
                        }
                }
                result.Add(str.Substring(startindex));
            }
            return result;
        }

        private static int getNextIndexInScope(string str, int startAt)
        {
            int scopeDepth = 1;
            bool inString = false;
            for (int i = startAt; i < str.Length; i++)
            {
                if (str[i] == '\"')
                {
                    inString = !inString;
                }
                else if (!inString)
                {
                    switch (str[i])
                    {
                        case '[':
                        case '{':
                            scopeDepth++;
                            break;
                        case ']':
                        case '}':
                            scopeDepth--;
                            break;
                    }
                    if (scopeDepth == 0)
                    {
                        if (i + 1 < str.Length)
                        {
                            return i + 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            return -1;
        }

        public static char Front(this string str, int index = 0)
        {
            return str[index];
        }

        public static char Back(this string str, int index = 0)
        {
            return str[str.Length - index - 1];
        }

        public static string RemoveAll(this string str, params char[] c)
        {
            char[] result = new char[str.Length];
            int occurences = 0;
            for (int i = 0; i < str.Length; i++)
            {
                bool removed = false;
                for (int j = 0; j < c.Length && !removed; j++)
                {
                    if (str[i] == c[j])
                    {
                        occurences++;
                        removed = true;
                    }
                }
                if (!removed)
                {
                    result[i - occurences] = str[i];
                }
            }
            return new string(result).TrimEnd('\0');
        }

        public static int Occurences(this string str, char c, int endIndex = 0)
        {
            if (endIndex == 0)
            {
                endIndex = str.Length;
            }
            if (endIndex > str.Length)
            {
                endIndex = str.Length;
            }
            int count = 0;

            for (int i = 0; i < endIndex; i++)
            {
                if (str[i] == c)
                {
                    count++;
                }
            }
            return count;
        }

        #endregion
    }
}
