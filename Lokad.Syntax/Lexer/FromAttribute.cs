using System;

namespace Lokad.Syntax.Lexer
{
    /// <summary> Marks an enumeration value as a sub-token of another token. </summary>
    /// <remarks>
    /// You are encouraged to create a child class of <see cref="FromAttribute"/>
    /// adapted to your enumeration, to avoid casting to integer on every time.
    /// </remarks>
    /// <example> 
    /// enum MyEnum 
    /// {
    ///   [Pattern("[a-z]+")] Identifier,
    ///   [Any("if"), From((int)Identifier, true)] If
    /// }
    /// </example>
    /// <example>
    /// public sealed class FAttribute : FromAttribute 
    /// {
    ///   public FAttribute(MyEnum s, bool isPrivate = false) : base((int)s, isPrivate) {}
    /// }
    /// 
    /// enum MyEnum
    /// {
    ///   [Pattern("[a-z]")] Identifier,
    ///   [Any("if"), F(Identifier, true)] If
    /// }
    /// </example>
    public class FromAttribute : Attribute
    {
        /// <summary> The value of the parent enumeration. </summary>
        /// <remarks> 
        /// For C# reasons, this cannot be an object of the "true"
        /// enumeration, so we have to settle for an integer that will then
        /// be cast to the actual value.
        /// </remarks>
        public int Parent { get; }

        /// <summary> Is this a private child ?  </summary>
        /// <remarks> 
        /// Private children are not assumed to be instances of their parent
        /// (e.g 'If' is not an example of 'Identifier') and cannot be used as
        /// such in terminals.
        /// 
        /// Public children can be converted to their parent on-the-fly
        /// (e.g. 'Multiply' is an example of 'Operator') and can be used as 
        /// such in terminals. The general principle is that a terminal
        /// expecting a  parent is automatically enriched with all its public
        /// descendants.
        /// </remarks>
        public bool IsPrivate;

        public FromAttribute(int parent, bool isPrivate = false)
        {
            Parent = parent;
            IsPrivate = isPrivate;
        }
    }
}
