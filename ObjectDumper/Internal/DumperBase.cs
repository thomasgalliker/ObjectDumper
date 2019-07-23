using System.Collections.Generic;
using System.Text;

namespace ObjectDumping.Internal
{
    public abstract class DumperBase
    {
        private readonly List<int> hashListOfFoundElements;
        private readonly StringBuilder stringBuilder;
        private bool isNewLine;

        protected DumperBase(DumpOptions dumpOptions)
        {
            this.DumpOptions = dumpOptions;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.hashListOfFoundElements = new List<int>();
            this.isNewLine = true;
        }

        public int Level { get; set; }

        public bool IsMaxLevel()
        {
            return this.Level > this.DumpOptions.MaxLevel;
        }

        protected DumpOptions DumpOptions { get; }

        /// <summary>
        /// Calls <see cref="Write(string, int)"/> using the current Level as indentLevel if the current
        /// position is at a the beginning of a new line or 0 otherwise. 
        /// </summary>
        /// <param name="value">string to be written</param>
        protected void Write(string value)
        {
            if (this.isNewLine)
            {
                Write(value, Level);
            }
            else
            {
                Write(value, 0);
            }
        }

        /// <summary>
        /// Writes value to underlying <see cref="StringBuilder"/> using <paramref name="indentLevel"/> and <see cref="DumpOptions.IndentChar"/> and <see cref="DumpOptions.IndentSize"/>
        /// to determin the indention chars prepended to <paramref name="value"/>
        /// </summary>
        /// <remarks>
        /// This function needs to keep up with if the last value written included the LineBreakChar at the end
        /// </remarks>
        /// <param name="value">string to be written</param>
        /// <param name="indentLevel">number of indentions to prepend default 0</param>
        protected void Write(string value, int indentLevel = 0)
        {
            this.stringBuilder.Append(this.DumpOptions.IndentChar, indentLevel * this.DumpOptions.IndentSize);
            this.stringBuilder.Append(value);
            if (value.EndsWith(this.DumpOptions.LineBreakChar))
            {
                this.isNewLine = true;
            }
            else
            {
                this.isNewLine = false;
            }
        }

        /// <summary>
        /// Writes a line break to underlying <see cref="StringBuilder"/> using <see cref="DumpOptions.LineBreakChar"/>
        /// </summary>
        /// <remarks>
        /// By definition this sets isNewLine to true
        /// </remarks>
        protected void LineBreak()
        {
            this.stringBuilder.Append(this.DumpOptions.LineBreakChar);
            isNewLine = true;
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

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }
    }
}
