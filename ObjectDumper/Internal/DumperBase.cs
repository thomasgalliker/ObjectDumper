using System;
using System.Text;

namespace ObjectDumping.Internal
{
    public abstract class DumperBase
    {
        private readonly CircularReferenceDetector circularReferenceDetector;
        private readonly StringBuilder stringBuilder;
        private bool isNewLine;
        private int level;

        protected DumperBase(DumpOptions dumpOptions)
        {
            this.DumpOptions = dumpOptions;
            this.Level = 0;
            this.stringBuilder = new StringBuilder();
            this.circularReferenceDetector = new CircularReferenceDetector();
            this.isNewLine = true;
        }

        protected abstract void FormatValue(object o, int intentLevel);

        public int Level
        {
            get => this.level;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Level must not be a negative number", nameof(this.Level));
                }

                this.level = value;
            }
        }

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
                this.Write(value, this.Level);
            }
            else
            {
                this.Write(value, 0);
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
            this.isNewLine = true;
        }

        protected void PushReferenceForCycleDetection(object value)
        {
            var type = value.GetType();
            if (type.IsAnonymous())
            {
                // Because the Equals and GetHashCode methods on anonymous types
                // are defined in terms of the Equals and GetHashCode methods
                // of the properties, two instances of the same anonymous type are equal
                // only if all their properties are equal.
                //
                // Source: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
                return;
            }

            if (type.IsPrimitive() || type == typeof(string))
            {
                return;
            }

            this.circularReferenceDetector.PushReferenceForCycleDetection(value);
        }

        protected void PopReferenceForCycleDetection(object value)
        {
            var type = value.GetType();
            if (type.IsAnonymous())
            {
                return;
            }

            if (type.IsPrimitive() || type == typeof(string))
            {
                return;
            }

            this.circularReferenceDetector.PopReferenceForCycleDetection();
        }

        /// <summary>
        /// Checks if the given <paramref name="value"/> is part of an infinite recursion.
        /// </summary>
        protected bool CheckForCircularReference(object value)
        {
            if (value == null)
            {
                return false;
            }

            //var type = value.GetType();
            //if (type.IsPrimitive() || type == typeof(string))
            //{
            //    return false;
            //}

            var contains = this.circularReferenceDetector.ContainsReferenceForCycleDetection(value);
            if (contains)
            {
                return true;
            }

            return false;
        }

        protected string ResolvePropertyName(string name)
        {
            if (this.DumpOptions.MemberRenamer is Func<string, string> memberRenamer)
            {
                return memberRenamer.Invoke(name);
            }

            return name;
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            this.circularReferenceDetector.EnsureEmpty();

            return this.stringBuilder.ToString();
        }
    }
}
