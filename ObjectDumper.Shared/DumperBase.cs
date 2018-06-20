using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics
{
    public abstract class DumperBase
    {
        private readonly DumpOptions dumpOptions;
        private readonly List<int> hashListOfFoundElements;
        private readonly StringBuilder stringBuilder;

        protected DumperBase(DumpOptions dumpOptions)
        {
            this.dumpOptions = dumpOptions;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
        }

        public int Level { get; set; }

        protected void StartLine(string value)
        {
            var space = new string(this.dumpOptions.IndentChar, this.Level * this.dumpOptions.IndentSize);

            this.stringBuilder.Append(space + value);
        }

        protected void LineBreak()
        {
            this.stringBuilder.Append(this.dumpOptions.LineBreakChar);
        }

        protected void Write(string value, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                value = string.Format(value, args);
            }

            this.stringBuilder.Append(value);
        }

        protected void AddAlreadyTouched(object element)
        {
            this.hashListOfFoundElements.Add(element.GetHashCode());
        }

        protected bool AlreadyTouched(object value)
        {
            if (value == null)
            {
                return false;
            }

            var hash = value.GetHashCode();
            for (var i = 0; i < this.hashListOfFoundElements.Count; i++)
            {
                if (this.hashListOfFoundElements[i] == hash)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }
    }
}