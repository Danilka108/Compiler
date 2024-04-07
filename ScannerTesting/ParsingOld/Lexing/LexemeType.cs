namespace Compiler.Parsing.Lexing;

public enum LexemeType
{
    UnexpectedSymbol = 1,
    ConstKeyword,
    StrKeyword,
    Identifier,
    StringLiteral,
    UnterminatedStringLiteral,
    Colon,
    Ampersand,
    AssignmentOperator,
    OperatorEnd,
    Separator,
}