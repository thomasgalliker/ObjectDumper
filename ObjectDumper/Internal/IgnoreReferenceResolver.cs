using System.Collections.Generic;
using System.Diagnostics;

namespace ObjectDumping.Internal
{
    internal sealed class IgnoreReferenceResolver
    {
        // The stack of references on the branch of the current object graph, used to detect reference cycles.
        private Stack<ReferenceEqualsWrapper> stackForCycleDetection;

        internal void PopReferenceForCycleDetection()
        {
            Debug.Assert(this.stackForCycleDetection != null);
            this.stackForCycleDetection.Pop();
        }

        internal bool ContainsReferenceForCycleDetection(object value)
        {
            return this.stackForCycleDetection?.Contains(new ReferenceEqualsWrapper(value)) ?? false;
        }

        internal void EnsureEmpty()
        {
            Debug.Assert(this.stackForCycleDetection == null || this.stackForCycleDetection.Count == 0);
        }

        internal void PushReferenceForCycleDetection(object value)
        {
            var wrappedValue = new ReferenceEqualsWrapper(value);

            if (this.stackForCycleDetection is null)
            {
                this.stackForCycleDetection = new Stack<ReferenceEqualsWrapper>();
            }

            Debug.Assert(!this.stackForCycleDetection.Contains(wrappedValue));
            this.stackForCycleDetection.Push(wrappedValue);
        }

    }
}
