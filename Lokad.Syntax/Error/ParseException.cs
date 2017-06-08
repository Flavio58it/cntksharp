using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lokad.Syntax.Error
{
    public sealed class ParseException : Exception
    {
        /// <summary> The token that caused an exception. </summary>
        public string Token { get; }

        /// <summary> The list of expected tokens. </summary>
        public IReadOnlyList<string> Expected { get; }

        /// <summary>
        /// Where the token was found.
        /// </summary>
        public readonly SourceSpan Location;

        public ParseException(string token, IReadOnlyList<string> expected, SourceSpan location) 
            : base(expected.Count == 0 
                  ? $"Syntax error, unexpected {token}." 
                  : $"Syntax error, found {token} but expected {Expect(expected)}.")
        {
            Token = token;
            Expected = expected;
            Location = location;

            // Always select one character.
            if (location.Length == 0) Location = new SourceSpan(location.Location, 1);
        }

        /// <summary> Pretty-printing an expectation list. </summary>
        private static string Expect(IReadOnlyList<string> expected)
        {
            Debug.Assert(expected.Count > 0);

            if (expected.Count == 1) return expected[0];

            var sb = new StringBuilder();

            sb.Append(expected[0]);
            for (var i = 1; i < expected.Count - 1; ++i)
            {
                sb.Append(", ");
                sb.Append(expected[i]);
            }
            sb.Append(" or ");
            sb.Append(expected[expected.Count - 1]);

            return sb.ToString();
        }
    }
}
