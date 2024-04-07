using System.Collections;
using Optional;

namespace Analysis;

public interface IState<TLexemeType> where TLexemeType : struct, Enum
{
    TLexemeType? CurrentLexemeType { get; }

    bool IsEnd();

    IEnumerable<IState<TLexemeType>> NextStates();

    IState<TLexemeType>? NextState(bool areLexemesRemained);
}