using System;
using System.Collections.Generic;

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

        public bool IsEmpty => this.stack.Count == 0;

        internal void PushReferenceForCycleDetection(object value)
        {
            var wrapper = new ReferenceEqualsWrapper(value);

            if (this.stack.Contains(wrapper))
            {
                throw new InvalidOperationException(
                    $"CircularReferenceDetector failed to push object of type '{value.GetType().GetFormattedName()}': " +
                    $"The same object is already exists.");
            }

            this.stack.Push(wrapper);
        }
    }
}
