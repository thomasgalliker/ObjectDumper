using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Diagnostics
{
    /// <summary>
    /// Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    public class ObjectDumper
    {
        private int level;
        private readonly int indentSize;
        private readonly StringBuilder stringBuilder;
        private readonly List<int> hashListOfFoundElements;

        public ObjectDumper(int indentSize = 2)
        {
            this.indentSize = indentSize;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
        }

        public static string Dump(object element)
        {
            return Dump(element, 2);
        }

        public static string Dump(object element, int indentSize)
        {
            var instance = new ObjectDumper(indentSize);
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
                    this.hashListOfFoundElements.Add(element.GetHashCode());
                    this.level++;
                }

                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (var item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            this.level++;
                            this.DumpElement(item);
                            this.level--;
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
                        object value = fieldInfo.GetValue(element);

                        if (fieldInfo.FieldType.GetTypeInfo().IsValueType || fieldInfo.FieldType == typeof(string))
                        {
                            this.Write("{0}: {1}", fieldInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(fieldInfo.FieldType.GetTypeInfo());
                            this.Write("{0}: {1}", fieldInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this.level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }
                            this.level--;
                        }
                    }

                    var publicProperties = element.GetType().GetRuntimeProperties()
                        .Where(p => p.GetMethod != null && p.GetMethod.IsStatic == false);
                    foreach (var propertyInfo in publicProperties)
                    {
                        var type = propertyInfo.PropertyType;
                        object value = propertyInfo.GetValue(element, null);

                        if (type.GetTypeInfo().IsValueType || type == typeof(string))
                        {
                            this.Write("{0}: {1}", propertyInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
                            this.Write("{0}: {1}", propertyInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this.level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }
                            this.level--;
                        }
                    }
                }

                if (!typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                {
                    this.level--;
                }
            }

            return this.stringBuilder.ToString();
        }

        private bool AlreadyTouched(object value)
        {
            if (value == null)
            {
                return false;
            }

            var hash = value.GetHashCode();
            for (var i = 0; i < this.hashListOfFoundElements.Count; i++)
            {
                if (this.hashListOfFoundElements[i] == hash)
                {
                    return true;
                }
            }
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', this.level * this.indentSize);

            if (args != null && args.Length > 0)
            {
                value = string.Format(value, args);
            }

            this.stringBuilder.AppendLine(space + value);
        }

        private string FormatValue(object o)
        {
            if (o == null)
            {
                return "null";
            }

            //if (o is DateTime)
            //{
            //    return ((DateTime)o).ToShortDateString();
            //}

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