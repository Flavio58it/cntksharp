using System;

namespace Lokad.Syntax.Lexer
{
    /// <summary> Marks a token as being an indent token. </summary>
    /// <remarks>
    /// Unlike normal tokens, the indent and <see cref="DedentAttribute"/>
    /// tokens are generated in packets. Every time a (non-ignored) newline
    /// is followed by spaces, the space length (assume tab = 4) is compared
    /// with that of the currently active indent. If it is longer, the
    /// active indent is pushed onto a stack, an indent token is returned
    /// and the new indent becomes active. If it is shorter, then 
    /// indents are popped from the stack until one of them has the same
    /// length, and one dedent token is generated for each. If the stack
    /// contains no equal indent, the largest indent smaller than the new
    /// one is kept, and an indent token is generated afterwards. The
    /// indent stack is emptied before end-of-stream.
    /// </remarks>
    /// <example>
    /// A [indent]
    ///   B [indent]
    ///     C [dedent]
    ///   D [indent]
    ///      E [dedent] [dedent]
    /// F [indent]
    ///   H [dedent] [indent]
    ///  I [dedent]
    /// </example>
    public sealed class IndentAttribute : Attribute { }
}
