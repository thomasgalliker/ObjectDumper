using System.Reflection;

namespace ObjectDumping.Internal
{
    internal class PropertyAndValue
    {
        public PropertyAndValue(object source, PropertyInfo propertyInfo)
        {
            this.Value = propertyInfo.TryGetValue(source);
            this.DefaultValue = propertyInfo.PropertyType.GetDefault();
            this.Property = propertyInfo;
        }

        public PropertyInfo Property { get; }

        public object Value { get; }

        public object DefaultValue { get; }

        public bool IsDefaultValue
        {
            get
            {
                return Equals(this.Value, this.DefaultValue);
            }
        }
    }
}
