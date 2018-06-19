using Enumerable = System.Linq.Enumerable;
using IntrospectionExtensions = System.Reflection.IntrospectionExtensions;
using RuntimeReflectionExtensions = System.Reflection.RuntimeReflectionExtensions;

namespace System.Diagnostics
{
    /// <summary>
    ///  Source: http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    /// </summary>
    public sealed class ObjectDumper
    {
        #region Public Constructors

        public ObjectDumper(Int32 indentSize = 2)
        {
            this._indentSize = indentSize;
            this._stringBuilder = new Text.StringBuilder();
            this._hashListOfFoundElements = new Collections.Generic.List<Int32>();
        }

        #endregion Public Constructors

        #region Private Fields

        private readonly Collections.Generic.List<Int32> _hashListOfFoundElements;
        private readonly Int32 _indentSize;
        private readonly Text.StringBuilder _stringBuilder;
        private Int32 _level;

        #endregion Private Fields

        #region Public Methods

        public static String Dump(Object element) => ObjectDumper.Dump(element, 2);

        public static String Dump(Object element, Int32 indentSize)
        {
            var instance = new ObjectDumper(indentSize);
            return instance.DumpElement(element);
        }

        #endregion Public Methods

        #region Private Methods

        private Boolean AlreadyTouched(Object value)
        {
            if (value == null)
            {
                return false;
            }

            var hash = value.GetHashCode();
            foreach (var t in this._hashListOfFoundElements)
            {
                if (t == hash)
                {
                    return true;
                }
            }

            return false;
        }

        private String DumpElement(Object element)
        {
            if (element == null || element is ValueType || element is String)
            {
                this.Write(this.FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!IntrospectionExtensions.GetTypeInfo(typeof(Collections.IEnumerable))
                    .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(objectType)))
                {
                    this.Write("{{{0}}}", objectType.FullName);
                    this._hashListOfFoundElements.Add(element.GetHashCode());
                    this._level++;
                }

                if (element is Collections.IEnumerable enumerableElement)
                {
                    foreach (var item in enumerableElement)
                    {
                        if (item is Collections.IEnumerable && !(item is String))
                        {
                            this._level++;
                            this.DumpElement(item);
                            this._level--;
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
                    Collections.Generic.IEnumerable<Reflection.FieldInfo> publicFields =
                        Enumerable.Where(RuntimeReflectionExtensions.GetRuntimeFields(element.GetType()),
                            f => !f.IsPrivate);
                    foreach (var fieldInfo in publicFields)
                    {
                        Object value;
                        try
                        {
                            value = fieldInfo.GetValue(element);
                        }
                        catch (Exception ex)
                        {
                            value = $"{{{ex.Message}}}";
                        }

                        if (IntrospectionExtensions.GetTypeInfo(fieldInfo.FieldType).IsValueType ||
                            fieldInfo.FieldType == typeof(String))
                        {
                            this.Write("{0}: {1}", fieldInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = IntrospectionExtensions.GetTypeInfo(typeof(Collections.IEnumerable))
                                .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(fieldInfo.FieldType));
                            this.Write("{0}: {1}", fieldInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this._level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }

                            this._level--;
                        }
                    }

                    Collections.Generic.IEnumerable<Reflection.PropertyInfo> publicProperties =
                        Enumerable.Where(RuntimeReflectionExtensions.GetRuntimeProperties(element.GetType()),
                            p => p.GetMethod != null && p.GetMethod.IsStatic == false);
                    foreach (var propertyInfo in publicProperties)
                    {
                        var type = propertyInfo.PropertyType;
                        Object value;
                        try
                        {
                            value = propertyInfo.GetValue(element, null);
                        }
                        catch (Exception ex)
                        {
                            value = $"{{{ex.Message}}}";
                        }

                        if (IntrospectionExtensions.GetTypeInfo(type).IsValueType || type == typeof(String))
                        {
                            this.Write("{0}: {1}", propertyInfo.Name, this.FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = IntrospectionExtensions.GetTypeInfo(typeof(Collections.IEnumerable))
                                .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(type));
                            this.Write("{0}: {1}", propertyInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && this.AlreadyTouched(value);
                            this._level++;
                            if (!alreadyTouched)
                            {
                                this.DumpElement(value);
                            }
                            else
                            {
                                this.Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }

                            this._level--;
                        }
                    }
                }

                if (!IntrospectionExtensions.GetTypeInfo(typeof(Collections.IEnumerable))
                    .IsAssignableFrom(IntrospectionExtensions.GetTypeInfo(objectType)))
                {
                    this._level--;
                }
            }

            return this._stringBuilder.ToString();
        }

        private String FormatValue(Object o)
        {
            switch (o)
            {
                case null:
                    return "null";

                case DateTime time:
                    return time.ToString("MM.dd.yyyy HH:mm:ss");

                case String _:
                    return $"\"{o}\"";

                case Char _ when (Char)o == '\0':
                    return String.Empty;

                case ValueType _:
                    return o.ToString();

                case Collections.IEnumerable _:
                    return "...";
            }

            return "{ }";
        }

        private void Write(String value, params Object[] args)
        {
            var space = new String(' ', this._level * this._indentSize);

            if (args != null && args.Length > 0)
            {
                value = String.Format(value, args);
            }

            this._stringBuilder.AppendLine(space + value);
        }

        #endregion Private Methods
    }
}
