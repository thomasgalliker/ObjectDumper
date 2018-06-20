using System.Collections;
using System.Diagnostics.Tests;
using System.Linq;
using System.Reflection;

namespace System.Diagnostics
{
    /// <summary>
    ///     Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    public class ObjectDumper : DumperBase
    {
        public ObjectDumper(DumpOptions dumpOptions) : base(dumpOptions)
        {
        }

        public static string Dump(object element, DumpOptions dumpOptions = default(DumpOptions))
        {
            var instance = new ObjectDumper(dumpOptions);
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                this.Write(this.FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.Write("{{{0}}}", objectType.FullName);
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
                            this.Write("{0}: {1}", fieldInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo()
                                .IsAssignableFrom(fieldInfo.FieldType.GetTypeInfo());
                            this.Write("{0}: {1}", fieldInfo.Name, isEnumerable ? "..." : "{ }");

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
                            this.Write("{0}: {1}", propertyInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
                            this.Write("{0}: {1}", propertyInfo.Name, isEnumerable ? "..." : "{ }");

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

            if (o is IEnumerable)
            {
                return "...";
            }

            return "{ }";
        }
    }
}