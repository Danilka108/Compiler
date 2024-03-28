using System;

namespace Compiler.parser;

public struct MovableSpan
{
    public int Start { get; private set; }

    public int End { get; private set; }

    public MovableSpan() : this(0, 0)
    {
    }

    public MovableSpan(int start, int end)
    {
        if (start > end) throw new ArgumentException("'start' must be less than or equal to the 'end'");

        Start = start;
        End = end;
    }

    public bool IsEmpty()
    {
        return End <= Start;
    }

    public bool IsNotEmpty()
    {
        return !IsEmpty();
    }

    public void ShiftStartToEnd()
    {
        Start = End;
    }

    public void ShiftEndToStart()
    {
        End = Start;
    }

    public void Reset()
    {
        Start = End = 0;
    }
}