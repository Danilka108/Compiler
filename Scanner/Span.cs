namespace Scanner;

public struct Span
{
    public int Start { get; private set; }

    public int End { get; private set; }

    internal bool IsEmpty()
    {
        return End <= Start;
    }

    internal bool IsNotEmpty()
    {
        return !IsEmpty();
    }

    internal void MoveEndPos()
    {
        End++;
    }

    internal void ResetStartPos()
    {
        Start = End;
    }

    internal void Reset()
    {
        Start = End = 0;
    }
}