using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    internal class ObjectDumperCSharp : DumperBase
    {
        public ObjectDumperCSharp(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object element, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            var instance = new ObjectDumperCSharp(dumpOptions);
            if (!dumpOptions.TrimInitialVariableName)
            {
                instance.Write($"var {instance.GetVariableName(element)} = ");
            }

            instance.FormatValue(element);
            if (!dumpOptions.TrimTrailingColonName)
            {
                instance.Write(";");
            }

            return instance.ToString();
        }

        private void CreateObject(object o, int intentLevel = 0)
        {
            this.AddAlreadyTouched(o);

            var type = o.GetType();

            var typeName = type.IsAnonymous() ? "" : type.GetFormattedName(this.DumpOptions.UseTypeFullName);

            this.Write($"new{(string.IsNullOrEmpty(typeName) ? "" : " ")}{typeName}", intentLevel);
            this.LineBreak();
            this.Write("{");
            this.LineBreak();
            this.Level++;
            this.DumpProperties(o);
            this.Level--;
            this.Write("}");
        }

        private void DumpProperties(object o)
        {
            var properties = o.GetType().GetRuntimeProperties()
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic && p.GetMethod.IsStatic == false)
                .ToList();

            if (this.DumpOptions.ExcludeProperties != null && this.DumpOptions.ExcludeProperties.Any())
            {
                properties = properties
                    .Where(p => !this.DumpOptions.ExcludeProperties.Contains(p.Name))
                    .ToList();
            }

            if (this.DumpOptions.SetPropertiesOnly)
            {
                properties = properties
                    .Where(p => p.SetMethod != null && p.SetMethod.IsPublic && p.SetMethod.IsStatic == false)
                    .ToList();
            }

            if (this.DumpOptions.PropertyOrderBy != null)
            {
                properties = properties
                    .OrderBy(this.DumpOptions.PropertyOrderBy.Compile())
                    .ToList();
            }

            var propertiesAndValues = properties
                .Select(p => new PropertyAndValue(o, p))
                .ToList();

            PropertyAndValue lastProperty;
            if (this.DumpOptions.IgnoreDefaultValues)
            {
                lastProperty = propertiesAndValues.LastOrDefault(pv => !pv.IsDefaultValue);
            }
            else
            {
                lastProperty = propertiesAndValues.LastOrDefault();
            }

            foreach (var propertiesAndValue in propertiesAndValues)
            {
                var value = propertiesAndValue.Value;

                if (this.AlreadyTouched(value))
                {
                    this.Write($"{this.ResolvePropertyName(propertiesAndValue.Property.Name)} = ");
                    this.FormatValue(propertiesAndValue.DefaultValue);
                    if (!Equals(propertiesAndValue, lastProperty))
                    {
                        this.Write(",");
                    }
                    this.Write(" // Circular reference detected");
                    this.LineBreak();
                    continue;
                }

                if (this.DumpOptions.IgnoreDefaultValues)
                {
                    if (propertiesAndValue.IsDefaultValue)
                    {
                        continue;
                    }
                }

                var indexParameters = propertiesAndValue.Property.GetIndexParameters();
                if (indexParameters.Length > 0)
                {
                    if (!this.DumpOptions.IgnoreIndexers)
                    {
                        this.DumpIntegerArrayIndexer(o, propertiesAndValue.Property, indexParameters);
                    }
                }
                else
                {
                    this.Write($"{this.ResolvePropertyName(propertiesAndValue.Property.Name)} = ");
                    this.FormatValue(value);
                    if (!Equals(propertiesAndValue, lastProperty))
                    {
                        this.Write(",");
                    }

                    this.LineBreak();
                }
            }
        }

        private void DumpIntegerArrayIndexer(object o, PropertyInfo property, ParameterInfo[] indexParameters)
        {
            if (indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof(int))
            {
                // get an integer count value
                // issues, what if it's not an integer index (Dictionary?), what if it's multi-dimensional?
                // just need to be able to iterate through each value in the indexed property
                // Source: https://stackoverflow.com/questions/4268244/iterating-through-an-indexed-property-reflection

                var arrayValues = new List<object>();
                var index = 0;
                while (true)
                {
                    try
                    {
                        arrayValues.Add(property.GetValue(o, new object[] { index }));
                        index++;
                    }
                    catch (TargetInvocationException) { break; }
                }

                var lastArrayValue = arrayValues.LastOrDefault();

                for (var arrayIndex = 0; arrayIndex < arrayValues.Count; arrayIndex++)
                {
                    var arrayValue = arrayValues[arrayIndex];
                    this.Write($"[{arrayIndex}] = ");
                    this.FormatValue(arrayValue);
                    if (!Equals(arrayValue, lastArrayValue))
                    {
                        this.Write($",{this.DumpOptions.LineBreakChar}");
                    }
                    else
                    {
                        this.Write($",");
                    }
                }

                this.LineBreak();
            }
        }

        protected override void FormatValue(object o, int intentLevel = 0)
        {
            if (this.IsMaxLevel())
            {
                return;
            }

            if (o == null)
            {
                this.Write("null", intentLevel);
                return;
            }

            if (o is bool)
            {
                this.Write($"{o.ToString().ToLower()}", intentLevel);
                return;
            }

            if (o is string)
            {
                var str = $@"{o}".Escape();
                this.Write($"\"{str}\"", intentLevel);
                return;
            }

            if (o is char)
            {
                var c = o.ToString().Replace("\0", "").Trim();
                this.Write($"\'{c}\'", intentLevel);
                return;
            }

            if (o is byte || o is sbyte)
            {
                this.Write($"{o}", intentLevel);
                return;
            }

            if (o is short @short)
            {
                if (@short == short.MinValue)
                {
                    this.Write($"short.MinValue", intentLevel);
                }
                else if (@short == short.MaxValue)
                {
                    this.Write($"short.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@short.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is ushort @ushort)
            {
                // No special handling for MinValue

                if (@ushort == ushort.MaxValue)
                {
                    this.Write($"ushort.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@ushort.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is int @int)
            {
                if (@int == int.MinValue)
                {
                    this.Write($"int.MinValue", intentLevel);
                }
                else if (@int == int.MaxValue)
                {
                    this.Write($"int.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@int.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is uint @uint)
            {
                // No special handling for MinValue

                if (@uint == uint.MaxValue)
                {
                    this.Write($"uint.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@uint.ToString(CultureInfo.InvariantCulture)}u", intentLevel);
                }

                return;
            }

            if (o is long @long)
            {
                if (@long == long.MinValue)
                {
                    this.Write($"long.MinValue", intentLevel);
                }
                else if (@long == long.MaxValue)
                {
                    this.Write($"long.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@long.ToString(CultureInfo.InvariantCulture)}L", intentLevel);
                }

                return;
            }

            if (o is ulong @ulong)
            {
                // No special handling for MinValue

                if (@ulong == ulong.MaxValue)
                {
                    this.Write($"ulong.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@ulong.ToString(CultureInfo.InvariantCulture)}UL", intentLevel);
                }

                return;
            }

            if (o is double @double)
            {
                if (@double == double.MinValue)
                {
                    this.Write($"double.MinValue", intentLevel);
                }
                else if (@double == double.MaxValue)
                {
                    this.Write($"double.MaxValue", intentLevel);
                }
                else if (double.IsNaN(@double))
                {
                    this.Write($"double.NaN", intentLevel);
                }
                else if (double.IsPositiveInfinity(@double))
                {
                    this.Write($"double.PositiveInfinity", intentLevel);
                }
                else if (double.IsNegativeInfinity(@double))
                {
                    this.Write($"double.NegativeInfinity", intentLevel);
                }
                else
                {
                    this.Write($"{@double.ToString(CultureInfo.InvariantCulture)}d", intentLevel);
                }

                return;
            }

            if (o is decimal @decimal)
            {
                if (@decimal == decimal.MinValue)
                {
                    this.Write($"decimal.MinValue", intentLevel);
                }
                else if (@decimal == decimal.MaxValue)
                {
                    this.Write($"decimal.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@decimal.ToString(CultureInfo.InvariantCulture)}m", intentLevel);
                }

                return;
            }

            if (o is float @float)
            {
                if (@float == float.MinValue)
                {
                    this.Write($"float.MinValue", intentLevel);
                }
                else if (@float == float.MaxValue)
                {
                    this.Write($"float.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@float.ToString(CultureInfo.InvariantCulture)}f", intentLevel);
                }

                return;
            }

            if (o is DateTime dateTime)
            {
                if (dateTime == DateTime.MinValue)
                {
                    this.Write($"DateTime.MinValue", intentLevel);
                }
                else if (dateTime == DateTime.MaxValue)
                {
                    this.Write($"DateTime.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"DateTime.ParseExact(\"{dateTime:O}\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)", intentLevel);
                }

                return;
            }

            if (o is DateTimeOffset dateTimeOffset)
            {
                if (dateTimeOffset == DateTimeOffset.MinValue)
                {
                    this.Write($"DateTimeOffset.MinValue", intentLevel);
                }
                else if (dateTimeOffset == DateTimeOffset.MaxValue)
                {
                    this.Write($"DateTimeOffset.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"DateTimeOffset.ParseExact(\"{dateTimeOffset:O}\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)", intentLevel);
                }

                return;
            }

            if (o is TimeSpan timeSpan)
            {
                if (timeSpan == TimeSpan.Zero)
                {
                    this.Write($"TimeSpan.Zero", intentLevel);
                }
                else if (timeSpan == TimeSpan.MinValue)
                {
                    this.Write($"TimeSpan.MinValue", intentLevel);
                }
                else if (timeSpan == TimeSpan.MaxValue)
                {
                    this.Write($"TimeSpan.MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"TimeSpan.ParseExact(\"{timeSpan:c}\", \"c\", CultureInfo.InvariantCulture, TimeSpanStyles.None)", intentLevel);
                }

                return;
            }

            if (o is CultureInfo cultureInfo)
            {
                this.Write($"new CultureInfo(\"{cultureInfo}\")", intentLevel);
                return;
            }

            var type = o.GetType();

            if (o is Enum)
            {
                var enumTypeName = type.GetFormattedName(this.DumpOptions.UseTypeFullName);
                var enumFlags = $"{o}".Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var enumValues = string.Join(" | ", enumFlags.Select(f => $"{enumTypeName}.{f.Replace(" ", "")}"));
                this.Write($"{enumValues}", intentLevel);
                return;
            }

            if (o is Guid guid)
            {
                this.Write($"new Guid(\"{guid:D}\")", intentLevel);
                return;
            }

            if (this.DumpOptions.CustomInstanceFormatters.TryGetFormatter(type, out var func))
            {
                this.Write(func(o));
                return;
            }

            if (o is Type systemType)
            {
                if (this.DumpOptions.CustomTypeFormatter.TryGetValue(systemType, out var formatter) ||
                    this.DumpOptions.CustomTypeFormatter.TryGetValue(typeof(Type), out formatter))
                {
                    this.Write(formatter(systemType));
                    return;
                }

                this.Write($"typeof({systemType.GetFormattedName(this.DumpOptions.UseTypeFullName)})", intentLevel);
                return;
            }
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var kvpKey = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Key)).GetValue(o, null);
                var kvpValue = type.GetRuntimeProperty(nameof(KeyValuePair<object, object>.Value)).GetValue(o, null);

                this.Write("{ ", intentLevel);
                this.FormatValue(kvpKey);
                this.Write(", ");
                this.FormatValue(kvpValue);
                this.Write(" }");
                return;
            }

#if NETSTANDARD2_0_OR_GREATER
            if (type.IsValueTuple())
            {
                this.WriteValueTuple(o, type);
                return;
            }
#endif

            if (o is IEnumerable enumerable)
            {
                var typeName = type.GetFormattedName(this.DumpOptions.UseTypeFullName);
                this.Write($"new{(string.IsNullOrEmpty(typeName) ? "" : " ")}{typeName}", intentLevel);
                this.LineBreak();
                this.Write("{");
                this.LineBreak();
                this.WriteItems(enumerable);
                this.Write("}");
                return;
            }

            this.CreateObject(o, intentLevel);
        }

#if NETSTANDARD2_0_OR_GREATER
        private void WriteValueTuple(object o, Type type)
        {
            var fields = type.GetFields().ToList();
            if (fields.Any())
            {
                var last = fields.LastOrDefault();

                this.Write("(");
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(o);
                    this.FormatValue(fieldValue, 0);
                    if (!Equals(field, last))
                    {
                        this.Write(", ");
                    }
                }
                this.Write(")");
            }
            else
            {
                this.Write("ValueTuple.Create()");
            }
        }
#endif

        private void WriteItems(IEnumerable items)
        {
            this.Level++;
            if (this.IsMaxLevel())
            {
                this.Level--;
                return;
            }

            var e = items.GetEnumerator();
            if (e.MoveNext())
            {
                this.FormatValue(e.Current, this.Level);

                while (e.MoveNext())
                {
                    this.Write(",");
                    this.LineBreak();

                    this.FormatValue(e.Current, this.Level);
                }

                this.LineBreak();
            }

            this.Level--;
        }

        private string GetVariableName(object element)
        {
            if (element == null)
            {
                return "x";
            }

            var type = element.GetType();
            if (type.IsAnonymous())
            {
                return "x";
            }

            var className = type.GetFormattedName(useFullName: false, useValueTupleFormatting: false);
            string variableName;

            var splitGenerics = className.Split('<');

            if (splitGenerics.Length > 2 || className.Contains(','))
            {
                // Complex generics and multi-dimensional arrays
                // are using simple variable names
                variableName = splitGenerics[0];
            }
            else
            {
                // Simple generics, nullable types and one-dimensional arrays
                // are using more sophisticated variable names
                variableName = className
                    .Replace("Nullable<", "OfNullable")
                    .Replace("<", "Of")
                    .Replace("<", "Of")
                    .Replace(">", "s")
                    .Replace(" ", "")
                    .Replace("[", "Array")
                    .Replace("]", "")
                    ;
            }

            if (TypeExtensions.IsKeyword(variableName))
            {
                variableName += "Value";
            }

            return variableName.ToLowerFirst();
        }
    }
}
