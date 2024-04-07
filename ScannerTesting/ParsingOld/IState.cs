using System.Collections.Generic;

namespace Compiler.Parsing;

public interface IState<out TLexemeType>
{
    TLexemeType? Lexeme { get; }

    bool IsEnd();

    IEnumerable<IState<TLexemeType>> NextPossibleStates();
}