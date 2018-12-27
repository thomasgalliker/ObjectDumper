using System.Collections.Generic;
using System.Text;

namespace ObjectDumping.Internal
{
    public abstract class DumperBase
    {
        private readonly List<int> hashListOfFoundElements;
        private readonly StringBuilder stringBuilder;

        protected DumperBase(DumpOptions dumpOptions)
        {
            this.DumpOptions = dumpOptions;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
        }

        public int Level { get; set; }

        public bool IsMaxLevel()
        {
            return this.Level > this.DumpOptions.MaxLevel;
        }

        protected DumpOptions DumpOptions { get; }

        private static string CalculateSpace(char c, int level, int size)
        {
            var space = new string(c, level * size);
            return space;
        }

        protected void StartLine(string value)
        {
            var space = CalculateSpace(this.DumpOptions.IndentChar, this.Level, this.DumpOptions.IndentSize);
            this.stringBuilder.Append(space + value);
        }

        protected void Write(string value, int? intentLevel = null)
        {
            var space = CalculateSpace(this.DumpOptions.IndentChar, intentLevel ?? 0, this.DumpOptions.IndentSize);
            this.stringBuilder.Append(space + value);
        }

        protected void LineBreak()
        {
            this.stringBuilder.Append(this.DumpOptions.LineBreakChar);
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
