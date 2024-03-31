using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.Parser;

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

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ParseErrorType errorType) return ParseErrorTypeMatches[errorType];
        // if (value is InvalidLexemeType tokenError) return TokenErrorMatches[tokenError];
        // if (value is ParseErrorType parseErrorType) return ParseErrorTypeMatches[parseErrorType];

        return BindingNotification.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}