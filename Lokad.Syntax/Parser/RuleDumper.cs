using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Lokad.Syntax.Lexer;

namespace Lokad.Syntax.Parser
{
    public class RuleDumper
    {
        /// <summary> The names of tokens. </summary>
        private readonly IReadOnlyList<string> _tokenNames;

        private readonly IReadOnlyList<string> _typePrefixes;

        /// <summary> Gathering of grammar rules </summary>
        private readonly Dictionary<string, List<string>> _productions;

        private string EnumFieldInfo(FieldInfo finfo)
        {
            var key = $"'{finfo.Name.ToLower()}'";
            var pattCi = finfo.GetCustomAttribute<PatternCiAttribute>();

            if (pattCi != null)
            {
                var patLst = new List<string> { "/" + pattCi.Pattern + "/"};
                _productions.Add(key, patLst);
                return key;
            }

            var anyCi = finfo.GetCustomAttribute<AnyAttribute>();

            if (anyCi == null) return $"'{finfo.Name.ToLower()}'";

            if (anyCi.Options.Count == 1)
                return $"'{anyCi.Options[0]}'";

            var lst = anyCi.Options.Select(v => $"'{v}'").ToList();
            _productions.Add(key, lst);
            return key;
        }

        private RuleDumper(Type t, Type token, IReadOnlyList<string> typePrefixes)
        {
            _productions = new Dictionary<string, List<string>>();
            _tokenNames = token.GetFields().Select(EnumFieldInfo).Skip(1).ToArray();
            _typePrefixes = typePrefixes;

            foreach (var m in t.GetMethods())
            {
                var ruleAttribute = m.GetCustomAttribute<RuleAttribute>();
                if (ruleAttribute == null) continue;
                ProcessMethod(m);
            }
        }

        public static string ToEBNFGrammar(
            Type t,
            Type tok,
            IReadOnlyList<string> typePrefix)
        {
            var dumper = new RuleDumper(t, tok, typePrefix);
            return dumper.AssembleGrammar();
        }

        private string AssembleGrammar()
        {
            var sb = new StringBuilder();

            foreach (var production in _productions)
            {
                sb.AppendLine($"{ToDisplayName(production.Key)} ::=");

                foreach (var p in production.Value)
                    sb.AppendLine($"  | {p}");

                sb.AppendLine("");
            }

            return sb.ToString();
        }

        static readonly Regex InnerClassRegex = new Regex(".*\\+(.*)");

        private string ToDisplayName(Type t) => ToDisplayName(t.ToString());

        private string ToDisplayName(string str)
        {
            var innerMatch = InnerClassRegex.Match(str);
            if (innerMatch.Success)
                return innerMatch.Groups[1].Value;

            foreach (var pref in _typePrefixes)
            {
                if (str.StartsWith(pref))
                    return str.Substring(pref.Length);
            }

            return str;
        }

        private void ProcessMethod(MethodInfo m)
        {
            var prod = "";
            foreach (var p in m.GetParameters())
            {
                var nta = p.GetCustomAttribute<NonTerminalAttribute>();
                if (nta != null)
                {
                    var pType = p.ParameterType;
                    if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        pType = pType.GetGenericArguments()[0];

                    if (nta.Optional)
                        prod += $" [{ToDisplayName(pType)}]";
                    else
                        prod += " " + ToDisplayName(pType);
                }

                var ta = p.GetCustomAttribute<TerminalAttribute>();
                if (ta != null)
                {
                    if (ta.Tokens.Count == 1)
                    {
                        if (ta.Optional)
                            prod += " [" + _tokenNames[ta.Tokens[0]] + "]";
                        else
                            prod += " " + _tokenNames[ta.Tokens[0]];
                    }
                    else
                    {
                        var tokens = string.Join(" | ", ta.Tokens.Select(ix => _tokenNames[ix]));
                        if (ta.Optional)
                            prod += " [" + tokens + "]";
                        else
                            prod += " (" + tokens + ")";
                    }
                }

                var la = p.GetCustomAttribute<ListAttribute>();
                if (la != null)
                {
                    var sep = la.Separator.HasValue ? _tokenNames[la.Separator.Value] : "";
                    var pType = p.ParameterType;

                    if (pType.IsArray) pType = pType.GetElementType();

                    var tStr = ToDisplayName(pType);

                    if (la.Min > 0)
                        prod += $" {tStr} {{{sep} {tStr}}}";
                    else
                        prod += $" [{tStr} {{{sep} {tStr}}}]";

                }
            }

            var productionKey = m.ReturnType.ToString();
            List<string> prods;

            if (!_productions.TryGetValue(productionKey, out prods))
            {
                prods = new List<string>();
                _productions.Add(productionKey, prods);
            }

            prods.Add(prod);

        }
    }
}
