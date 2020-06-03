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
                instance.Write($"var {GetVariableName(element)} = ");
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

            this.Write($"new {GetClassName(o)}", intentLevel);
            this.LineBreak();
            this.Write("{");
            this.LineBreak();
            this.Level++;

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

            if (this.DumpOptions.IgnoreDefaultValues)
            {
                properties = properties
                    .Where(p =>
                    {
                        var value = p.GetValue(o);
                        var defaultValue = p.PropertyType.GetDefault();
                        var isDefaultValue = Equals(value, defaultValue);
                        return !isDefaultValue;
                    })
                    .ToList();
            }

            if (this.DumpOptions.PropertyOrderBy != null)
            {
                properties = properties.OrderBy(this.DumpOptions.PropertyOrderBy.Compile())
                    .ToList();
            }

            var last = properties.LastOrDefault();

            foreach (var property in properties)
            {
                var value = property.TryGetValue(o);

                if (this.AlreadyTouched(value))
                {
                    continue;
                }

                this.Write($"{property.Name} = ");
                this.FormatValue(value);
                if (!Equals(property, last))
                {
                    this.Write(",");
                }

                this.LineBreak();
            }

            this.Level--;
            this.Write("}");
        }

        private void FormatValue(object o, int intentLevel = 0)
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

            if (o is double)
            {
                this.Write($"{o}d", intentLevel);
                return;
            }

            if (o is decimal)
            {
                this.Write($"{o}m", intentLevel);
                return;
            }

            if (o is byte || o is sbyte)
            {
                this.Write($"{o}", intentLevel);
                return;
            }

            if (o is float)
            {
                this.Write($"{o}f", intentLevel);
                return;
            }

            if (o is int || o is uint)
            {
                this.Write($"{o}", intentLevel);
                return;
            }

            if (o is long || o is ulong)
            {
                this.Write($"{o}L", intentLevel);
                return;
            }

            if (o is short || o is ushort)
            {
                this.Write($"{o}", intentLevel);
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

            if (o is Enum)
            {
                this.Write($"{o.GetType().FullName}.{o}", intentLevel);
                return;
            }

            if (o is Guid guid)
            {
                this.Write($"new Guid(\"{guid:D}\")", intentLevel);
                return;
            }

            var type = o.GetType();
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

                this.Write($"typeof({systemType.FullName})", intentLevel);
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

            if (o is IEnumerable enumerable)
            {
                this.Write($"new {GetClassName(o)}", intentLevel);
                this.LineBreak();
                this.Write("{");
                this.LineBreak();
                this.WriteItems(enumerable);
                this.Write("}");
                return;
            }

            this.CreateObject(o, intentLevel);
        }

        private void WriteItems(IEnumerable items)
        {
            this.Level++;
            if (this.IsMaxLevel())
            {
                ////this.StartLine("// Omitted code");
                ////this.LineBreak();
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

        private static string GetClassName(object o)
        {
            var type = o.GetType();
            var className = type.GetFormattedName();
            return className;
        }

        private static string GetVariableName(object element)
        {
            if (element == null)
            {
                return "x";
            }

            var className = GetClassName(element);
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
                    .Replace(">", "s")
                    .Replace(" ", "")
                    .Replace("[", "Array")
                    .Replace("]", "")
                    ;
            }

            return variableName.ToLowerFirst();
        }
    }
}
