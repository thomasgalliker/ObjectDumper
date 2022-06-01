using System;
using System.Runtime.CompilerServices;

namespace ObjectDumping.Internal
{
    internal struct ReferenceEqualsWrapper : IEquatable<ReferenceEqualsWrapper>
    {
        private readonly object @object;

        public ReferenceEqualsWrapper(object obj)
        {
            this.@object = obj;
        }

        public override bool Equals(object obj)
        {
            return obj is ReferenceEqualsWrapper otherObj && this.Equals(otherObj);
        }

        public bool Equals(ReferenceEqualsWrapper obj)
        {
            return ReferenceEquals(this.@object, obj.@object);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this.@object);
        }
    }
}
