using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Animation.Easings;
using Scanner;

namespace Compiler;

public class Fixer
{
    private readonly string _content;
    private readonly Token<TokenType, TokenError>[] _tokens;

    public Fixer(string content)
    {
        _content = content;
        _tokens = new Scanner<TokenType, TokenError>(content, TokensScanners.TokenScanners).ToArray();
    }

    public string Fix()
    {
        var content = FixTokenErrors();

        while (true)
        {
            var newContent = FixParserErrors(content);
            if (newContent == content) break;

            content = newContent;
        }

        return content;
    }

    private string FixParserErrors(string content)
    {
        var tokens = new Scanner<TokenType, TokenError>(content, TokensScanners.TokenScanners);
        var parser = new Parser(tokens);

        foreach (var parseError in parser)
        {
            var replacement = parseError.Type switch
            {
                ParseErrorType.ConstKeywordExpected => "const",
                ParseErrorType.IdentifierExpected => "change_me",
                ParseErrorType.TypeDividerExpected => ":",
                ParseErrorType.LinkExpected => "&",
                ParseErrorType.StrTypeExpected => "str",
                ParseErrorType.AssignmentOperatorExpected => "=",
                ParseErrorType.StringLiteralExpected => "\"change me\"",
                ParseErrorType.OperatorEndExpected => ";",
                ParseErrorType.NothingExpected => ""
            };

            // newContent.Remove(parseError.Span.Start, parseError.Span.End - parseError.Span.Start);
            return content.Insert(parseError.Span.Start, replacement);
        }

        return content;
    }

    private string FixTokenErrors()
    {
        var validTokens = _tokens
            .Select(token => token as Token<TokenType, TokenError>.ValidToken)
            .Where(t => t is not null)
            .Cast<Token<TokenType, TokenError>.ValidToken>().ToArray();

        return string.Join(
            "",
            validTokens.Select(t => _content.Substring(t.Span.Start, t.Span.End - t.Span.Start))
        );
    }
}