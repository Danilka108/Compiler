using System;
using System.Linq;

namespace Compiler.Parsing.Lexing;

public class Caret(string content)
{
    private Span _span;

    public Eater StartEating()
    {
        return new Eater(content, _span);
    }

    public EatingResult FinishEating(Eater eater)
    {
        var result = new EatingResult
        {
            OldSpan = _span,
            NewSpan = eater.Span
        };

        _span = eater.Span.ShiftStartToEnd();
        return result;
    }

    public struct EatingResult
    {
        public Span OldSpan { get; init; }
        public Span NewSpan { get; init; }
    }

    public void Move()
    {
        _span = _span.ShiftEnd(1).ShiftStartToEnd();
    }

    public Span Span()
    {
        return _span;
    }

    public bool IsEnd()
    {
        return _span.End == content.Length;
    }

    public class Eater(string content, Span span)
    {
        public Span Span { get; private set; } = span.ShiftStartToEnd();

        // public string EatenContent()
        // {
        //     if (IsOutOfRange()) return "";
        //     return content[Span.Start..int.Min(Span.End, content.Length)];
        // }

        public bool Eat(char symbol)
        {
            if (GetCurrentSymbol() != symbol) return false;

            Span = Span.ShiftEnd(1);
            return true;
        }

        public bool Eat(Func<char, bool> predicate)
        {
            if (GetCurrentSymbol() is { } currentSymbol && predicate(currentSymbol))
            {
                Span = Span.ShiftEnd(1);
                return true;
            }

            return false;
        }

        public bool Eat(char symbol, char nextSymbol)
        {
            if (GetCurrentSymbol() != symbol || GetNextSymbol() != nextSymbol) return false;

            Span = Span.ShiftEnd(2);
            return true;
        }

        public bool Eat(string symbols)
        {
            return symbols.All(Eat);
        }

        public bool EatWhile(Func<char, bool> predicate)
        {
            var start = Span.End;
            while (GetCurrentSymbol() is { } currentSymbol && predicate(currentSymbol)) Span = Span.ShiftEnd(1);
            return Span.End - start > 0;
        }

        public bool EatWhile(Func<char, char?, bool> predicate)
        {
            var start = Span.End;
            while (GetCurrentSymbol() is { } currentSymbol &&
                   predicate(currentSymbol, GetNextSymbol())) Span = Span.ShiftEnd(1);
            return Span.End - start > 0;
        }

        private char? GetCurrentSymbol()
        {
            if (IsOutOfRange()) return null;
            return content[Span.End];
        }

        private char? GetNextSymbol()
        {
            if (IsOutOfRange(1)) return null;
            return content[Span.End + 1];
        }

        private bool IsOutOfRange()
        {
            return Span.End >= content.Length;
        }

        private bool IsOutOfRange(int shift)
        {
            return Span.End + shift >= content.Length;
        }
    }
}