namespace Scanner;

public delegate ScanResult<TTokenType, TError> TokenScanner<TTokenType, TError>(Caret caret);

public class Scanner<TTokenType, TError> where TTokenType : Enum
{
    private Caret _caret;
    private readonly IEnumerable<TokenScanner<TTokenType, TError>> _tokenScanners;
    private readonly List<Token<TTokenType, TError>> _tokens;

    public Scanner(string content, IEnumerable<TokenScanner<TTokenType, TError>> tokenScanners)
    {
        _caret = new Caret(content);
        _tokenScanners = tokenScanners;
        _tokens = new List<Token<TTokenType, TError>>();
    }

    public IEnumerable<Token<TTokenType, TError>> Scan()
    {
        _tokens.Clear();
        _caret.Reset();

        for (; !_caret.IsEnd();) IterOverTokenScanners();
        HandleUnexpectedSymbols();

        return _tokens;
    }

    private void IterOverTokenScanners()
    {
        foreach (var tokenScanner in _tokenScanners)
        {
            var tempCaret = TempCaret();

            var token = tokenScanner(tempCaret).ToToken(tempCaret.Span());
            if (token == null) continue;

            HandleUnexpectedSymbols();
            _tokens.Add(token);

            UpdateCaret(tempCaret);
            return;
        }

        _caret.Eat();
    }

    private Caret TempCaret()
    {
        var tempCaret = _caret.Clone();
        tempCaret.ResetStartPos();

        return tempCaret;
    }

    private void UpdateCaret(Caret caret)
    {
        _caret = caret;
        _caret.ResetStartPos();
    }

    private void HandleUnexpectedSymbols()
    {
        if (_caret.Span().IsNotEmpty())
            _tokens.Add(new Token<TTokenType, TError>.InvalidToken(default, _caret.Span()));
    }
}