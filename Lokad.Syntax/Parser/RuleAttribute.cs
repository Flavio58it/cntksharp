using System;

namespace Lokad.Syntax.Parser
{
    public class RuleAttribute : Attribute
    {
        /// <summary> The priority level of this rule. </summary>
        /// <remarks> Used to filter based on <see cref="NonTerminalAttribute.MaxRank"/>. </remarks>
        public int Rank { get; set; }
    }
}
