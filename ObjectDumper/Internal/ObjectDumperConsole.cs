using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal class ObjectDumperConsole : DumperBase
    {
        public ObjectDumperConsole(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object element, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            var instance = new ObjectDumperConsole(dumpOptions);

            instance.FormatValue(element);

            return instance.ToString();
        }

        private void CreateObject(object o, int intentLevel = 0)
        {
            this.AddAlreadyTouched(o);

            var type = o.GetType();

            this.Write($"{{{type.GetFormattedName(this.DumpOptions.UseTypeFullName)}}}", intentLevel);
            this.LineBreak();
            this.Level++;

            var properties = type.GetRuntimeProperties()
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

                var indexParameters = property.GetIndexParameters();
                if (indexParameters.Length > 0)
                {
                    if (!this.DumpOptions.IgnoreIndexers)
                    {
                        DumpIntegerArrayIndexer(o, property, indexParameters);
                    }
                }
                else
                {
                    this.Write($"{property.Name}: ");
                    this.FormatValue(value);
                    if (!Equals(property, last))
                    {
                        this.LineBreak();
                    }
                }
            }

            this.Level--;
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
                    this.Write($"[{arrayIndex}]: ");
                    FormatValue(arrayValue);
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
                    this.Write($"MinValue", intentLevel);
                }
                else if (@short == short.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
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
                    this.Write($"MaxValue", intentLevel);
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
                    this.Write($"MinValue", intentLevel);
                }
                else if (@int == int.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
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
                    this.Write($"MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@uint.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is long @long)
            {
                if (@long == long.MinValue)
                {
                    this.Write($"MinValue", intentLevel);
                }
                else if (@long == long.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@long.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is ulong @ulong)
            {
                // No special handling for MinValue

                if (@ulong == ulong.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@ulong.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is double @double)
            {
                if (@double == double.MinValue)
                {
                    this.Write($"MinValue", intentLevel);
                }
                else if (@double == double.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
                }
                else if (double.IsNaN(@double))
                {
                    this.Write($"NaN", intentLevel);
                }
                else if (double.IsPositiveInfinity(@double))
                {
                    this.Write($"PositiveInfinity", intentLevel);
                }
                else if (double.IsNegativeInfinity(@double))
                {
                    this.Write($"NegativeInfinity", intentLevel);
                }
                else
                {
                    this.Write($"{@double.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is decimal @decimal)
            {
                if (@decimal == decimal.MinValue)
                {
                    this.Write($"MinValue", intentLevel);
                }
                else if (@decimal == decimal.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@decimal.ToString(CultureInfo.InvariantCulture)}", intentLevel);
                }

                return;
            }

            if (o is float @float)
            {
                if (@float == float.MinValue)
                {
                    this.Write($"MinValue", intentLevel);
                }
                else if (@float == float.MaxValue)
                {
                    this.Write($"MaxValue", intentLevel);
                }
                else
                {
                    this.Write($"{@float.ToString(CultureInfo.InvariantCulture)}", intentLevel);
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
                    this.Write($"{dateTime}", intentLevel);
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
                    this.Write($"{dateTimeOffset:O}", intentLevel);
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
                    this.Write($"{timeSpan:c}", intentLevel);
                }

                return;
            }

            if (o is CultureInfo cultureInfo)
            {
                this.Write($"{cultureInfo}", intentLevel);
                return;
            }

            var type = o.GetType();

            if (o is Enum)
            {
                var enumFlags = $"{o}".Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var enumTypeName = type.GetFormattedName(this.DumpOptions.UseTypeFullName);
                // In case of multiple flags, we prefer short class name here
                var enumValues = string.Join(" | ", enumFlags.Select(f => $"{(enumFlags.Length > 1 ? "" : $"{enumTypeName}.")}{f.Replace(" ", "")}"));
                this.Write($"{enumValues}", intentLevel);
                return;
            }

            if (o is Guid guid)
            {
                this.Write($"{guid:B}", intentLevel);
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

                this.Write($"{systemType.GetFormattedName(this.DumpOptions.UseTypeFullName)}", intentLevel);
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

#if NETSTANDARD_2
            if (type.IsValueTuple())
            {
                WriteValueTuple(o, type);
                return;
            }
#endif

            if (o is IEnumerable enumerable)
            {
                if (this.Level > 0)
                {
                    this.Write($"...", intentLevel);
                    this.Level++;
                }

                this.WriteItems(enumerable);
                return;
            }

            this.CreateObject(o, intentLevel);
        }

#if NETSTANDARD_2
        protected void WriteValueTuple(object o, Type type)
        {
            var fields = type.GetFields().ToList();
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
#endif

        private void WriteItems(IEnumerable items)
        {
            if (this.IsMaxLevel())
            {
                this.Level--;
                return;
            }

            var e = items.GetEnumerator();
            if (e.MoveNext())
            {
                if (this.Level > 0)
                {
                    this.LineBreak();
                }
                this.FormatValue(e.Current, this.Level);

                while (e.MoveNext())
                {
                    this.LineBreak();

                    this.FormatValue(e.Current, this.Level);
                }

                //this.LineBreak();
            }

            if (Level > 0)
            {
                this.Level--;
            }
        }
    }
}
