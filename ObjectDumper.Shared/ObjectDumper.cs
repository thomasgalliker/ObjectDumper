namespace System.Diagnostics
{
    public static class ObjectDumper
    {
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