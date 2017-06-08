using System;
using System.Text.RegularExpressions;

namespace Lokad.Syntax.Lexer
{
    /// <summary> 
    /// Applied to enumeration members to express what pattern to
    /// use to extract them during tokenization.
    /// </summary>
    public class PatternAttribute : Attribute
    {
        /// <summary> The pattern. </summary>
        /// <remarks> 
        /// Will be used as a regular expression. 
        /// A '^' will be prepended, if none is found. 
        /// </remarks>
        public string Pattern { get; }

        /// <summary> The characters with which this pattern can start. </summary>
        public string Start { get; set; }

        /// <summary> Is this pattern case-sensitive ? </summary>
        /// <remarks> Default is true. </remarks>
        public bool CaseSensitive { get; set; } = true;

        public PatternAttribute(string pattern)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            Pattern = pattern;
        }

        /// <summary> Converts attribute to <see cref="TokenDefinition"/>. </summary>
        public TokenDefinition ToDefinition()
        {
            var flags = RegexOptions.Compiled | RegexOptions.CultureInvariant;
            if (!CaseSensitive) flags = flags | RegexOptions.IgnoreCase;

            var csPattern = Pattern.StartsWith("\\G") ? Pattern : $"\\G({Pattern})";
            var jsPattern = new JsRegex(Pattern.Replace("\\G", ""), CaseSensitive ? "" : "i");

            return new TokenDefinition(new Regex(csPattern, flags), jsPattern, startsWith: Start);
        }
    }

    /// <summary> As <see cref="PatternAttribute"/> but case-insensitive by default. </summary>
    public sealed class PatternCiAttribute : PatternAttribute
    {
        public PatternCiAttribute(string pattern) : base(pattern)
        {
            CaseSensitive = false;
        }
    }
}
