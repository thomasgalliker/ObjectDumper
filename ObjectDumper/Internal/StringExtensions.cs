namespace ObjectDumping.Internal
{
    internal static class StringExtensions
    {
        internal static string ToLowerFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            var a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}
