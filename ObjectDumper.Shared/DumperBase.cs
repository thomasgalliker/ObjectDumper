using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics
{
    public abstract class DumperBase
    {
        private readonly List<int> hashListOfFoundElements;
        private readonly StringBuilder stringBuilder;

        protected DumperBase(int indentSize)
        {
            this.IndentSize = indentSize;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
        }

        public int IndentSize { get; }

        public int Level { get; set; }

        protected void StartLine(string value)
        {
            var space = new string(' ', this.Level * this.IndentSize);

            this.stringBuilder.Append(space + value);
        }

        protected void LineBreak()
        {
            this.stringBuilder.Append("\n\r");
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