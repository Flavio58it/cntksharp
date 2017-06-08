using System;

namespace Lokad.Syntax.Lexer
{
    /// <summary> Placed on an enumeration of tokens to provide general information. </summary>
    public sealed class TokensAttribute : Attribute
    {
        /// <summary> Is it possible to escape newlines with the '\' character ? </summary>
        /// <remarks>
        /// If nohing separates a newline from a preceding '\', then both are canceled
        /// out. Comments count as nothing.
        /// </remarks>
        public bool EscapeNewlines { get; set; }

        /// <summary> A pattern for comments. </summary>
        /// <example> "//[^\n]*" </example>
        public string Comments { get; set; }
    }
}
