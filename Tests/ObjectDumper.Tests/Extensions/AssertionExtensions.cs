using System;
using FluentAssertions;

namespace ObjectDumping.Tests
{
    public static class AssertionExtensions
    {
        /// <summary>
        /// Compares the given <paramref name="actualValue"/> with the <paramref name="expected"/> value
        /// replacing the new line characters of the expected value.
        /// </summary>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="lineBreakChar">The line break characters to be used to compare <paramref name="actualValue"/> to <paramref name="expected"/>.</param>
        public static void ShouldBeEquivalent(this string actualValue, string expected, string? lineBreakChar = null)
        {
            if (lineBreakChar == null)
            {
                lineBreakChar = Environment.NewLine;
            }

            actualValue.Should().Be(expected.Replace("\r\n", lineBreakChar));
        }
    }
}