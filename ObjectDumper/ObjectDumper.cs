namespace System.Diagnostics
{
    public static class ObjectDumper
    {
        /// <summary>
        /// Serializes the given <see cref="element"/> to string with additional options <see cref="dumpOptions"/>.
        /// </summary>
        /// <param name="element">Object to be dumped to string using the given <paramref name="dumpStyle"/>.</param>
        /// <param name="dumpStyle">The formatting style.</param>
        /// <param name="dumpOptions">Further options to customize the dump output.</param>
        /// <returns></returns>
        public static string Dump(object element, DumpStyle dumpStyle = DumpStyle.Console, DumpOptions dumpOptions = null)
        {
            if (dumpOptions == null)
            {
                dumpOptions = new DumpOptions();
            }

            if (dumpStyle == DumpStyle.Console)
            {
                return ObjectDumperConsole.Dump(element, dumpOptions);
            }

            return ObjectDumperCSharp.Dump(element, dumpOptions);
        }
    }
}
