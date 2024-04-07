using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing.Lexing;

namespace Compiler.Parsing;

public class Parser<TLexemeType, TState>
    where TState : IState<TLexemeType>, new()
{
    private IState<TLexemeType> _state = new TState();

    private readonly LexemeType[] _lexemes;

    private int _tailStart;

    private readonly List<ParsingError<TLexemeType>> _errors;

    private readonly int _contentLength;

    private readonly ILexemeChecker<TLexemeType> _lexemeChecker;

    public static IEnumerable<ParsingError<TLexemeType>> Parse(IEnumerable<LexemeType> lexemes)
    {
        var parser = new Parser<TLexemeType, TState>([..lexemes]);

        while (true)
        {
            if (parser.DoIteration() is ControlFlow.Stop) break;
        }

        return parser._errors;
    }

    private Parser(Lexeme<TLexemeType>[] lexemes, ILexemeChecker<TLexemeType> lexemeChecker)
    {
        _lexemeChecker = lexemeChecker;
        _lexemes = lexemes.Where(lexeme => !_lexemeChecker.IsIgnorableLexeme(lexeme.Type)).ToArray();
        _contentLength = _lexemes.Length > 0 ? lexemes.Last().Span.End : 0;
        _tailStart = 0;
        _errors = new List<ParsingError<TLexemeType>>();
    }

    private enum ControlFlow
    {
        Continue,
        Stop,
    }

    private bool _ignoreNextError = false;

    private ControlFlow DoIteration()
    {
        if (IsEnd())
        {
            return ControlFlow.Stop;
        }

        if (AreUnprocessedLexemesRemained())
        {
            return HandleUnprocessedLexemesAreRemained();
        }

        if (AreLexemesExhausted())
        {
            return HandleLexemesAreExhausted();
        }

        if (IsCurrentLexemeInvalid())
        {
            return HandleCurrentLexemeIsInvalid();
        }

        for (var i = _tailStart; i < _lexemes.Length; i++)
        {
            var lexeme = _lexemes[i];

            if (_state.IsIgnorableLexeme(lexeme.Type))
            {
                continue;
            }

            if (_state.IsBoundaryLexeme(lexeme.Type))
            {
                break;
            }

            if (!TryMove(lexeme.Type)) continue;

            if (i > _tailStart && _ignoreNextError)
            {
                HandleExpectedLexemeNotFoundImmediately(lexeme);
            }

            _ignoreNextError = false;
            _tailStart = i + 1;

            return ControlFlow.Continue;
        }


        _ignoreNextError = true;
        return HandleExpectedLexemeNotFound();
    }

    private bool IsEnd()
    {
        return _tailStart == _lexemes.Length && _state.IsEnd();
    }

    private bool AreUnprocessedLexemesRemained()
    {
        return _tailStart != _lexemes.Length && _state.IsEnd();
    }

    private ControlFlow HandleUnprocessedLexemesAreRemained()
    {
        var lexeme = _lexemes[_tailStart];
        var span = new Span(_contentLength, _contentLength);

        var error = new ParsingError
        {
            Span = span,
            Lexeme = lexeme.Type,
            TailStart = lexeme.Span.Start,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };

        _errors.Add(error);
        return ControlFlow.Stop;
    }

    private bool AreLexemesExhausted()
    {
        return _lexemes.Length == 0 && !_state.IsEnd();
    }

    private ControlFlow HandleLexemesAreExhausted()
    {
        var lexeme = _state.NextPossibleStates().First();
        var span = new Span(_contentLength, _contentLength);
        var error = new ParsingError
        {
            Span = span,
            Lexeme = lexeme.Lexeme,
            TailStart = _contentLength,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };

        _errors.Add(error);

        Move();
        return ControlFlow.Continue;
    }

    private bool IsCurrentLexemeInvalid()
    {
        return _state.IsInvalidLexeme(CurrentLexeme().Type);
    }

    private ControlFlow HandleCurrentLexemeIsInvalid()
    {
        var lexeme = CurrentLexeme();
        var error = new ParsingError
        {
            Span = lexeme.Span,
            Lexeme = lexeme.Type,
            TailStart = lexeme.Span.End,
            ErrorKind = ParsingErrorKind.InvalidLexeme,
        };

        _errors.Add(error);

        _tailStart += 1;
        return ControlFlow.Continue;
    }

    private void HandleExpectedLexemeNotFoundImmediately(LexemeType foundLexemeType)
    {
        var span = new Span(CurrentLexeme().Span.Start, foundLexemeType.Span.Start);
        var error = new ParsingError
        {
            Span = span,
            Lexeme = foundLexemeType.Type,
            TailStart = foundLexemeType.Span.End,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
        _errors.Add(error);
    }

    private ControlFlow HandleExpectedLexemeNotFound()
    {
        var prevLexeme = _lexemes[_tailStart - 1 < 0 ? 0 : _tailStart - 1];
        var currentLexeme = CurrentLexeme();
        var expectedLexeme = _state.NextPossibleStates().First().Lexeme;

        var span = new Span(prevLexeme.Span.Start, currentLexeme.Span.Start);
        var error = new ParsingError
        {
            Span = span,
            Lexeme = expectedLexeme,
            TailStart = currentLexeme.Span.Start,
            ErrorKind = ParsingErrorKind.LexemeExpected,
        };
        _errors.Add(error);

        Move();
        return ControlFlow.Continue;
    }

    private LexemeType CurrentLexeme()
    {
        return _lexemes[_tailStart];
    }

    private bool TryMove(Lexing.LexemeType nextLexeme)
    {
        var nextState = _state.NextPossibleStates()
            .FirstOrDefault(possibleState => possibleState.Lexeme == nextLexeme);

        if (nextState is not null && nextState.Lexeme == nextLexeme)
        {
            _state = nextState;
            return true;
        }

        return false;
    }

    private void Move()
    {
        var nextState = _state.NextPossibleStates().FirstOrDefault();
        if (nextState is not null) _state = nextState;
    }
}