namespace ObjectDumping.Internal
{
    internal struct ReferenceEqualsWrapper : IEquatable<ReferenceEqualsWrapper>
    {
        private readonly object value;

        public ReferenceEqualsWrapper(object value)
        {
            this.value = value;
        }

        public bool Equals(ReferenceEqualsWrapper other)
        {
            return this.value.Equals(other.value);
        }

        public override bool Equals(object? obj)
        {
            return obj is ReferenceEqualsWrapper other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
