using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectDumping.Internal
{
    public abstract class DumperBase
    {
        private readonly CircularReferenceDetector circularReferenceDetector;
        private readonly TextWriter writer;
        private bool isNewLine;
        private int level;

        protected DumperBase(DumpOptions dumpOptions)
            : this(new StringWriter(new StringBuilder()), dumpOptions)
        {
        }

        protected DumperBase(TextWriter writer, DumpOptions dumpOptions)
        {
            this.DumpOptions = dumpOptions;
            this.Level = 0;
            this.circularReferenceDetector = new CircularReferenceDetector();
            this.isNewLine = true;
            this.writer = writer;
        }

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
            for (var i = 0; i < indentLevel * this.DumpOptions.IndentSize; i++)
            {
                this.writer.Write(this.DumpOptions.IndentChar);
            }

            this.writer.Write(value);

            if (value.EndsWith(this.DumpOptions.LineBreakChar))
            {
                this.isNewLine = true;
            }
            else
            {
                this.isNewLine = false;
            }
        }

        protected async Task WriteAsync(string value, int indentLevel = 0, CancellationToken token = default)
        {
            for (var i = 0; i < indentLevel * this.DumpOptions.IndentSize; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                this.writer.Write(this.DumpOptions.IndentChar);
            }

            await this.writer.WriteAsync(value);

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
            this.writer.Write(this.DumpOptions.LineBreakChar);
            this.isNewLine = true;
        }

        /// <summary>
        /// Writes a line break to underlying <see cref="StringBuilder"/> using <see cref="DumpOptions.LineBreakChar"/>
        /// </summary>
        /// <remarks>
        /// By definition this sets isNewLine to true
        /// </remarks>
        protected async Task LineBreakAsync()
        {
            await this.writer.WriteAsync(this.DumpOptions.LineBreakChar);
            this.isNewLine = true;
        }

        protected void PushReferenceForCycleDetection(object value)
        {
            if (value == null)
            {
                return;
            }

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
            if (value == null)
            {
                return;
            }

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

        protected async Task FlushAsync()
        {
            await this.writer.FlushAsync();
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="string"/>
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            if (!this.circularReferenceDetector.IsEmpty)
            {
                throw new InvalidOperationException(
                    "CircularReferenceDetector: Something went wrong if the circular reference detector stack is not empty at this time");
            }
            this.writer.Flush();
            return this.writer.ToString();
        }
    }
}
