using System.Collections;

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
                        var value = enumerator.Current;
                        isLast = !enumerator.MoveNext();
                        yield return new MetaEnumerableItem<T>(value, isFirst, isLast);
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
                        var value = enumerator.Current!;
                        isLast = !enumerator.MoveNext();
                        yield return new MetaEnumerableItem<object>(value, isFirst, isLast);
                        isFirst = false;
                    } while (!isLast);
                }
            }
        }
    }

    internal class MetaEnumerableItem<T>
    {
        public MetaEnumerableItem(T value, bool isFirst, bool isLast)
        {
            this.Value = value;
            this.IsFirst = isFirst;
            this.IsLast = isLast;
        }

        internal T Value { get; }

        internal bool IsFirst { get; }

        internal bool IsLast { get; }
    }
}
