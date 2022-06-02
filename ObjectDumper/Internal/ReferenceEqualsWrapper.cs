using System;
using System.Runtime.CompilerServices;

namespace ObjectDumping.Internal
{
    internal struct ReferenceEqualsWrapper : IEquatable<ReferenceEqualsWrapper>
    {
        private readonly object value;

        public ReferenceEqualsWrapper(object value)
        {
            this.value = value;
        }

        public override bool Equals(object value)
        {
            return value is ReferenceEqualsWrapper wrapper && this.Equals(wrapper);
        }

        public bool Equals(ReferenceEqualsWrapper wrapper)
        {
            return ReferenceEquals(this.value, wrapper.value);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this.value);
        }
    }
}
