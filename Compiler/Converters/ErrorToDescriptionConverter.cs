using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.Parser;
using Compiler.Parsing;
using Compiler.ViewModels;

namespace Compiler.Converters;

public class ErrorToDescriptionConverter : IValueConverter
{
    private static readonly IDictionary<InvalidLexemeType, string> TokenErrorMatches =
        new Dictionary<InvalidLexemeType, string>
        {
            { InvalidLexemeType.UnexpectedSymbol, Lang.Resources.TokenErrorUnexpectedSymbol },
            { InvalidLexemeType.UnterminatedString, Lang.Resources.TokenErrorUnterminatedString },
            // { TokenError.IdentifierCanOnlyStartWithANumber, Lang.Resources.TokenErrorIdentifierCanOnlyStartWithALetter }
        };

    private static readonly IDictionary<ParseErrorType, string> ParseErrorTypeMatches =
        new Dictionary<ParseErrorType, string>
        {
            { ParseErrorType.UnexpectedSymbol, Lang.Resources.TokenErrorUnexpectedSymbol },
            { ParseErrorType.UnterminatedString, Lang.Resources.TokenErrorUnterminatedString },
            { ParseErrorType.ConstKeywordExpected, Lang.Resources.ConstKeywordExpected },
            { ParseErrorType.IdentifierExpected, Lang.Resources.IdentifierExpected },
            { ParseErrorType.TypeDividerExpected, Lang.Resources.TypeDividerExpected },
            { ParseErrorType.LinkExpected, Lang.Resources.LinkExpected },
            { ParseErrorType.StrTypeExpected, Lang.Resources.StrTypeExpected },
            { ParseErrorType.AssignmentOperatorExpected, Lang.Resources.AssignmentOperatorExpected },
            { ParseErrorType.StringLiteralExpected, Lang.Resources.StringLiteralExpected },
            { ParseErrorType.OperatorEndExpected, Lang.Resources.OperatorEndExpected },
            { ParseErrorType.NothingExpected, Lang.Resources.NothingExpected },
            { ParseErrorType.SeparatorExpected, Lang.Resources.SeparatorExpected }
        };

    private static readonly IDictionary<Parsing.Lexing.LexemeType, string> LexemeTypeMatches =
        new Dictionary<Parsing.Lexing.LexemeType, string>
        {
            { Parsing.Lexing.LexemeType.UnexpectedSymbol, Lang.Resources.TokenErrorUnexpectedSymbol },
            { Parsing.Lexing.LexemeType.UnterminatedStringLiteral, Lang.Resources.TokenErrorUnterminatedString },
            { Parsing.Lexing.LexemeType.ConstKeyword, Lang.Resources.TokenTypeConstKeyword },
            { Parsing.Lexing.LexemeType.Identifier, Lang.Resources.TokenTypeIdentifier },
            { Parsing.Lexing.LexemeType.Colon, Lang.Resources.TokenTypeColon },
            { Parsing.Lexing.LexemeType.Ampersand, Lang.Resources.TokenTypeAmpersand },
            { Parsing.Lexing.LexemeType.StrKeyword, Lang.Resources.TokenTypeStrKeyword },
            { Parsing.Lexing.LexemeType.AssignmentOperator, Lang.Resources.TokenTypeAssignmentOperator },
            { Parsing.Lexing.LexemeType.StringLiteral, Lang.Resources.TokenTypeStringLiteral },
            { Parsing.Lexing.LexemeType.OperatorEnd, Lang.Resources.TokenTypeOperatorEnd },
        };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is EditorErrorViewModel viewModel)
        {
            var lexemeDesc = viewModel.Error.Lexeme is { } l ? LexemeTypeMatches[l] : "";
            var tail = viewModel.Tail;

            var res = viewModel.Error.ErrorKind switch
            {
                ParsingErrorKind.LexemeExpected when viewModel.Error.Lexeme is not null => string.Format(
                    Lang.Resources.ParsingErrorLexemeExpected, lexemeDesc, tail),
                ParsingErrorKind.LexemeExpected when viewModel.Error.Lexeme is null => string.Format(
                    Lang.Resources.ParsingErrorNothingExpected, tail),
                ParsingErrorKind.InvalidLexeme when viewModel.Error.Lexeme is null => string.Format(
                    Lang.Resources.ParsingErrorInvalidLexeme, lexemeDesc, tail),
                _ => "",
            };

            return res;
        }

        // if (value is ParseErrorType errorType) return ParseErrorTypeMatches[errorType];
        // if (value is InvalidLexemeType tokenError) return TokenErrorMatches[tokenError];
        // if (value is ParseErrorType parseErrorType) return ParseErrorTypeMatches[parseErrorType];

        return BindingNotification.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}