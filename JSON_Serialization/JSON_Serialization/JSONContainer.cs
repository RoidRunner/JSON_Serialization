using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JSON
{
    public class JSONContainer
    {
        #region Fields and Properties

        private Dictionary<string, JSONField> fields = new Dictionary<string, JSONField>();
        private List<JSONField> array = new List<JSONField>();

        /// <summary>
        /// Container Type of this container
        /// </summary>
        public ContainerType Type { get; private set; } = ContainerType.EMPTY;
        /// <summary>
        /// True, if container is of type array. False, otherwise
        /// </summary>
        public bool IsArray => Type == ContainerType.ARRAY;
        /// <summary>
        /// True, if container is of type object. False, otherwise
        /// </summary>
        public bool IsObject => Type == ContainerType.OBJECT;
        /// <summary>
        /// Read-only collection of all fields contained in this container
        /// </summary>
        public IReadOnlyCollection<JSONField> Fields => fields.Values;
        /// <summary>
        /// Creates a read-only list based on the Fields collection
        /// </summary>
        public IReadOnlyList<JSONField> FieldsAsList()
        {
            return new List<JSONField>(fields.Values).AsReadOnly();
        }
        /// <summary>
        /// Read-only list of array listings
        /// </summary>
        public IReadOnlyList<JSONField> Array => array.AsReadOnly();

        #endregion
        #region Constructors

        private JSONContainer()
        {

        }

        /// <summary>
        /// Creates a new JSONContainer Object
        /// </summary>
        public static JSONContainer NewObject()
        {
            return new JSONContainer() { Type = ContainerType.OBJECT }; 
        }

        /// <summary>
        /// Creates a new JSONContainer Array
        /// </summary>
        public static JSONContainer NewArray()
        {
            return new JSONContainer() { Type = ContainerType.ARRAY };
        }

        /// <summary>
        /// Creates a new JSONContainer Array
        /// </summary>
        /// <param name="fields">JSONField IEnumerable of all fields the array is to be initialized with</param>
        public static JSONContainer NewArray(IEnumerable<JSONField> fields)
        {
            return new JSONContainer()
            {
                Type = ContainerType.ARRAY,
                array = new List<JSONField>(fields)
            };
        }

        #endregion
        #region TryGetField

        /// <summary>
        /// Attempts to retrieve a field
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="field">JSONField object containing the returned field info</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out JSONField field)
        {
            return fields.TryGetValue(identifier, out field);
        }
        /// <summary>
        /// Attempts to retrieve a field as a boolean value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Boolean value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out bool val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Boolean;
                return field.IsBool;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a string value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">String value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out string val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.String;
                return field.IsString;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a character value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Character value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out char val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Character;
                return field.IsString;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a unsigned 64-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Unsigned 64-bit integer value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out ulong val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Unsigned_Int64;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a signed 64-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Signed 64-bit integer value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out long val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Signed_Int64;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a unsigned 32-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Unsigned 32-bit integer value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out uint val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Unsigned_Int32;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a signed 32-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">Signed 32-bit integer value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out int val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Signed_Int32;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a 64-bit floating point value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">64-bit floating point value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out double val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Float64;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a 32-bit floating point value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">32-bit floating point value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out float val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Float32;
                return field.IsNumber;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a JSONField list value from an array container
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">JSONField list value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out IReadOnlyList<JSONField> val)
        {
            if (TryGetField(identifier, out JSONContainer container))
            {
                val = container.Array;
                return container.Type == ContainerType.ARRAY;
            }
            val = default;
            return false;
        }
        /// <summary>
        /// Attempts to retrieve a field as a JSONContainer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be retrieved</param>
        /// <param name="val">JSONContainer value stored in the field</param>
        /// <returns>True, if a matching field was obtained. False, otherwise</returns>
        public bool TryGetField(string identifier, out JSONContainer val)
        {
            if (TryGetField(identifier, out JSONField field))
            {
                val = field.Container;
                return field.IsObject;
            }
            val = default;
            return false;
        }

        #endregion
        #region Object AddField

        private bool AssureContainerType(ContainerType type)
        {
            if (this.Type == type)
            {
                return true;
            }
            else if (this.Type == ContainerType.EMPTY)
            {
                this.Type = type;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Attempts to add a generic field
        /// </summary>
        /// <param name="field">JSONField object storing all information</param>
        /// <returns>True, if field was successfully added. Adding a field will fail if a duplicate or empty identifier is encountered. False, otherwise</returns>
        public bool TryAddField(JSONField field)
        {
            if (!AssureContainerType(ContainerType.OBJECT))
            {
                throw new InvalidOperationException("Cannot add a field to an array object!");
            }
            if (string.IsNullOrEmpty(field.Identifier))
            {
                return false;
            }
            return fields.TryAdd(field.Identifier, field);
        }
        /// <summary>
        /// Attempts to add a field containing a boolean value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Boolean value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, bool val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing a string value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">String value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, string val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing a character value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Character value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, char val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing an unsigned 64-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Unsigned 64-bit integer value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, ulong val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing an signed 64-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Signed 64-bit integer value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, long val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing an unsigned 32-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Unsigned 32-bit integer value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, uint val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing an signed 32-bit integer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">Signed 32-bit integer value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, int val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing a 64-bit floating point value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">64-bit floating point value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, double val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing a 32-bit floating point value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">32-bit floating point value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, float val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing an array of JSONField objects
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">JSONField object IEnumerable to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, IEnumerable<JSONField> val)
        {
            return TryAddField(new JSONField(val, identifier));
        }
        /// <summary>
        /// Attempts to add a field containing a JSONContainer value
        /// </summary>
        /// <param name="identifier">Key/Name/Identifier identifying the field to be added</param>
        /// <param name="val">JSONContainer value to be stored in the new field</param>
        /// <returns>True, if field was successfully added. False, otherwise</returns>
        public bool TryAddField(string identifier, JSONContainer val)
        {
            return TryAddField(new JSONField(val, identifier));
        }

        #endregion
        #region Array Adds

        /// <summary>
        /// Adds a JSONField object to array
        /// </summary>
        /// <param name="field">JSONField value to add to the array</param>
        public void Add(JSONField field)
        {
            if (!AssureContainerType(ContainerType.ARRAY))
            {
                throw new InvalidOperationException("Cannot add an array value to an object!");
            }
            array.Add(field);
        }
        /// <summary>
        /// Adds a boolean value to array
        /// </summary>
        /// <param name="val">Boolean value to add to the array</param>
        public void Add(bool val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a string value to array
        /// </summary>
        /// <param name="val">String value to add to the array</param>
        public void Add(string val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a character value to array
        /// </summary>
        /// <param name="val">Character value to add to the array</param>
        public void Add(char val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds an unsigned 64-bit integer value to array
        /// </summary>
        /// <param name="val">Unsigned 64-bit integer value to add to the array</param>
        public void Add(ulong val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a signed 64-bit integer value to array
        /// </summary>
        /// <param name="val">Signed 64-bit integer value to add to the array</param>
        public void Add(long val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds an unsigned 32-bit integer value to array
        /// </summary>
        /// <param name="val">Unsigned 32-bit integer value to add to the array</param>
        public void Add(uint val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a Signed 32-bit integer value to array
        /// </summary>
        /// <param name="val">Signed 32-bit integer value to add to the array</param>
        public void Add(int val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds an 64-bit floating point value to array
        /// </summary>
        /// <param name="val">64-bit floating point value to add to the array</param>
        public void Add(double val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds an 32-bit floating point value to array
        /// </summary>
        /// <param name="val">32-bit floating point value to add to the array</param>
        public void Add(float val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a JSONContainer value to array
        /// </summary>
        /// <param name="val">JSONContainer value to add to the array</param>
        public void Add(JSONContainer val)
        {
            Add(new JSONField(val));
        }
        /// <summary>
        /// Adds a JSONContainer containing an array value to array
        /// </summary>
        /// <param name="val">JSONField IEnumerable value to add to the array as an array container</param>
        public void AddRange(IEnumerable<JSONField> val)
        {
            foreach (JSONField field in val)
            {
                Add(field);
            }
        }


        public override string ToString()
        {
            switch (Type)
            {
                case ContainerType.OBJECT:
                    return $"Object:({string.Join(", ", Fields)})";
                case ContainerType.ARRAY:
                    return $"Array:[{string.Join(", ", Array)}]";
                default:
                    return JSONHelper.NULL;
            }
        }

        #endregion
        #region Build

        internal void Build(ref StringBuilder builder, int prettyIndent = -1)
        {
            bool pretty = prettyIndent >= 0;
            for (int i = 0; i < prettyIndent; i++)
            {
                builder.Append("  ");
            }
            switch (Type)
            {
                case ContainerType.EMPTY:
                    builder.Append(JSONHelper.NULL);
                    break;
                case ContainerType.OBJECT:
                    {
                        int fieldIndent;
                        if (pretty)
                        {
                            builder.Append("{\n");
                            fieldIndent = prettyIndent + 1;
                        }
                        else
                        {
                            builder.Append('{');
                            fieldIndent = -1;
                        }
                        List<JSONField> fields = new List<JSONField>(Fields);
                        for (int i = 0; i < fields.Count; i++)
                        {
                            for (int j = 0; j < fieldIndent; j++)
                            {
                                builder.Append("  ");
                            }
                            builder.Append('\"');
                            builder.Append(fields[i].Identifier);
                            builder.Append('\"');
                            if (pretty)
                            {
                                builder.Append(" : ");
                            }
                            else
                            {
                                builder.Append(':');
                            }
                            fields[i].Build(ref builder, fieldIndent);
                            if (i < fields.Count - 1)
                            {
                                if (pretty)
                                {
                                    builder.Append(",\n");
                                }
                                else
                                {
                                    builder.Append(',');
                                }
                            }
                        }
                        if (pretty)
                        {
                            builder.AppendLine();
                        }
                        for (int i = 0; i < prettyIndent; i++)
                        {
                            builder.Append("  ");
                        }
                        {
                            builder.Append('}');
                        }
                    }
                    break;
                case ContainerType.ARRAY:
                    {
                        int fieldIndent;
                        if (pretty)
                        {
                            builder.Append("[\n");
                            fieldIndent = prettyIndent + 1;
                        }
                        else
                        {
                            builder.Append('[');
                            fieldIndent = -1;
                        }
                        IReadOnlyList<JSONField> array = Array;
                        for (int i = 0; i < array.Count; i++)
                        {
                            for (int j = 0; j < fieldIndent; j++)
                            {
                                builder.Append("  ");
                            }
                            array[i].Build(ref builder, fieldIndent);
                            if (i < array.Count - 1)
                            {
                                if (pretty)
                                {
                                    builder.Append(",\n");
                                }
                                else
                                {
                                    builder.Append(',');
                                }
                            }
                        }
                        if (pretty)
                        {
                            builder.AppendLine();
                        }
                        for (int i = 0; i < prettyIndent; i++)
                        {
                            builder.Append("  ");
                        }
                        {
                            builder.Append(']');
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Builds the jsoncontainer into a json formatted string
        /// </summary>
        /// <param name="pretty">If set, will include whitespaces and linefeeds to make the resulting string easier to read at the cost of length</param>
        /// <returns>A json formatted string containing all data stored in this container</returns>
        public string Build(bool pretty = false)
        {
            StringBuilder builder = new StringBuilder();
            if (pretty)
            {
                Build(ref builder, 0);
            }
            else
            {
                Build(ref builder);
            }
            return builder.ToString();
        }

        #endregion
        #region Parsing

        /// <summary>
        /// Attempts to parse a JSONContainer from a json formatted string
        /// </summary>
        /// <param name="str">As JSON formatted input string</param>
        /// <param name="result">The parsed JSONContainer result</param>
        /// <param name="errormessage">A string containing a hint in case parsing fails</param>
        /// <returns>True, if parsing is successful. False, otherwise</returns>
        public static bool TryParse(string str, out JSONContainer result, out string errormessage)
        {
            str = JSONHelper.TrimStringForParsing(str);

            if (!string.IsNullOrEmpty(str))
            {
                int index = 0;
                return TryParseObjectOrArray(str, out result, ref index, out errormessage, new Stack<JSONContainer>());
            }
            else
            {
                result = null;
                errormessage = "Parsing String cannot be null or empty!";
                return false;
            }
        }

        private enum CollectionType
        {
            Unknown,
            Object,
            Array
        }

        internal static bool TryParseObjectOrArray(string str, out JSONContainer obj, ref int index, out string errormessage, Stack<JSONContainer> containerStack)
        {
            obj = null;
            errormessage = string.Empty;

            for (; index < str.Length; index++)
            {
                char current = str[index];
                if (current == '[')
                {
                    index++;
                    return TryParseArray(str, out obj, ref index, out errormessage, containerStack);
                }
                else if (current == '{')
                {
                    index++;
                    return TryParseObject(str, out obj, ref index, out errormessage, containerStack);
                }
                else if (JSONHelper.IsWhiteSpace(current))
                {
                    continue;
                }
                else
                {
                    errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Container, "Expected object ('{') or array ('[') scope start", containerStack);
                    return false;
                }
            }
            errormessage = "Unexpected ending of input string";
            return false;
        }

        private enum ObjectParseStep
        {
            SeekingFieldIdentifierOrScopeEnd,
            ParsingFieldIdentifier,
            SeekingColon,
            SeekingValue,
        }

        private static bool TryParseObject(string str, out JSONContainer obj, ref int index, out string errormessage, Stack<JSONContainer> containerStack)
        {
            obj = new JSONContainer();
            containerStack.Push(obj);
            errormessage = string.Empty;
            int fieldIdentifierStart = index;
            string fieldIdentifier = null;

            ObjectParseStep parseStep = ObjectParseStep.SeekingFieldIdentifierOrScopeEnd;

            for (; index < str.Length; index++)
            {
                char current = str[index];
                switch (parseStep)
                {
                    case ObjectParseStep.SeekingFieldIdentifierOrScopeEnd:
                        {
                            if (current == '\"')
                            {
                                fieldIdentifierStart = index + 1;
                                parseStep = ObjectParseStep.ParsingFieldIdentifier;
                            }
                            else if (current == '}')
                            {
                                index++;
                                containerStack.Pop();
                                return true;
                            }
                            else if (JSONHelper.IsWhiteSpace(current))
                            {
                                continue;
                            }
                            else
                            {
                                errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Object, "Expected field identifier start '\"' or object scope end '}'", containerStack);
                                return false;
                            }
                        }
                        break;
                    case ObjectParseStep.ParsingFieldIdentifier:
                        {
                            if (current == '\"')
                            {
                                if (index - 1 > fieldIdentifierStart)
                                {
                                    fieldIdentifier = str.Substring(fieldIdentifierStart, index - fieldIdentifierStart);
                                    parseStep = ObjectParseStep.SeekingColon;
                                }
                                else
                                {
                                    errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Object, "Encountered empty (length = 0) field identifier!", containerStack);
                                    return false;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        break;
                    case ObjectParseStep.SeekingColon:
                        {
                            if (current == ':')
                            {
                                parseStep = ObjectParseStep.SeekingValue;
                            }
                            else if (JSONHelper.IsWhiteSpace(current))
                            {
                                continue;
                            }
                            else
                            {
                                errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Object, "Expected colon separating field identifier and field value!", containerStack);
                                return false;
                            }
                        }
                        break;
                    case ObjectParseStep.SeekingValue:
                        {
                            if (JSONField.TryParseProgressive(str, fieldIdentifier, out JSONField field, ref index, out errormessage, containerStack))
                            {
                                if (!obj.TryAddField(field))
                                {
                                    errormessage = JSONHelper.GetErrorMessageString(str, index, JSONHelper.StructureType.Object, "Duplicate field identifier \"{fieldIdentifier}\"", containerStack);
                                    return false;
                                }
                                if (str[index] == '}')
                                {
                                    index++;
                                    containerStack.Pop();
                                    return true;
                                }
                                parseStep = ObjectParseStep.SeekingFieldIdentifierOrScopeEnd;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                }
            }
            errormessage = "Unexpected ending of input string";
            return false;
        }

        private enum ArrayParseStep
        {
            SeekingValueOrScopeEnd,
            SeekingCommaOrScopeEnd
        }

        private static bool TryParseArray(string str, out JSONContainer obj, ref int index, out string errormessage, Stack<JSONContainer> containerStack)
        {
            obj = new JSONContainer();
            obj.Type = ContainerType.ARRAY;
            containerStack.Push(obj);
            errormessage = string.Empty;

            for (; index < str.Length; index++)
            {
                char current = str[index];
                if (current == ']')
                {
                    index++;
                    containerStack.Pop();
                    return true;
                }
                else if (JSONHelper.IsWhiteSpace(current))
                {
                    continue;
                }
                else
                {
                    if (JSONField.TryParseProgressive(str, null, out JSONField field, ref index, out errormessage, containerStack))
                    {
                        obj.Add(field);
                    }
                    else
                    {
                        return false;
                    }
                    if (str[index] == ']')
                    {
                        index++;
                        containerStack.Pop();
                        return true;
                    }
                }
            }
            errormessage = "Unexpected ending of input string";
            return false;
        }

        #endregion
    }
}