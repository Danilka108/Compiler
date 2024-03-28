namespace Compiler.parser;

public enum LexemeType
{
    ConstKeyword = 1,
    StrKeyword = 2,
    Identifier = 3,
    StringLiteral = 4,
    Colon = 5,
    Ampersand = 6,
    AssignmentOperator = 7,
    OperatorEnd = 8,
    Separator = 9
}