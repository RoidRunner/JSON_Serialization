using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JSON
{
    public enum FieldType
    {
        NULL,
        STRING,
        NUMBER,
        CONTAINER,
        BOOL,
    }

    public enum ContainerType
    {
        EMPTY,
        OBJECT,
        ARRAY
    }

    public class JSONField
    {


        #region Basic Variables

        /// <summary>
        /// Identifier string that identifies the adjacent json value
        /// </summary>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Field value interpreted as an unsigned 64-bit integer
        /// </summary>
        public ulong Unsigned_Int64 { get; private set; }
        /// <summary>
        /// Field value interpreted as a signed 64-bit integer
        /// </summary>
        public long Signed_Int64 { get; private set; }

        /// <summary>
        /// Field value interpreted as an unsigned 32-bit integer
        /// </summary>
        public uint Unsigned_Int32 => (uint)Unsigned_Int64;
        /// <summary>
        /// Field value interpreted as a signed 32-bit integer
        /// </summary>
        public int Signed_Int32 => (int)Signed_Int64;

        /// <summary>
        /// Field value interpreted as a 64-bit floating point number
        /// </summary>
        public double Float64 { get; private set; }
        /// <summary>
        /// Field value interpreted as a 32-bit floating point number
        /// </summary>
        public float Float32 => (float)Float64;

        /// <summary>
        /// Field value interpreted as a boolean
        /// </summary>
        public bool Boolean { get; private set; }

        /// <summary>
        /// Field value interpreted as a character string
        /// </summary>
        public string String { get; private set; }
        /// <summary>
        /// Field value interpreted as a single character
        /// </summary>
        public char Character
        {
            get { if (!string.IsNullOrEmpty(String)) { return String[0]; } else { return '\0'; } }
        }

        /// <summary>
        /// Field value interpreted as nested json container
        /// </summary>
        public JSONContainer Container { get; private set; }

        /// <summary>
        /// Field Type of this field
        /// </summary>
        public FieldType Type;
        /// <summary>
        /// If the number contained is < 0
        /// </summary>
        public bool IsSigned;
        /// <summary>
        /// If the number contained is not an integer
        /// </summary>
        public bool IsFloat;
        /// <summary>
        /// If the value stored is a number
        /// </summary>
        public bool IsNumber => Type == FieldType.NUMBER;
        /// <summary>
        /// If the value stored is a string
        /// </summary>
        public bool IsString => Type == FieldType.STRING;
        /// <summary>
        /// If the value stored is a boolean
        /// </summary>
        public bool IsBool => Type == FieldType.BOOL;
        /// <summary>
        /// If the value stored is a container
        /// </summary>
        public bool IsContainer => Type == FieldType.CONTAINER;
        /// <summary>
        /// If the value stored is an array container
        /// </summary>
        public bool IsArray
        {
            get
            {
                if (Type != FieldType.CONTAINER)
                {
                    return false;
                }
                if (Container != null)
                {
                    return Container.Type == ContainerType.ARRAY;
                }
                return false;
            }
        }
        /// <summary>
        /// If the value stored is an object container
        /// </summary>
        public bool IsObject
        {
            get
            {
                if (Type != FieldType.CONTAINER)
                {
                    return false;
                }
                if (Container != null)
                {
                    return Container.Type == ContainerType.OBJECT;
                }
                return false;
            }
        }


        #endregion
        #region Constructors

        internal JSONField()
        {
            Type = FieldType.NULL;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="identifier"></param>
        public JSONField(ulong val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = false;
            Unsigned_Int64 = val;
            Signed_Int64 = (long)val;
            IsFloat = false;
            Float64 = val;
        }

        public JSONField(long val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = val < 0;
            Unsigned_Int64 = (ulong)val;
            Signed_Int64 = val;
            IsFloat = false;
            Float64 = val;
        }

        public JSONField(uint val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = false;
            Unsigned_Int64 = val;
            Signed_Int64 = val;
            IsFloat = false;
            Float64 = val;
        }

        public JSONField(int val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = val < 0;
            Unsigned_Int64 = (ulong)val;
            Signed_Int64 = val;
            IsFloat = false;
            Float64 = val;
        }

        public JSONField(double val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = val < 0;
            Unsigned_Int64 = (ulong)val;
            Signed_Int64 = (long)val;
            IsFloat = true;
            Float64 = val;
        }

        public JSONField(float val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.NUMBER;
            IsSigned = val < 0;
            Unsigned_Int64 = (ulong)val;
            Signed_Int64 = (long)val;
            IsFloat = true;
            Float64 = val;
        }

        public JSONField(bool val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.BOOL;
            Boolean = val;
        }

        public JSONField(string val, string identifier = null)
        {
            Identifier = identifier;

            if (val == null)
            {
                Type = FieldType.NULL;
            }
            else
            {
                Type = FieldType.STRING;
                String = val;
            }
        }

        public JSONField(char val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.STRING;
            String = val.ToString();
        }

        public JSONField(JSONContainer val, string identifier = null)
        {
            Identifier = identifier;

            if (val == null)
            {
                Type = FieldType.NULL;
            }
            else
            {
                Type = FieldType.CONTAINER;
                Container = val;
            }
        }

        public JSONField(IEnumerable<JSONField> val, string identifier = null)
        {
            Identifier = identifier;

            Type = FieldType.CONTAINER;
            Container = JSONContainer.NewArray(val);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Identifier))
            {
                return Type.ToString();
            }
            else
            {
                return $"{Identifier} : {Type}";
            }
        }

        #endregion
        #region ToJSON

        public const string NULL = "null";
        public const string BOOL_TRUE = "true";
        public const string BOOL_FALSE = "false";

        public void Build(ref StringBuilder builder, int prettyIndent)
        {
            switch (Type)
            {
                case FieldType.NULL:
                    builder.Append(NULL);
                    break;
                case FieldType.STRING:
                    builder.Append("\"");
                    builder.Append(JSONHelper.GetSafelyFormattedString(String));
                    builder.Append("\"");
                    break;
                case FieldType.NUMBER:
                    if (IsFloat)
                    {
                        builder.Append(Float64.ToString("##########0.###########").Replace(',', '.'));
                    }
                    else if (IsSigned)
                    {
                        builder.Append(Signed_Int64.ToString());
                    }
                    else
                    {
                        builder.Append(Unsigned_Int64.ToString());
                    }
                    break;
                case FieldType.CONTAINER:
                    if (Container != null)
                    {
                        Container.Build(ref builder, prettyIndent);
                    }
                    else
                    {
                        builder.Append(NULL);
                    }
                    break;
                case FieldType.BOOL:
                    if (Boolean)
                    {
                        builder.Append(BOOL_TRUE);
                    }
                    else
                    {
                        builder.Append(BOOL_FALSE);
                    }
                    break;
            }
        }

        #endregion
        #region Parse

        private const string UNEXPECTEDFIELDSTARTERRORFORMAT = "Unexpected character {0}. Expected field!";

        internal static bool TryParseProgressive(string str, string fieldIdentifier, out JSONField field, ref int index, out string errormessage, Stack<JSONContainer> containerStack)
        {
            field = null;
            errormessage = string.Empty;
            int fieldValueStart = index;
            bool valueIsInteger = true;

            FieldParseStep parseStep = FieldParseStep.Seeking;
            for (; index < str.Length; index++)
            {
                char current = str[index];
                switch (parseStep)
                {
                    case FieldParseStep.Seeking:
                        {
                            if (!JSONHelper.IsWhiteSpace(current))
                            {
                                switch (current)
                                {
                                    case '[':
                                    case '{':
                                        {
                                            if (!JSONContainer.TryParseObjectOrArray(str, out JSONContainer fieldObj, ref index, out errormessage, containerStack))
                                            {
                                                return false;
                                            }
                                            else
                                            {
                                                field = new JSONField(fieldObj, fieldIdentifier);
                                                return true;
                                            }
                                        }
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
                                    case '.':
                                        {
                                            fieldValueStart = index;
                                            parseStep = FieldParseStep.ParseNumber;
                                        }
                                        break;
                                    case '\"':
                                        {
                                            fieldValueStart = index + 1;
                                            parseStep = FieldParseStep.ParseString;
                                        }
                                        break;
                                    case 't':
                                    case 'f':
                                    case 'n':
                                        {
                                            string valueString;
                                            if (str.Length - index >= 4)
                                            {
                                                valueString = str.Substring(index, 4);
                                                if (valueString == JSONHelper.TRUE)
                                                {
                                                    field = new JSONField(true, fieldIdentifier);
                                                    index += 4;
                                                    return true;
                                                }
                                                else if (valueString == JSONHelper.NULL)
                                                {
                                                    field = new JSONField
                                                    {
                                                        Identifier = fieldIdentifier
                                                    };
                                                    index += 4;
                                                    return true;
                                                }
                                            }
                                            if (str.Length - index >= 5)
                                            {
                                                if (str.Substring(index, 5) == JSONHelper.FALSE)
                                                {
                                                    field = new JSONField(false, fieldIdentifier);
                                                    index += 5;
                                                    return true;
                                                }
                                            }
                                            errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, string.Format(UNEXPECTEDFIELDSTARTERRORFORMAT, current), containerStack);
                                            return false;
                                        }
                                    default:
                                        {
                                            errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, string.Format(UNEXPECTEDFIELDSTARTERRORFORMAT, current), containerStack);
                                            return false;
                                        }
                                }
                            }
                        }
                        break;
                    case FieldParseStep.ParseString:
                        {
                            if (current == '\\')
                            {
                                index++;
                            }
                            else if (current == '\"')
                            {
                                if (index > fieldValueStart)
                                {
                                    string value = str.Substring(fieldValueStart, index - fieldValueStart);
                                    field = new JSONField(JSONHelper.GetOriginalFormat(value), fieldIdentifier);
                                }
                                else
                                {
                                    field = new JSONField(string.Empty, fieldIdentifier);
                                }
                                index++;
                                return true;
                            }
                        }
                        break;
                    case FieldParseStep.ParseNumber:
                        {
                            string value;
                            switch (current)
                            {
                                case '}':
                                case ']':
                                case ',':
                                    value = str.Substring(fieldValueStart, index - fieldValueStart);
                                    if (valueIsInteger)
                                    {
                                        if (value.StartsWith('-'))
                                        {
                                            if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out long val))
                                            {
                                                field = new JSONField(val, fieldIdentifier);
                                                return true;
                                            }
                                        }
                                        else if (ulong.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong val))
                                        {
                                            field = new JSONField(val, fieldIdentifier);
                                            return true;
                                        }
                                    }
                                    else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                                    {
                                        field = new JSONField(val, fieldIdentifier);
                                        return true;
                                    }
                                    int substrStart = Math.Max(0, index - 5);
                                    errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, "Unable to parse \"{value}\" to number value!", containerStack);
                                    return false;
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
                                    continue;
                                case '.':
                                    if (valueIsInteger)
                                    {
                                        valueIsInteger = false;
                                    }
                                    break;
                                default:
                                    if (JSONHelper.IsWhiteSpace(current))
                                    {
                                        value = str.Substring(fieldValueStart, index - fieldValueStart);
                                        if (valueIsInteger)
                                        {
                                            if (value.StartsWith('-'))
                                            {
                                                if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out long val))
                                                {
                                                    field = new JSONField(val, fieldIdentifier);
                                                }
                                            }
                                            else if (ulong.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong val))
                                            {
                                                field = new JSONField(val, fieldIdentifier);
                                            }
                                        }
                                        else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                                        {
                                            field = new JSONField(val, fieldIdentifier);
                                        }
                                        if (field == null)
                                        {
                                            errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, "Unable to parse \"{value}\" to number value!", containerStack);
                                            return false;
                                        }
                                        parseStep = FieldParseStep.SeekingEnd;
                                    }
                                    else
                                    {
                                        errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, "Expected decimal number or point!", containerStack);
                                        return false;
                                    }
                                    break;
                            }
                        }
                        break;
                    case FieldParseStep.SeekingEnd:
                        {
                            if (current == ',')
                            {
                                index++;
                                return true;
                            }
                            else if (current == '}' || current == ']')
                            {

                                return true;
                            }
                            else if (JSONHelper.IsWhiteSpace(current))
                            {
                                continue;
                            }
                            else
                            {
                                errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Field, "Expected whitespace or field scope end (',', ']' or '}')!", containerStack);
                                return false;
                            }
                        }
                }
            }
            errormessage = "Unexpected ending of input string";
            return false;
        }

        internal enum FieldParseStep
        {
            Seeking,
            ParseString,
            ParseNumber,
            ParseKeyword,
            ParseObject,
            SeekingEnd
        }

        #endregion
    }
}
