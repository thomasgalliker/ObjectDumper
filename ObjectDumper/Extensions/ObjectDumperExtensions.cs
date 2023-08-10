using System.IO;

public static class ObjectDumperExtensions
{
    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the default dump options.</param>
    /// <returns></returns>
    public static string Dump(this object element)
    {
        return ObjectDumper.Dump(element);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpStyle" />.</param>
    /// <param name="dumpStyle">The formatting style.</param>
    /// <returns></returns>
    public static string Dump(this object element, DumpStyle dumpStyle)
    {
        return ObjectDumper.Dump(element, dumpStyle);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string with additional options <see cref="dumpOptions" />.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpOptions" />.</param>
    /// <param name="dumpOptions">Further options to customize the dump output.</param>
    /// <returns></returns>
    public static string Dump(this object element, DumpOptions dumpOptions)
    {
        return ObjectDumper.Dump(element, dumpOptions);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the default dump options.</param>
    /// <param name="writer">Where <paramref name="element"/> will dump.</param>
    /// <returns></returns>
    public static void Dump(this object element, TextWriter writer)
    {
        ObjectDumper.Dump(element, writer, default);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpStyle" />.</param>
    /// <param name="writer">Where <paramref name="element"/> will dump.</param>
    /// <param name="dumpStyle">The formatting style.</param>
    /// <returns></returns>
    public static void Dump(this object element, TextWriter writer, DumpStyle dumpStyle)
    {
        ObjectDumper.Dump(element, writer, new() { DumpStyle = dumpStyle });
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string with additional options <see cref="dumpOptions" />.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpOptions" />.</param>
    /// <param name="writer">Where <paramref name="element"/> will dump.</param>
    /// <param name="dumpOptions">Further options to customize the dump output.</param>
    /// <returns></returns>
    public static void Dump(this object element, TextWriter writer, DumpOptions dumpOptions)
    {
        ObjectDumper.Dump(element, writer, dumpOptions);
    }
}
