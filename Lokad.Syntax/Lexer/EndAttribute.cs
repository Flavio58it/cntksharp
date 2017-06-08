using System;

namespace Lokad.Syntax.Lexer
{
    /// <summary> Marks a token as the end-of-stream. </summary>
    /// <remarks> It is recommended that the default token bear this attribute. </remarks>
    public sealed class EndAttribute : Attribute {}
}
