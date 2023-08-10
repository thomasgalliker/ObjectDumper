using System.IO;
using ObjectDumping.Internal;

// ReSharper disable once CheckNamespace
public static class ObjectDumper
{
    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the default dump options.</param>
    /// <returns></returns>
    public static string Dump(object element)
    {
        var dumpOptions = new DumpOptions { DumpStyle = DumpStyle.Console };
        return Dump(element, dumpOptions);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpStyle" />.</param>
    /// <param name="dumpStyle">The formatting style.</param>
    /// <returns></returns>
    public static string Dump(object element, DumpStyle dumpStyle)
    {
        var dumpOptions = new DumpOptions { DumpStyle = dumpStyle };
        return Dump(element, dumpOptions);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string with additional options <see cref="dumpOptions" />.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpOptions" />.</param>
    /// <param name="dumpOptions">Further options to customize the dump output.</param>
    /// <returns></returns>
    public static string Dump(object element, DumpOptions dumpOptions)
    {
        if (dumpOptions == null)
        {
            dumpOptions = new DumpOptions();
        }

        if (dumpOptions.DumpStyle == DumpStyle.Console)
        {
            return ObjectDumperConsole.Dump(element, dumpOptions);
        }

        return ObjectDumperCSharp.Dump(element, dumpOptions);
    }

    /// <summary>
    ///     Serializes the given <see cref="element" /> to string with additional options <see cref="dumpOptions" />.
    /// </summary>
    /// <param name="element">Object to be dumped to string using the given <paramref name="dumpOptions" />.</param>
    /// <param name="writer">Where <paramref name="element"/> is write.</param>
    /// <param name="dumpOptions">Further options to customize the dump output.</param>
    public static void Dump(object element, TextWriter writer, DumpOptions dumpOptions)
    {
        if (dumpOptions == null)
        {
            dumpOptions = new DumpOptions();
        }

        switch (dumpOptions.DumpStyle)
        {
            case DumpStyle.Console:
                ObjectDumperCSharp.Dump(element, writer, dumpOptions);
                break;
            case DumpStyle.CSharp:
                ObjectDumperConsole.Dump(element, writer, dumpOptions);
                break;
            default:
                break;
        }
    }

}
