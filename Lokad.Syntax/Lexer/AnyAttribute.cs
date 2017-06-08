using System;
using System.Collections.Generic;

namespace Lokad.Syntax.Lexer
{
    /// <summary> 
    /// Applied to enumeration members to express what pattern to
    /// use to extract them during tokenization, as a list of 
    /// acceptable values.
    /// </summary>
    public class AnyAttribute : Attribute
    {
        /// <summary> All possible values for this token. </summary>
        public IReadOnlyList<string> Options { get; }

        /// <summary> Is this token case sensitive ? </summary>
        public bool CaseSensitive { get; set; } = true;

        public AnyAttribute(params string[] options)
        {
            if (options.Length == 0)
                throw new ArgumentException(@"Expected at least one value.", nameof(options));

            Options = options;
        }

        /// <summary> Convert a token to a definition. </summary>
        public TokenDefinition ToDefinition()
        {
            return new TokenDefinition(Options, CaseSensitive);
        }
    }

    /// <summary> Like <see cref="AnyAttribute"/> but case-insensitive by default. </summary>
    public sealed class AnyCiAttribute : AnyAttribute
    {
        public AnyCiAttribute(params string[] options) : base(options)
        {
            CaseSensitive = false;
        }
    }
}
