using System.Collections;
using System.Diagnostics.Extensions;
using System.Linq;
using System.Reflection;

namespace System.Diagnostics
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    internal class ObjectDumperCSharp : DumperBase
    {
        public ObjectDumperCSharp(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object element, DumpOptions dumpOptions = default(DumpOptions))
        {
            var instance = new ObjectDumperCSharp(dumpOptions);
            if (element == null)
            {
                instance.Write("null");
            }
            else
            {
                instance.Write($"var {GetVariableName(element)} = ");
                instance.FormatValue(element);
                instance.Write(";");
            }

            return instance.ToString();
        }

        private void CreateObject(object o, int? intentLevel = null)
        {
            this.Write($"new {GetClassName(o)}", intentLevel);
            this.LineBreak();
            this.StartLine("{");
            this.LineBreak();
            this.Level++;

            var properties = o.GetType().GetRuntimeProperties()
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic && p.GetMethod.IsStatic == false)
                .ToList();

            if (this.DumpOptions.SetPropertiesOnly)
            {
                properties = properties
                    .Where(p => p.SetMethod != null && p.SetMethod.IsPublic && p.SetMethod.IsStatic == false)
                    .ToList();
            }

            var last = properties.LastOrDefault();

            foreach (var property in properties)
            {
                var value = property.GetValue(o);
                if (value != null)
                {
                    this.StartLine($"{property.Name} = ");
                    this.FormatValue(value);
                    if (!Equals(property, last))
                    {
                        this.Write(",");
                    }

                    this.LineBreak();
                }
            }

            this.Level--;
            this.StartLine("}");
        }

        /*
        private string DumpElement(object element)
        {
            this.FormatValue(element);
            return "";
            if (element == null || element is ValueType || element is string)
            {
                this.FormatValue(element);
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.Write($"new {objectType.Namespace}.{objectType.Name}()");
                    this.Write("{");
                    this.AddAlreadyTouched(element);
                    this.Level++;
                }

                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (var item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            this.Level++;
                            this.DumpElement(item);
                            this.Level--;
                            this.Write("}");
                        }
                        else
                        {
                            if (!this.AlreadyTouched(item))
                            {
                                this.DumpElement(item);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
                            }
                        }
                    }
                }
                else
                {
                    var publicFields = element.GetType().GetRuntimeFields().Where(f => !f.IsPrivate);
                    foreach (var fieldInfo in publicFields)
                    {
                        object value;
                        try
                        {
                            value = fieldInfo.GetValue(element);
                        }
                        catch (Exception ex)
                        {
                            value = $"{{{ex.Message}}}";
                        }

                        if (fieldInfo.FieldType.GetTypeInfo().IsValueType || fieldInfo.FieldType == typeof(string))
                        {
                            this.Write("{0} = ", fieldInfo.Name);
                            this.FormatValue(value);
                            this.Write(",\n\r");
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo()
                                .IsAssignableFrom(fieldInfo.FieldType.GetTypeInfo());
                            this.Write("{0} = {1}", fieldInfo.Name, isEnumerable ? "" : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this.Level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }

                            this.Level--;
                            this.Write("}");
                        }
                    }

                    var publicProperties = element.GetType().GetRuntimeProperties()
                        .Where(p => p.GetMethod != null && p.GetMethod.IsStatic == false);
                    foreach (var propertyInfo in publicProperties)
                    {
                        var type = propertyInfo.PropertyType;
                        object value;
                        try
                        {
                            value = propertyInfo.GetValue(element, null);
                        }
                        catch (Exception ex)
                        {
                            value = $"{{{ex.Message}}}";
                        }

                        if (type.GetTypeInfo().IsValueType || type == typeof(string))
                        {
                            this.Write("{0} = ", propertyInfo.Name);
                            this.FormatValue(value);
                            this.Write(",\n\r");
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
                            this.Write("{0} = {1}", propertyInfo.Name, isEnumerable ? "" : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this.Level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }

                            this.Level--;
                            this.Write("}");
                        }
                    }
                }

                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.Level--;
                    this.Write("}");
                }
            }

            return this.ToString();
        }
        */

        private void FormatValue(object o, int? intentLevel = null)
        {
            if (o is bool)
            {
                this.Write($"{o.ToString().ToLower()}", intentLevel);
                return;
            }

            if (o is string)
            {
                this.Write("\"" + $@"{o}" + "\"", intentLevel);
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
                this.Write($"{o}l", intentLevel);
                return;
            }

            if (o is short || o is ushort)
            {
                this.Write($"{o}", intentLevel);
                return;
            }

            if (o is DateTime)
            {
                this.Write($"DateTime.Parse(\"{o}\")", intentLevel);
                return;
            }

            if (o is Enum)
            {
                this.Write($"{o.GetType().FullName}.{o}", intentLevel);
                return;
            }

            if (o is IEnumerable)
            {
                this.Write($"new {GetClassName(o)}", intentLevel);
                this.LineBreak();
                this.StartLine("{");
                this.LineBreak();
                this.WriteItems((IEnumerable)o);
                this.StartLine("}");
                return;
            }

            this.CreateObject(o, intentLevel);
        }

        private void WriteItems(IEnumerable items)
        {
            this.Level++;
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
            var className = GetClassName(element);
            var variableName = className.ToLowerFirst().Replace("<", "").Replace(">", "");
            return variableName;
        }
    }
}