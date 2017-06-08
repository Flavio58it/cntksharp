using System.Collections.Generic;

namespace Lokad.Syntax.Error
{
    /// <summary> Provides names for tokens of the specified type. </summary>
    public interface ITokenNamer<TTok>
    {
        /// <summary> The contextual name of a token among a list of tokens. </summary>
        /// <remarks> Allowed to return null. </remarks>
        string TokenName(TTok t, ICollection<TTok> others);
    }
}
