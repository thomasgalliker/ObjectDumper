using System.Collections;
using System.Collections.Generic;

namespace ObjectDumping.Internal
{
    internal static class LastEnumerator
    {
        internal static IEnumerable<MetaEnumerableItem<T>> GetLastEnumerable<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                var isFirst = true;
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var current = enumerator.Current;
                        isLast = !enumerator.MoveNext();
                        yield return new MetaEnumerableItem<T>
                        {
                            Value = current,
                            IsLast = isLast,
                            IsFirst = isFirst
                        };
                        isFirst = false;
                    } while (!isLast);
                }
            }
        }

        internal static IEnumerable<MetaEnumerableItem<object>> GetLastEnumerable(this IEnumerable source)
        {
            var enumerator = source.GetEnumerator();
            {
                var isFirst = true;
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var current = enumerator.Current;
                        isLast = !enumerator.MoveNext();
                        yield return new MetaEnumerableItem<object>
                        {
                            Value = current,
                            IsLast = isLast,
                            IsFirst = isFirst
                        };
                        isFirst = false;
                    } while (!isLast);
                }
            }
        }
    }

    public class MetaEnumerableItem<T>
    {
        public T Value { get; set; }

        public bool IsLast { get; set; }

        public bool IsFirst { get; set; }
    }
}
