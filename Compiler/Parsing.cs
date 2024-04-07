using System.Collections.Generic;
using System.Linq;
using Optional;
using Analysis;

namespace Compiler;

public class Parsing
{
}

public readonly struct State : IState<LexemeType>
{
    public Option<LexemeType> CurrentLexemeType { get; private init; }

    public bool IsEnd()
    {
        return CurrentLexemeType.Exists(lt => lt is Compiler.LexemeType.OperatorEnd);
    }

    public IEnumerable<IState<LexemeType>> NextStates()
    {
        foreach (var lexemeType in CurrentLexemeType)
        {
            return NextPossibleStates(lexemeType);
        }

        return NextState(Compiler.LexemeType.ConstKeyword);
    }

    private IEnumerable<IState<LexemeType>> NextPossibleStates(LexemeType lexemeType)
    {
        return lexemeType switch
        {
            Compiler.LexemeType.ConstKeyword => NextState(Compiler.LexemeType.Identifier),
            Compiler.LexemeType.Identifier => NextState(Compiler.LexemeType.Colon),
            Compiler.LexemeType.Colon => NextState(Compiler.LexemeType.Ampersand),
            Compiler.LexemeType.Ampersand => NextState(Compiler.LexemeType.StrKeyword),
            Compiler.LexemeType.StrKeyword => NextState(Compiler.LexemeType.AssignmentOperator),
            Compiler.LexemeType.AssignmentOperator => NextState(Compiler.LexemeType.StringLiteral),
            Compiler.LexemeType.StringLiteral => NextState(Compiler.LexemeType.OperatorEnd),
            _ => [],
        };
    }

    private IEnumerable<IState<LexemeType>> NextState(LexemeType lexemeType)
    {
        return [new State { CurrentLexemeType = lexemeType.Some() }];
    }
}