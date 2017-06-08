using System.Collections.Generic;

namespace Lokad.Syntax.Lexer
{
    /// <summary> A lexing rule: a pattern and its token. </summary>
    public struct LexerRule<TTok>
    {
        /// <summary> The definition for this rule. </summary>
        public readonly TokenDefinition Definition;

        /// <summary> The token to be returned. </summary>
        public readonly TTok Token;

        /// <summary> True if this is a public child of another token. </summary>
        public readonly bool IsPublicChild;

        /// <summary>
        /// More specific tokens that can be matched once this token
        /// is recognized.
        /// </summary>
        public readonly IReadOnlyList<LexerRule<TTok>> SubTokens; 

        public LexerRule(TokenDefinition definition, TTok token, bool publicChild = false, IReadOnlyList<LexerRule<TTok>> subTokens = null)
        {
            Definition = definition;
            Token = token;
            SubTokens = subTokens;
            IsPublicChild = publicChild;
        }

        public override string ToString() => $"{Token} = {Definition}";
    }
}