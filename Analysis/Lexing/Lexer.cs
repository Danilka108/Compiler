using Optional;
using Optional.Unsafe;

namespace Analysis.Lexing;

public class Lexer<TLexemeType> where TLexemeType : Enum
{
    public delegate Option<TLexemeType> LexemeEater(Eater eater);

    public static IEnumerable<Lexeme<TLexemeType>> Scan(ILexemeHelper<TLexemeType> lexemeHelper,
        IEnumerable<LexemeEater> lexemeEaters, string content)
    {
        var lexer = new Lexer<TLexemeType>(lexemeHelper, [..lexemeEaters], new Caret(content));

        while (!lexer._caret.IsEnd())
        {
            lexer.DoIteration();
        }

        return lexer._lexemes;
    }

    private readonly ILexemeHelper<TLexemeType> _lexemeHelper;
    private readonly LexemeEater[] _lexemeEaters;
    private readonly Caret _caret;
    private readonly List<Lexeme<TLexemeType>> _lexemes;

    private Lexer(ILexemeHelper<TLexemeType> lexemeHelper, LexemeEater[] lexemeEaters, Caret caret)
    {
        _lexemeHelper = lexemeHelper;
        _lexemeEaters = lexemeEaters;
        _caret = caret;
        _lexemes = new List<Lexeme<TLexemeType>>();
    }

    private void DoIteration()
    {
        foreach (var lexemeEaterFunc in _lexemeEaters)
        {
            var eater = _caret.StartEating();
            var lexemeType = lexemeEaterFunc(eater);

            if (!lexemeType.HasValue) continue;

            var span = _caret.FinishEating(eater).NewSpan;
            var lexeme = lexemeType.ValueOrFailure().IntoLexeme(span);
            _lexemes.Add(lexeme);

            return;
        }

        var invalidLexeme = _lexemeHelper.UnexpectedSymbol().IntoLexeme(_caret.Span().ShiftEnd(1));
        _lexemes.Add(invalidLexeme);
        _caret.Move();
    }
}