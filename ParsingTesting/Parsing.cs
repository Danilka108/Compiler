using Optional;
using Analysis;

namespace ParsingTesting;

public class Parsing
{
}

public readonly struct State : IState<LexemeType>
{
    public LexemeType? CurrentLexemeType { get; private init; }

    private bool IsInsideOfStruct { get; init; }

    public bool IsEnd()
    {
        return this is { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: false };
    }

    public IEnumerable<IState<LexemeType>> NextStates()
    {
        return this switch
        {
            { CurrentLexemeType: null } =>
            [
                new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = false }
            ],
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: false } =>
            [
                new State { CurrentLexemeType = LexemeType.StructKeyword, IsInsideOfStruct = false }
            ],
            { CurrentLexemeType: LexemeType.StructKeyword, IsInsideOfStruct: false } =>
            [
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = false }
            ],
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: false } =>
            [
                new State { CurrentLexemeType = LexemeType.OpenBracket, IsInsideOfStruct = false }
            ],
            { CurrentLexemeType: LexemeType.OpenBracket, IsInsideOfStruct: false } =>
            [
                new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true },
                new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false }
            ],
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: true } =>
            [
                new State { CurrentLexemeType = LexemeType.DataType, IsInsideOfStruct = true }
            ],
            { CurrentLexemeType: LexemeType.DataType, IsInsideOfStruct: true } =>
            [
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = true }
            ],
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: true } =>
            [
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = true }
            ],
            { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: true } =>
            [
                new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false },
                new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true }
            ],
            { CurrentLexemeType: LexemeType.CloseBracket, IsInsideOfStruct: false } =>
            [
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = false }
            ],
            _ => [],
        };
    }

    public IState<LexemeType>? NextState(bool areLexemesRemained)
    {
        return this switch
        {
            { CurrentLexemeType: null } =>
                new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.StructKeyword, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.StructKeyword, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.OpenBracket, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.OpenBracket, IsInsideOfStruct: false } =>
                areLexemesRemained
                    ? new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true }
                    : new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.DataType, IsInsideOfStruct = true },
            { CurrentLexemeType: LexemeType.DataType, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = true },
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = true },
            { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: true } =>
                areLexemesRemained
                    ? new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true }
                    : new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false },
            { CurrentLexemeType: LexemeType.CloseBracket, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = false },
            _ => null,
        };
    }
}

// public readonly struct State : IState<LexemeType>
// {
//     public Option<LexemeType> CurrentLexemeType { get; private init; }
//
//     public bool IsInsideOfStruct { get; private init; }
//
//     public bool IsEnd()
//     {
//         return CurrentLexemeType.Exists(lt => lt is LexemeType.EndOperator) && !IsInsideOfStruct;
//     }
//
//     public IEnumerable<IState<LexemeType>> NextPossibleStates()
//     {
//         foreach (var lexemeType in CurrentLexemeType)
//         {
//             return NextPossibleStates(lexemeType);
//         }
//
//         return NextState(LexemeType.AccessModifier, false);
//     }
//
//     private IEnumerable<IState<LexemeType>> NextPossibleStates(LexemeType lexemeType)
//     {
//         return lexemeType switch
//         {
//             LexemeType.AccessModifier
//                 ParsingTesting.LexemeType.ConstKeyword => NextState(ParsingTesting.LexemeType.Identifier),
//         ParsingTesting.LexemeType.Identifier => NextState(ParsingTesting.LexemeType.Colon),
//         ParsingTesting.LexemeType.Colon => NextState(ParsingTesting.LexemeType.Ampersand),
//         ParsingTesting.LexemeType.Ampersand => NextState(ParsingTesting.LexemeType.StrKeyword),
//         ParsingTesting.LexemeType.StrKeyword => NextState(ParsingTesting.LexemeType.AssignmentOperator),
//         ParsingTesting.LexemeType.AssignmentOperator => NextState(ParsingTesting.LexemeType.StringLiteral),
//         ParsingTesting.LexemeType.StringLiteral => NextState(ParsingTesting.LexemeType.OperatorEnd),
//         _ => [],
//         };
//     }
//
//     private IEnumerable<IState<LexemeType>> NextState(LexemeType lexemeType, bool isInsideOfStruct)
//     {
//         return [new State { CurrentLexemeType = lexemeType.Some(), IsInsideOfStruct = isInsideOfStruct }];
//     }
// }