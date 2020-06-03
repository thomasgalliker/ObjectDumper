using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
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
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (this.Level > this.DumpOptions.MaxLevel)
            {
                return this.ToString();
            }

            if (element == null || element is ValueType || element is string)
            {
                this.Write(this.FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.Write(GetClassName(element));
                    this.LineBreak();
                    this.AddAlreadyTouched(element);
                    this.Level++;
                }

                if (element is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            this.Level++;
                            this.DumpElement(item);
                            this.Level--;
                        }
                        else
                        {
                            if (!this.AlreadyTouched(item))
                            {
                                this.DumpElement(item);
                            }
                            else
                            {
                                this.Write($"{GetClassName(element)} <-- bidirectional reference found");
                                this.LineBreak();
                            }
                        }

                        this.LineBreak();
                    }
                }
                else
                {
                    var publicFields = element.GetType().GetRuntimeFields().Where(f => !f.IsPrivate);
                    foreach (var fieldInfo in publicFields)
                    {
                        var value = fieldInfo.TryGetValue(element);

                        if (fieldInfo.FieldType.GetTypeInfo().IsValueType || fieldInfo.FieldType == typeof(string))
                        {
                            this.Write($"{fieldInfo.Name}: {this.FormatValue(value)}");
                            this.LineBreak();
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo()
                                .IsAssignableFrom(fieldInfo.FieldType.GetTypeInfo());
                            this.Write($"{fieldInfo.Name}: {(isEnumerable ? "..." : (value != null ? "{ }" : "null"))}");
                            this.LineBreak();

                            if (value != null)
                            {
                                var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                                this.Level++;
                                if (!alreadyTouched)
                                {
                                    this.DumpElement(value);
                                }
                                else
                                {
                                    this.Write($"{GetClassName(element)} <-- bidirectional reference found");
                                    this.LineBreak();
                                }

                                this.Level--;
                            }
                        }
                    }

                    var properties = element.GetType().GetRuntimeProperties()
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
                                var value = p.GetValue(element);
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

                    foreach (var propertyInfo in properties)
                    {
                        var type = propertyInfo.PropertyType;
                        var value = propertyInfo.TryGetValue(element);

                        if (type.GetTypeInfo().IsValueType || type == typeof(string))
                        {
                            this.Write($"{propertyInfo.Name}: {this.FormatValue(value)}");
                            this.LineBreak();
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
                            this.Write($"{propertyInfo.Name}: {(isEnumerable ? "..." : (value != null ? "{ }" : "null"))}");
                            this.LineBreak();

                            if (value != null)
                            {
                                var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                                this.Level++;
                                if (!alreadyTouched)
                                {
                                    this.DumpElement(value);
                                }
                                else
                                {
                                    this.Write($"{GetClassName(element)} <-- bidirectional reference found");
                                    this.LineBreak();
                                }

                                this.Level--;
                            }
                        }
                    }
                }

                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.Level--;
                }
            }

            return this.ToString();
        }

        private string FormatValue(object o)
        {
            if (o == null)
            {
                return "null";
            }

            if (o is string)
            {
                return $"\"{o}\"";
            }

            if (o is char && (char)o == '\0')
            {
                return string.Empty;
            }

            if (o is ValueType)
            {
                return o.ToString();
            }
            
            if (o is CultureInfo)
            {
                return o.ToString();
            }

            if (o is IEnumerable)
            {
                return "...";
            }

            return "{ }";
        }

        private static string GetClassName(object element)
        {
            var type = element.GetType();
            var className = type.GetFormattedName(useFullName: true);
            return $"{{{className}}}";
        }
    }
}
