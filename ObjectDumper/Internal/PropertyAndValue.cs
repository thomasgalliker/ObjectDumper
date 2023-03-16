using System.Reflection;

namespace ObjectDumping.Internal
{
    internal class PropertyAndValue
    {
        public PropertyAndValue(object source, PropertyInfo propertyInfo)
        {
            this.Value = propertyInfo.TryGetValue(source);
            this.DefaultValue = propertyInfo.PropertyType.TryGetDefault();

#if NET5_0_OR_GREATER
            this.IsInitOnly = propertyInfo.IsInitOnly();
#endif
            this.Property = propertyInfo;
        }

        public PropertyInfo Property { get; }

        public object Value { get; }

        public object DefaultValue { get; }

#if NET5_0_OR_GREATER
        public bool IsInitOnly { get; }
#endif

        public bool IsDefaultValue
        {
            get
            {
                return Equals(this.Value, this.DefaultValue);
            }
        }

        public override string ToString()
        {
            return $"PropertyAndValue: Property={this.Property}, Value={this.Value}";
        }
    }
}
