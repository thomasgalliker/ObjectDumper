using System.Collections.Generic;
using System.Diagnostics;

namespace ObjectDumping.Internal
{
    internal sealed class CircularReferenceDetector
    {
        // The stack of references on the branch of the current object graph, used to detect reference cycles.
        private readonly Stack<ReferenceEqualsWrapper> stack = new Stack<ReferenceEqualsWrapper>();

        internal void PopReferenceForCycleDetection()
        {
            this.stack.Pop();
        }

        internal bool ContainsReferenceForCycleDetection(object value)
        {
            var wrapper = new ReferenceEqualsWrapper(value);
            return this.stack.Contains(wrapper);
        }

        internal void EnsureEmpty()
        {
            Debug.Assert(
                condition: this.stack.Count == 0,
                message: "Something went wrong if the circular reference detector stack is not empty at this time");
        }

        internal void PushReferenceForCycleDetection(object value)
        {
            var wrapper = new ReferenceEqualsWrapper(value);
            Debug.Assert(!this.stack.Contains(wrapper));
            this.stack.Push(wrapper);
        }
    }
}
