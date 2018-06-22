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

        public bool SetPropertiesOnly
        {
            get { return this.dumpOptions.SetPropertiesOnly; }
        }

        private static string CalculateSpace(char c, int level, int size)
        {
            var space = new string(c, level * size);
            return space;
        }

        protected void StartLine(string value)
        {
            var space = CalculateSpace(this.dumpOptions.IndentChar, this.Level, this.dumpOptions.IndentSize);
            this.stringBuilder.Append(space + value);
        }

        protected void Write(string value, int? intentLevel = null)
        {
            var space = CalculateSpace(this.dumpOptions.IndentChar, intentLevel ?? 0, this.dumpOptions.IndentSize);
            this.stringBuilder.Append(space + value);
        }

        protected void LineBreak()
        {
            this.stringBuilder.Append(this.dumpOptions.LineBreakChar);
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