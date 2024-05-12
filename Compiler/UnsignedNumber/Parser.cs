using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalysis;

namespace Compiler.UnsignedNumber;

internal class State
{
    private readonly Lexeme<LexemeType>[] _lexemes;
    private int _currentLexemeIndex;
    private readonly StringBuilder _result;

    public State(IEnumerable<Lexeme<LexemeType>> lexemes)
    {
        _lexemes = lexemes.ToArray();
        _currentLexemeIndex = 0;
        _result = new StringBuilder();
    }

    public State(State state)
    {
        _lexemes = state._lexemes;
        _currentLexemeIndex = state._currentLexemeIndex;
        _result = new StringBuilder();
        _result.Append(state._result);
    }

    public bool Match(LexemeType expectedType)
    {
        if (_currentLexemeIndex < _lexemes.Length &&
            _lexemes[_currentLexemeIndex].Type == expectedType)
        {
            _currentLexemeIndex++;
            return true;
        }

        return false;
    }

    public void Append(string value)
    {
        _result.Append("-" + value);
    }

    public void Append(State state)
    {
        _currentLexemeIndex = state._currentLexemeIndex;
        _result.Clear();
        _result.Append(state._result);
    }

    public override string ToString()
    {
        return _result.ToString();
    }
}

public class Parser
{
    public string Parse(IEnumerable<Lexeme<LexemeType>> lexemes)
    {
        var state = new State(lexemes);
        return UnsignedNumber(state) ? state.ToString() : "";
    }

    private bool UnsignedNumber(State state)
    {
        state.Append("unsigned_number");

        if (Branch(state, s => DecimalNumber(s) && ExponentialPart(s)))
        {
            return true;
        }

        if (Branch(state, DecimalNumber))
        {
            return true;
        }

        if (Branch(state, ExponentialPart))
        {
            return true;
        }

        return false;
    }

    private bool DecimalNumber(State state)
    {
        state.Append("decimal_number");

        if (Branch(state, s => UnsignedInteger(s) && DecimalFraction(s)))
        {
            return true;
        }

        if (Branch(state, UnsignedInteger))
        {
            return true;
        }

        if (Branch(state, DecimalFraction))
        {
            return true;
        }

        return false;
    }

    private bool DecimalFraction(State state)
    {
        state.Append("decimal_fraction");

        if (state.Match(LexemeType.Dot))
        {
            state.Append(".");
            return UnsignedInteger(state);
        }

        return false;
    }

    private bool ExponentialPart(State state)
    {
        state.Append("exponential_part");

        if (state.Match(LexemeType.Ten))
        {
            state.Append("10");
            return Integer(state);
        }

        return false;
    }

    private bool Integer(State state)
    {
        state.Append("integer");

        if (state.Match(LexemeType.Digit))
        {
            return UnsignedInteger(state);
        }

        if (state.Match(LexemeType.Plus))
        {
            state.Append("+");
            return UnsignedInteger(state);
        }

        if (state.Match(LexemeType.Dash))
        {
            state.Append("-");
            return UnsignedInteger(state);
        }

        return false;
    }

    private bool UnsignedInteger(State state)
    {
        var i = 0;
        for (; state.Match(LexemeType.Digit); i++)
        {
            state.Append("unsigned_integer");
            state.Append("digit");
        }

        return i > 0;
    }

    private static bool Branch(State state, Func<State, bool> func)
    {
        var branchState = new State(state);

        if (func(branchState))
        {
            state.Append(branchState);
            return true;
        }

        return false;
    }
}