using System.Text.RegularExpressions;

namespace Fenicia.Common;

public static class AdvancedQueryParser
{
    private static readonly Regex _filterRegex = new(@"^(?<property>[^\[\]]+?)\[(?<operator>[^\[\]]*?)\](?<value>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static List<QueryFilter> Parse(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var filters = new List<QueryFilter>();
        var parts = query.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            var subParts = part.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var subPart in subParts)
            {
                var match = _filterRegex.Match(subPart);
                if (!match.Success)
                {
                    continue;
                }

                var property = match.Groups["property"].Value;
                var operatorStr = match.Groups["operator"].Value;
                var value = match.Groups["value"].Value;

                if (!TryParseOperator(operatorStr, out var op))
                {
                    continue;
                }

                filters.Add(new QueryFilter(property, op, value));
            }
        }

        return filters;
    }

    private static bool TryParseOperator(string op, out QueryOperator queryOperator)
    {
        queryOperator = op switch
        {
            "" or "=" => QueryOperator.Equals,
            "!=" => QueryOperator.NotEquals,
            "*" => QueryOperator.Contains,
            ">" => QueryOperator.GreaterThan,
            "<" => QueryOperator.LessThan,
            ">=" => QueryOperator.GreaterThanOrEqual,
            "<=" => QueryOperator.LessThanOrEqual,
            _ => QueryOperator.Equals
        };

        return true;
    }
}
