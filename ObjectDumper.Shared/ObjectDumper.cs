namespace System.Diagnostics
{
    public static class ObjectDumper
    {
        /// <summary>
        /// Serializes the given <see cref="element"/> to string of style <see cref="dumpStyle"/>.
        /// </summary>
        public static string Dump(object element, DumpStyle dumpStyle)
        {
            return Dump(element, new DumpOptions { DumpStyle = dumpStyle });
        }

        /// <summary>
        /// Serializes the given <see cref="element"/> to string with additional options <see cref="dumpOptions"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dumpOptions"></param>
        /// <returns></returns>
        public static string Dump(object element, DumpOptions dumpOptions = default(DumpOptions))
        {
            if (dumpOptions.DumpStyle == DumpStyle.Console)
            {
                return ObjectDumperConsole.Dump(element);
            }

            return ObjectDumperCSharp.Dump(element);
        }
    }
}