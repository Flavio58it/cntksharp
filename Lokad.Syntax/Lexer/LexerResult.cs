using System.Collections.Generic;
using Lokad.Syntax.Parser;

namespace Lokad.Syntax.Lexer
{
    /// <summary> The result of tokenizing an input buffer. </summary>
    public sealed class LexerResult<TTok>
    {
        /// <summary> The buffer from which this result was obtained. </summary>
        public string Buffer { get; }

        /// <summary> Does this buffer contain invalid tokens ? </summary>
        public bool HasInvalidTokens { get; }

        /// <summary> The tokens extracted from the buffer. </summary>
        public IReadOnlyList<LexerToken<TTok>> Tokens { get; }

        /// <summary> The position of all newlines in the buffer, in ascending order. </summary>
        /// <remarks> Used for <see cref="LineOfPosition"/></remarks>
        public IReadOnlyList<int> Newlines { get; }

        /// <summary> Total number of tokens. </summary>
        public readonly int Count;

        /// <summary> Get the string at the specified token position. </summary>
        public string GetString(int pos)
        {
            var tok = Tokens[pos];
            if (tok.Start == Buffer.Length) return "";
            if (Buffer[tok.Start] == '\n' || Buffer[tok.Start] == '\r') return "";
            return Buffer.Substring(tok.Start, tok.Length);
        }

        /// <summary> Get the string and source-span at the specified token position. </summary>
        public Pos<string> GetStringPos(int pos)
        {
            var tok = Tokens[pos];

            int line, column;
            LineOfPosition(tok.Start, out line, out column);

            var span = new SourceSpan(new SourceLocation(tok.Start, line, column), tok.Length);
            var str = Buffer.Substring(tok.Start, tok.Length);

            return new Pos<string>(str, span);
        }
        
        public LexerResult(string buffer, List<LexerToken<TTok>> tokens, List<int> newlines, bool hasInvalidTokens)
        {
            Buffer = buffer;
            Tokens = tokens;
            Newlines = newlines;
            Count = tokens.Count;
            HasInvalidTokens = hasInvalidTokens;
        }

        /// <summary> Return the line and column on which a certain position appears. </summary>
        public void LineOfPosition(int position, out int line, out int column)
        {
            var a = 0;
            var b = Newlines.Count;

            if (b == 0 || position < Newlines[0])
            {
                line = 1; // +1 for one-based
                column = position + 1; // +1 for one-based
                return;
            }

            var last = Newlines[b - 1];
            if (position > last)
            {
                line = b + 1; // +1 for one-based
                column = position - last;
                return;
            }
            
            while (a + 1 < b)
            {
                var m = (a + b)/2;
                var v = Newlines[m];
                if (position <= v) b = m;
                else a = m;
            }

            // 'a' is the index of the last newline that is before the specified
            // position, meaning the actual line is 'a + 1' (e.g. the line after 
            // newline 10 is line 11).            
            line = a + 2; // +1 for above, +1 for one-based
            column = position - Newlines[a];
        } 
    }
}