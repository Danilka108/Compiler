using Analysis.Lexing;
using Optional.Collections;

namespace Analysis;

public class Parser<TLexemeType, TState>
    where TLexemeType : struct, Enum
    where TState : IState<TLexemeType>, new()
{
    private IState<TLexemeType> _state = new TState();

    private readonly ILexemeHelper<TLexemeType> _lexemeHelper;

    private readonly Lexeme<TLexemeType>[] _lexemes;

    private int _tailStart;

    private readonly List<ParsingError<TLexemeType>> _errors;

    private readonly List<ParsingError<TLexemeType>> _tempErrors;

    private readonly int _contentLength;

    public static IEnumerable<ParsingError<TLexemeType>> Parse(ILexemeHelper<TLexemeType> lexemeHelper,
        IEnumerable<Lexeme<TLexemeType>> lexemes)
    {
        var parser = new Parser<TLexemeType, TState>([..lexemes], lexemeHelper);

        while (true)
        {
            if (parser.DoIteration() is ControlFlow.Stop) break;
        }

        return parser._errors;
    }

    private Parser(Lexeme<TLexemeType>[] lexemes, ILexemeHelper<TLexemeType> lexemeHelper)
    {
        _lexemeHelper = lexemeHelper;
        _lexemes = lexemes.Where(lexeme => !_lexemeHelper.IsIgnorableLexeme(lexeme.Type)).ToArray();
        _contentLength = _lexemes.Length > 0 ? lexemes.Last().Span.End : 0;
        _tailStart = 0;
        _errors = new List<ParsingError<TLexemeType>>();
        _tempErrors = new List<ParsingError<TLexemeType>>();
    }

    private enum ControlFlow
    {
        Continue,
        Stop,
    }

    private void PushTempErrors()
    {
        _errors.AddRange(_tempErrors);
        _tempErrors.Clear();
    }

    private ControlFlow DoIteration()
    {
        if (IsEnd())
        {
            PushTempErrors();
            return ControlFlow.Stop;
        }

        if (AreUnprocessedLexemesRemained())
        {
            _state = new TState();
            return ControlFlow.Continue;
            // PushTempErrors();
            // _errors.Add(HandleUnprocessedLexemesAreRemained());
            // return ControlFlow.Stop;
        }

        if (AreLexemesExhausted())
        {
            _errors.Add(HandleLexemesAreExhausted());
            _state = _state.NextState(areLexemesRemained: false) ?? _state;
            return ControlFlow.Continue;
        }

        if (IsCurrentLexemeInvalid())
        {
            _errors.Add(HandleCurrentLexemeIsInvalid());
            _tailStart += 1;
            return ControlFlow.Continue;
        }

        for (var i = _tailStart; i < _lexemes.Length; i++)
        {
            var lexeme = _lexemes[i];

            if (_lexemeHelper.IsIgnorableLexeme(lexeme.Type))
            {
                continue;
            }

            if (IsBoundaryLexeme(lexeme))
            {
                break;
            }

            if (!TryMove(lexeme.Type))
            {
                continue;
            }

            if (i > _tailStart && _tempErrors.Count == 0)
            {
                _errors.Add(HandleExpectedLexemeNotFoundImmediately(lexeme));
            }

            ShiftEndOfTempErrors(lexeme);
            PushTempErrors();

            _tailStart = i + 1;
            return ControlFlow.Continue;
        }

        _tempErrors.Add(HandleExpectedLexemeNotFound());
        // Move();
        _state = _state.NextState(areLexemesRemained: false) ?? _state;
        return ControlFlow.Continue;
    }

    private void ShiftEndOfTempErrors(Lexeme<TLexemeType> lexeme)
    {
        for (var j = 0; j < _tempErrors.Count; j++)
        {
            if (lexeme.Span.Start > _tempErrors[j].Span.End)
            {
                _tempErrors[j] = _tempErrors[j].SetEnd(lexeme.Span.Start);
            }
        }
    }

    private bool IsEnd()
    {
        return _tailStart == _lexemes.Length && _state.IsEnd();
    }

    private bool AreUnprocessedLexemesRemained()
    {
        // return _tailStart < _lexemes.Length - 1 && _state.IsEnd();
        return _tailStart < _lexemes.Length && _state.IsEnd();
    }

    private ParsingError<TLexemeType> HandleUnprocessedLexemesAreRemained()
    {
        var lexeme = _lexemes[_tailStart];
        var span = new Span(_tailStart, _contentLength);

        return new ParsingError<TLexemeType>
        {
            Span = span,
            Lexeme = lexeme.Type,
            TailStart = span.Start,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
    }

    private bool AreLexemesExhausted()
    {
        return (_lexemes.Length == 0 || _tailStart == _lexemes.Length) && !_state.IsEnd();
    }

    private ParsingError<TLexemeType> HandleLexemesAreExhausted()
    {
        var lexemeType = _state.NextState(areLexemesRemained: false)?.CurrentLexemeType;
        var span = new Span(_contentLength, _contentLength);

        return new ParsingError<TLexemeType>
        {
            Span = span,
            Lexeme = lexemeType,
            TailStart = span.Start,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
    }

    private TLexemeType? GetNextPossibleLexeme()
    {
        return _state
            .NextStates()
            .FirstOrDefault()?.CurrentLexemeType;
    }

    private bool IsCurrentLexemeInvalid()
    {
        return _lexemeHelper.IsInvalidLexeme(CurrentLexeme().Type);
    }

    private ParsingError<TLexemeType> HandleCurrentLexemeIsInvalid()
    {
        var lexeme = CurrentLexeme();
        return new ParsingError<TLexemeType>
        {
            Span = lexeme.Span,
            Lexeme = lexeme.Type,
            TailStart = lexeme.Span.End,
            ErrorKind = ParsingErrorKind.InvalidLexeme,
        };
    }

    private ParsingError<TLexemeType> HandleExpectedLexemeNotFoundImmediately(Lexeme<TLexemeType> foundLexeme)
    {
        var span = new Span(CurrentLexeme().Span.Start, foundLexeme.Span.Start);
        return new ParsingError<TLexemeType>
        {
            Span = span,
            Lexeme = foundLexeme.Type,
            TailStart = foundLexeme.Span.End,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
    }

    private ParsingError<TLexemeType> HandleExpectedLexemeNotFound()
    {
        var currentLexeme = CurrentLexeme();
        var expectedLexemeType = GetNextPossibleLexeme();

        var span = new Span(currentLexeme.Span.Start, currentLexeme.Span.End);
        return new ParsingError<TLexemeType>
        {
            Span = span,
            Lexeme = expectedLexemeType,
            TailStart = currentLexeme.Span.Start,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
    }

    private Lexeme<TLexemeType> CurrentLexeme()
    {
        return _lexemes[_tailStart];
    }

    private bool TryMove(TLexemeType nextLexemeType)
    {
        var maybeNextState = _state
            .NextStates()
            .Where(s => s.CurrentLexemeType.Equals(nextLexemeType))
            .FirstOrNone();

        foreach (var nextState in maybeNextState)
        {
            _state = nextState;
            return true;
        }

        return false;
    }

    private bool IsBoundaryLexeme(Lexeme<TLexemeType> lexeme)
    {
        return _state.NextStates().SelectMany(s => s.NextStates())
            .Any(s => s.CurrentLexemeType.Equals(lexeme.Type));
    }
}