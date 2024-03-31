using System.Linq;

namespace Compiler.Parser;

public static class Fixer
{
    public static string Fix(string content)
    {
        while (true)
        {
            var errors = Parser.Scan(content).ToArray();
            if (errors.Length == 0) return content;

            content = errors.Aggregate(content, FixParseErrors);
        }
    }

    private const string StringDefaultValue = "\"change me\"";
    private const string ConstKeywordValue = "const";
    private const string IdentifierDefaultValue = "change_me";
    private const string TypeDividerValue = ":";
    private const string LinkValue = "&";
    private const string StrTypeValue = "str";
    private const string AssignmentOperatorValue = "=";
    private const string OperatorEndValue = ";";
    private const string SeparatorValue = " ";

    private static string FixParseErrors(string content, ParseError error)
    {
        var value = content.Remove(error.Span.Start, error.Span.Count);
        return error.Type switch
        {
            ParseErrorType.UnexpectedSymbol => value,
            ParseErrorType.UnterminatedString => value.Insert(error.Span.Start, StringDefaultValue),
            ParseErrorType.ConstKeywordExpected => value.Insert(error.Span.Start, ConstKeywordValue),
            ParseErrorType.IdentifierExpected => value.Insert(error.Span.Start, IdentifierDefaultValue),
            ParseErrorType.TypeDividerExpected => value.Insert(error.Span.Start, TypeDividerValue),
            ParseErrorType.LinkExpected => value.Insert(error.Span.Start, LinkValue),
            ParseErrorType.StrTypeExpected => value.Insert(error.Span.Start, StrTypeValue),
            ParseErrorType.AssignmentOperatorExpected => value.Insert(error.Span.Start, AssignmentOperatorValue),
            ParseErrorType.StringLiteralExpected => value.Insert(error.Span.Start, StringDefaultValue),
            ParseErrorType.OperatorEndExpected => value.Insert(error.Span.Start, OperatorEndValue),
            ParseErrorType.NothingExpected => value,
            ParseErrorType.SeparatorExpected => value.Insert(error.Span.Start, SeparatorValue),
        };
    }
}