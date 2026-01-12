using System;

namespace ObjectDumping.Tests.Testdata
{
    public class NestedItemA : NestedItem<NestedItemB>
    {
    }

    public class NestedItemB : NestedItem<NestedItemA>
    {
    }

    public class NestedItem<TNext> : NestedItem, IEquatable<NestedItem<TNext>?> where TNext : NestedItem
    {
        public TNext? Next { get; set; }

        public bool Equals(NestedItem<TNext>? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Property == other.Property;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is NestedItem<TNext> nestedItem))
            {
                return false;
            }

            return this.Equals(nestedItem);
        }

        public override int GetHashCode()
        {
            return this.Property.GetHashCode();
        }
    }

    public abstract class NestedItem
    {
        public int Property { get; set; }
    }

}
