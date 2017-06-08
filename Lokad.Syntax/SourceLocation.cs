namespace Lokad.Syntax
{
    /// <summary> A position within the parsed script. </summary>
    public struct SourceLocation
    {
        /// <summary> Position within the input file. </summary>
        public readonly int Position;
        
        /// <summary>Line number, 1-based.</summary>
        public readonly int Line;

        /// <summary>Source column number, 1-based.</summary>        
        public readonly int Column;

        public SourceLocation(int position, int line, int column)
        {
            Position = position;
            Line = line;
            Column = column;
        }

        public override string ToString() => $"{Line}:{Column}";

        #region Equality

        public bool Equals(SourceLocation other)
        {
            return Position == other.Position && Line == other.Line && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SourceLocation && Equals((SourceLocation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position;
                hashCode = (hashCode*397) ^ Line;
                hashCode = (hashCode*397) ^ Column;
                return hashCode;
            }
        }

        #endregion
    }
}