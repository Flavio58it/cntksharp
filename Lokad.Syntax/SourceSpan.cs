namespace Lokad.Syntax
{
    /// <summary> A position and length within the parsed script. </summary>
    public struct SourceSpan
    {
        public readonly SourceLocation Location;

        public readonly int Length;

        public SourceSpan(SourceLocation location, int length)
        {
            Location = location;
            Length = length;
        }

        public override string ToString() => 
            $"{Location.Line}:{Location.Column}-{Location.Column + Length}";

        #region Equality

        public bool Equals(SourceSpan other)
        {
            return Location.Equals(other.Location) && Length == other.Length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SourceSpan && Equals((SourceSpan) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Location.GetHashCode()*397) ^ Length;
            }
        }

        #endregion
    }
}