using System;

namespace Lokad.Syntax.Lexer
{
    /// <summary> Marks a token as the error token. </summary>
    /// <remarks> 
    /// Error tokens are generated when the lexer fails to extract
    /// a correct token. 
    /// </remarks>
    public sealed class ErrorAttribute : Attribute {}
}
