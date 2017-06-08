using System;

namespace Lokad.Syntax.Parser
{
    /// <summary>
    /// On a parameter of a function marked with <see cref="RuleAttribute"/>,
    /// indicates that the value should be extracted from another function
    /// marked with <see cref="RuleAttribute"/>.
    /// </summary>
    public class NonTerminalAttribute : Attribute
    {
        /// <summary>
        /// The rule used to generate this non-terminal should have at most
        /// this priority. By default, there is no limit.
        /// </summary>
        public int? MaxRank { get; set; }

        public bool Optional { get; set; }

        protected NonTerminalAttribute(int maxRank = -1, bool optional = false)
        {
            MaxRank = maxRank < 0 ? (int?)null : maxRank;

            Optional = optional;
        }
    }
}
