using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lokad.Syntax.Lexer
{
    /// <summary> A regular expression intended for client-side Javascript. </summary>
    public sealed class JsRegex
    {
        public readonly string Pattern;
        public readonly string Flags;

        public JsRegex(string pattern, string flags)
        {
            Pattern = pattern;
            Flags = flags;
        }

        public override string ToString() =>
            "/^" + Pattern.Replace("/", "\\/") + "/" + Flags;

        public string[] ToArray() => new[] {Pattern, Flags};
    }
    
    /// <summary> Describes how a token is recognized from the source. </summary>
    public sealed class TokenDefinition
    {
        // Current implementation relies on regular expressions. We could likely do 
        // much better, both in terms of allocation and in terms of pure speed of
        // traversal, if we implemented a custom recognition engine. However, 
        // tokenization is currently only a very small portion of script compilation
        // (about 2ms lex + parse on a typical script), so optimization efforts are
        // best spent elsewhere.
        
        /// <summary> A token recognized as a regular expression. </summary>
        internal TokenDefinition(Regex regularExpression, JsRegex jsPattern, int? maximumLength = null, string startsWith = null)
        {
            JsRegex = jsPattern;
            RegularExpression = regularExpression;
            MaximumLength = maximumLength ?? int.MaxValue;
            _startsWith = startsWith;
        }

        /// <summary> A token recognized as one of many strings. </summary>
        public TokenDefinition(IReadOnlyList<string> strings, bool caseSensitive = true)
        {
            var flags = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline;
            if (!caseSensitive) flags = flags | RegexOptions.IgnoreCase;

            // Order strings by descending length, because C# regular expressions match "a|b" from
            // left to right rather than the longest match.

            var pattern = string.Join("|", strings.OrderByDescending(s => s.Length).Select(Regex.Escape));

            RegularExpression = new Regex("\\G(" + pattern + ")", flags);
            MaximumLength = strings.Select(s => s.Length).Max();

            var chars = new HashSet<char>(strings.Select(s => s.ToLowerInvariant()[0]));
            chars.UnionWith(strings.Select(s => s.ToUpperInvariant()[0]));

            _startsWith = new string(chars.ToArray());

            JsRegex = new JsRegex(pattern, caseSensitive ? "" : "i");
        }

        /// <summary>
        /// Attempt to match the buffer contents, starting at the specified position.        
        /// </summary>
        /// <returns> The length of the match, 0 if no match. </returns>
        public int MatchLength(string buffer, int start)
        {
            var match = RegularExpression.Match(buffer, start);
            return match.Success && match.Index == start ? match.Length : 0;
        }

        /// <summary> The maximum length of such a token. </summary>        
        public int MaximumLength { get; }
        
        /// <summary> The regular expression that recognizes this token. </summary>
        private Regex RegularExpression { get; }

        public JsRegex JsRegex { get; }

        /// <see cref="StartsWith"/>
        private readonly string _startsWith;

        /// <summary> True if the match can start with this character. </summary>
        public bool StartsWith(char c) => _startsWith == null || _startsWith.IndexOf(c) != -1;        

        public override string ToString() => RegularExpression.ToString();
    }
}
