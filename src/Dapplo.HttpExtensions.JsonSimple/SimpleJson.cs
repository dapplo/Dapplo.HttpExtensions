//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#pragma warning disable 1591
// VERSION: 0.38.0

// NOTE: uncomment the following line to make SimpleJson class internal.
//#define SIMPLE_JSON_INTERNAL

// NOTE: uncomment the following line to make JsonArray and JsonObject class internal.
//#define SIMPLE_JSON_OBJARRAYINTERNAL

// NOTE: uncomment the following line to enable dynamic support.
#define SIMPLE_JSON_DYNAMIC

// NOTE: uncomment the following line to enable DataContract support.
#define SIMPLE_JSON_DATACONTRACT

// NOTE: uncomment the following line to enable IReadOnlyCollection<T> and IReadOnlyList<T> support.
#define SIMPLE_JSON_READONLY_COLLECTIONS

// NOTE: uncomment the following line to disable linq expressions/compiled lambda (better performance) instead of method.invoke().
// define if you are using .net framework <= 3.0 or < WP7.5
//#define SIMPLE_JSON_NO_LINQ_EXPRESSION

// NOTE: uncomment the following line if you are compiling under Window Metro style application/library.
// usually already defined in properties
//#define NETFX_CORE;

// If you are targetting WinStore, WP8 and NET4.5+ PCL make sure to #define SIMPLE_JSON_TYPEINFO;

// original json parsing code from http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html

#if NETSTANDARD1_3
#define SIMPLE_JSON_TYPEINFO
#endif

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

#if !SIMPLE_JSON_NO_LINQ_EXPRESSION
#endif
#if SIMPLE_JSON_DYNAMIC
#endif

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable RedundantExplicitArrayCreation
// ReSharper disable SuggestUseVarKeywordEvident

namespace Dapplo.HttpExtensions.JsonSimple
{
    /// <summary>
    ///     This class encodes and decodes JSON strings.
    ///     Spec. details, see http://www.json.org/
    ///     JSON uses Arrays and Objects. These correspond here to the datatypes JsonArray(IList&lt;object>) and
    ///     JsonObject(IDictionary&lt;string,object>).
    ///     All numbers are parsed to doubles.
    /// </summary>
    [GeneratedCode("simple-json", "1.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
#if SIMPLE_JSON_INTERNAL
	internal
#else
    public
#endif
        static class SimpleJson
    {
        private const int TOKEN_NONE = 0;
        private const int TOKEN_CURLY_OPEN = 1;
        private const int TOKEN_CURLY_CLOSE = 2;
        private const int TOKEN_SQUARED_OPEN = 3;
        private const int TOKEN_SQUARED_CLOSE = 4;
        private const int TOKEN_COLON = 5;
        private const int TOKEN_COMMA = 6;
        private const int TOKEN_STRING = 7;
        private const int TOKEN_NUMBER = 8;
        private const int TOKEN_TRUE = 9;
        private const int TOKEN_FALSE = 10;
        private const int TOKEN_NULL = 11;
        private const int BUILDER_CAPACITY = 2000;

        private static readonly char[] EscapeTable;
        private static readonly char[] EscapeCharacters = {'"', '\\', '\b', '\f', '\n', '\r', '\t'};

        static SimpleJson()
        {
            EscapeTable = new char[93];
            EscapeTable['"'] = '"';
            EscapeTable['\\'] = '\\';
            EscapeTable['\b'] = 'b';
            EscapeTable['\f'] = 'f';
            EscapeTable['\n'] = 'n';
            EscapeTable['\r'] = 'r';
            EscapeTable['\t'] = 't';
        }

        /// <summary>
        ///     Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An IList&lt;object>, a IDictionary&lt;string,object>, a double, a string, null, true, or false</returns>
        public static object DeserializeObject(string json)
        {
            if (TryDeserializeObject(json, out var obj))
            {
                return obj;
            }
            throw new SerializationException("Invalid JSON string");
        }

        /// <summary>
        ///     Try parsing the json string into a value.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="obj">The object.</param>
        /// <returns>Returns true if successfull otherwise false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
        public static bool TryDeserializeObject(string json, out object obj)
        {
            var success = true;
            if (json != null)
            {
                var charArray = json.ToCharArray();
                var index = 0;
                obj = ParseValue(charArray, ref index, ref success);
            }
            else
            {
                obj = null;
            }

            return success;
        }

        public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy = null)
        {
            var jsonObject = DeserializeObject(json);
            return type is null || jsonObject != null && ReflectionUtils.IsAssignableFrom(jsonObject.GetType(), type)
                ? jsonObject
                : (jsonSerializerStrategy ?? CurrentJsonSerializerStrategy).DeserializeObject(jsonObject, type);
        }

        public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy = null)
        {
            return (T) DeserializeObject(json, typeof(T), jsonSerializerStrategy);
        }

        /// <summary>
        ///     Converts a IDictionary&lt;string,object> / IList&lt;object> object into a JSON string
        /// </summary>
        /// <param name="json">A IDictionary&lt;string,object> / IList&lt;object></param>
        /// <param name="jsonSerializerStrategy">Serializer strategy to use</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy = null)
        {
            var builder = new StringBuilder(BUILDER_CAPACITY);
            var success = SerializeValue(jsonSerializerStrategy ?? CurrentJsonSerializerStrategy, json, builder);
            return success ? builder.ToString() : null;
        }

        /// <summary>
        ///     minify(compress) the JSON string.
        ///     inspired by http://stackoverflow.com/questions/8913138/minify-indented-json-string-in-net
        /// </summary>
        /// <param name="json">JSON string to compress</param>
        /// <returns>minified JSON string</returns>
        public static string Minify(string json)
        {
            return Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
        }

        public static string EscapeToJavascriptString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return jsonString;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < jsonString.Length;)
            {
                var c = jsonString[i++];

                if (c == '\\')
                {
                    var remainingLength = jsonString.Length - i;
                    if (remainingLength >= 2)
                    {
                        var lookahead = jsonString[i];
                        if (lookahead == '\\')
                        {
                            sb.Append('\\');
                            ++i;
                        }
                        else if (lookahead == '"')
                        {
                            sb.Append("\"");
                            ++i;
                        }
                        else if (lookahead == 't')
                        {
                            sb.Append('\t');
                            ++i;
                        }
                        else if (lookahead == 'b')
                        {
                            sb.Append('\b');
                            ++i;
                        }
                        else if (lookahead == 'n')
                        {
                            sb.Append('\n');
                            ++i;
                        }
                        else if (lookahead == 'r')
                        {
                            sb.Append('\r');
                            ++i;
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
        {
            IDictionary<string, object> table = new JsonObject();

            // {
            NextToken(json, ref index);

            while (true)
            {
                var token = LookAhead(json, index);
                if (token == TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                if (token == TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == TOKEN_CURLY_CLOSE)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {
                    // name
                    var name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }
                    // :
                    token = NextToken(json, ref index);
                    if (token != TOKEN_COLON)
                    {
                        success = false;
                        return null;
                    }
                    // value
                    var value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }
                    table[name] = value;
                }
            }
        }

        private static JsonArray ParseArray(char[] json, ref int index, ref bool success)
        {
            var array = new JsonArray();

            // [
            NextToken(json, ref index);

            while (true)
            {
                var token = LookAhead(json, index);
                if (token == TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                if (token == TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == TOKEN_SQUARED_CLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    var value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }
                    array.Add(value);
                }
            }
            return array;
        }

        private static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case TOKEN_STRING:
                    return ParseString(json, ref index, ref success);
                case TOKEN_NUMBER:
                    return ParseNumber(json, ref index, out success);
                case TOKEN_CURLY_OPEN:
                    return ParseObject(json, ref index, ref success);
                case TOKEN_SQUARED_OPEN:
                    return ParseArray(json, ref index, ref success);
                case TOKEN_TRUE:
                    NextToken(json, ref index);
                    return true;
                case TOKEN_FALSE:
                    NextToken(json, ref index);
                    return false;
                case TOKEN_NULL:
                    NextToken(json, ref index);
                    return null;
                case TOKEN_NONE:
                    break;
            }
            success = false;
            return null;
        }

        private static string ParseString(char[] json, ref int index, ref bool success)
        {
            var s = new StringBuilder(BUILDER_CAPACITY);

            EatWhitespace(json, ref index);

            // "
            index++;
            var complete = false;
            while (true)
            {
                if (index == json.Length)
                {
                    break;
                }

                var c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                if (c == '\\')
                {
                    if (index == json.Length)
                    {
                        break;
                    }
                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        var remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint)))
                            {
                                return "";
                            }

                            // convert the integer codepoint to a unicode char and add to string
                            if (0xD800 <= codePoint && codePoint <= 0xDBFF) // if high surrogate
                            {
                                index += 4; // skip 4 chars
                                remainingLength = json.Length - index;
                                if (remainingLength >= 6)
                                {
                                    if (new string(json, index, 2) == "\\u" &&
                                        uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var lowCodePoint))
                                    {
                                        if (0xDC00 <= lowCodePoint && lowCodePoint <= 0xDFFF) // if low surrogate
                                        {
                                            s.Append((char) codePoint);
                                            s.Append((char) lowCodePoint);
                                            index += 6; // skip 6 chars
                                            continue;
                                        }
                                    }
                                }
                                success = false; // invalid surrogate pair
                                return "";
                            }
                            s.Append(ConvertFromUtf32((int) codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    s.Append(c);
                }
            }
            if (!complete)
            {
                success = false;
                return null;
            }
            return s.ToString();
        }

        private static string ConvertFromUtf32(int utf32)
        {
            // http://www.java2s.com/Open-Source/CSharp/2.6.4-mono-.net-core/System/System/Char.cs.htm
            if (utf32 < 0 || utf32 > 0x10FFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(utf32), "The argument must be from 0 to 0x10FFFF.");
            }
            if (0xD800 <= utf32 && utf32 <= 0xDFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(utf32), "The argument must not be in surrogate pair range.");
            }
            if (utf32 < 0x10000)
            {
                return new string((char) utf32, 1);
            }
            utf32 -= 0x10000;
            return new string(new char[] {(char) ((utf32 >> 10) + 0xD800), (char) (utf32 % 0x0400 + 0xDC00)});
        }

        private static object ParseNumber(char[] json, ref int index, out bool success)
        {
            EatWhitespace(json, ref index);
            var lastIndex = GetLastIndexOfNumber(json, index);
            var charLength = lastIndex - index + 1;
            object returnNumber = null;
            var numberAsString = new string(json, index, charLength);
            if (numberAsString.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1
                || numberAsString.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
            {
                success = double.TryParse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var number);
                returnNumber = number;
            }
            else
            {
                if (long.TryParse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var longNumber))
                {
                    returnNumber = longNumber;
                }
                else if (decimal.TryParse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalNumber))
                {
                    returnNumber = decimalNumber;
                }
                else if (double.TryParse(numberAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleNumber))
                {
                    returnNumber = doubleNumber;
                }

                success = returnNumber != null;
            }
            index = lastIndex + 1;
            return returnNumber;
        }

        private static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;
            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        private static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r\b\f".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        private static int LookAhead(char[] json, int index)
        {
            var saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);
            if (index == json.Length)
            {
                return TOKEN_NONE;
            }
            var c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return TOKEN_CURLY_OPEN;
                case '}':
                    return TOKEN_CURLY_CLOSE;
                case '[':
                    return TOKEN_SQUARED_OPEN;
                case ']':
                    return TOKEN_SQUARED_CLOSE;
                case ',':
                    return TOKEN_COMMA;
                case '"':
                    return TOKEN_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return TOKEN_NUMBER;
                case ':':
                    return TOKEN_COLON;
            }
            index--;
            var remainingLength = json.Length - index;
            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
                {
                    index += 5;
                    return TOKEN_FALSE;
                }
            }
            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
                {
                    index += 4;
                    return TOKEN_TRUE;
                }
            }
            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
                {
                    index += 4;
                    return TOKEN_NULL;
                }
            }
            return TOKEN_NONE;
        }

        private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
        {
            var success = true;
            if (value is string stringValue)
            {
                return SerializeString(stringValue, builder);
            }
            if (IsChar(value))
            {
                return SerializeString(((char) value).ToString(), builder);
            }

            switch (value)
            {
                case IDictionary<string, object> dict:
                    return SerializeObject(jsonSerializerStrategy, dict.Keys, dict.Values, builder);
                case IDictionary<string, string> stringDictionary:
                    return SerializeObject(jsonSerializerStrategy, stringDictionary.Keys, stringDictionary.Values, builder);
                case IEnumerable enumerableValue:
                    return SerializeArray(jsonSerializerStrategy, enumerableValue, builder);
            }

            if (IsNumeric(value))
            {
                return SerializeNumber(value, builder);
            }

            if (value is bool b)
            {
                builder.Append(b ? "true" : "false");
            }
            else if (value is null)
            {
                builder.Append("null");
            }
            else
            {
                success = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out var serializedObject);
                if (success)
                {
                    SerializeValue(jsonSerializerStrategy, serializedObject, builder);
                }
            }
            return success;
        }

        private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
        {
            builder.Append("{");
            var ke = keys.GetEnumerator();
            var ve = values.GetEnumerator();
            var first = true;
            while (ke.MoveNext() && ve.MoveNext())
            {
                var key = ke.Current;
                var value = ve.Current;
                if (!first)
                {
                    builder.Append(",");
                }

                if (key is string stringKey)
                {
                    SerializeString(stringKey, builder);
                }
                else if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }
                builder.Append(":");
                if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }
                first = false;
            }
            builder.Append("}");
            return true;
        }

        private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
        {
            builder.Append("[");
            var first = true;
            foreach (var value in anArray)
            {
                if (!first)
                {
                    builder.Append(",");
                }
                if (!SerializeValue(jsonSerializerStrategy, value, builder))
                {
                    return false;
                }
                first = false;
            }
            builder.Append("]");
            return true;
        }

        private static bool SerializeString(string aString, StringBuilder builder)
        {
            // Happy path if there's nothing to be escaped. IndexOfAny is highly optimized (and unmanaged)
            if (aString.IndexOfAny(EscapeCharacters) == -1)
            {
                builder.Append('"');
                builder.Append(aString);
                builder.Append('"');

                return true;
            }

            builder.Append('"');
            var safeCharacterCount = 0;
            var charArray = aString.ToCharArray();

            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];

                // Non ascii characters are fine, buffer them up and send them to the builder
                // in larger chunks if possible. The escape table is a 1:1 translation table
                // with \0 [default(char)] denoting a safe character.
                if (c >= EscapeTable.Length || EscapeTable[c] == default(char))
                {
                    safeCharacterCount++;
                }
                else
                {
                    if (safeCharacterCount > 0)
                    {
                        builder.Append(charArray, i - safeCharacterCount, safeCharacterCount);
                        safeCharacterCount = 0;
                    }

                    builder.Append('\\');
                    builder.Append(EscapeTable[c]);
                }
            }

            if (safeCharacterCount > 0)
            {
                builder.Append(charArray, charArray.Length - safeCharacterCount, safeCharacterCount);
            }

            builder.Append('"');
            return true;
        }

        private static bool SerializeNumber(object number, StringBuilder builder)
        {
            if (number is long l)
            {
                builder.Append(l.ToString(CultureInfo.InvariantCulture));
            }
            else if (number is ulong)
            {
                builder.Append(((ulong) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is int)
            {
                builder.Append(((int) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is uint)
            {
                builder.Append(((uint) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is decimal)
            {
                builder.Append(((decimal) number).ToString(CultureInfo.InvariantCulture));
            }
            else if (number is float)
            {
                builder.Append(((float) number).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
            }
            return true;
        }

        /// <summary>
        ///     Determines if a given object is a char.
        /// </summary>
        private static bool IsChar(object value)
        {
            if (value is char)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines if a given object is numeric in any way
        ///     (can be integer, double, null, etc).
        /// </summary>
        private static bool IsNumeric(object value)
        {
            if (value is sbyte)
            {
                return true;
            }
            if (value is byte)
            {
                return true;
            }
            if (value is short)
            {
                return true;
            }
            if (value is ushort)
            {
                return true;
            }
            if (value is int)
            {
                return true;
            }
            if (value is uint)
            {
                return true;
            }
            if (value is long)
            {
                return true;
            }
            if (value is ulong)
            {
                return true;
            }
            if (value is float)
            {
                return true;
            }
            if (value is double)
            {
                return true;
            }
            if (value is decimal)
            {
                return true;
            }
            return false;
        }

        private static IJsonSerializerStrategy _currentJsonSerializerStrategy;

        public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
        {
            get
            {
                return _currentJsonSerializerStrategy ??
                       (_currentJsonSerializerStrategy =
#if SIMPLE_JSON_DATACONTRACT
                               DataContractJsonSerializerStrategy
#else
 PocoJsonSerializerStrategy
#endif
                       );
            }
            set { _currentJsonSerializerStrategy = value; }
        }

        private static PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
            => _pocoJsonSerializerStrategy ?? (_pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());

#if SIMPLE_JSON_DATACONTRACT

        private static DataContractJsonSerializerStrategy _dataContractJsonSerializerStrategy;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static DataContractJsonSerializerStrategy DataContractJsonSerializerStrategy
            => _dataContractJsonSerializerStrategy ?? (_dataContractJsonSerializerStrategy = new DataContractJsonSerializerStrategy());

#endif
    }
}