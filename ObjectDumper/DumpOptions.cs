using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
public class DumpOptions
{
    public DumpOptions()
    {
        this.DumpStyle = DumpStyle.Console;
        this.IndentSize = 2;
        this.IndentChar = ' ';
        this.LineBreakChar = "\n\r";
        this.SetPropertiesOnly = false;
        this.MaxLevel = int.MaxValue;
        this.ExcludeProperties = new HashSet<string>();
        this.PropertyOrderBy = null;
    }

    public DumpStyle DumpStyle { get; set; }

    public int IndentSize { get; set; }

    public char IndentChar { get; set; }

    public string LineBreakChar { get; set; }

    public bool SetPropertiesOnly { get; set; }

    public int MaxLevel { get; set; }

    public ICollection<string> ExcludeProperties { get; set; }

    public Expression<Func<PropertyInfo, object>> PropertyOrderBy { get; set; }
}
