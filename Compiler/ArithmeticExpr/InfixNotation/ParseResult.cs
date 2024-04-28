using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.ArithmeticExpr.InfixNotation;

public class ParseResult
{
    private Expr? _expr = null;

    private ParseError[] _errors = [];

    private bool IsOk => _errors.Length == 0;

    private bool HasErrors => _errors.Length > 0;

    public Expr? Expr => HasErrors ? null : _expr;

    public IEnumerable<ParseError> Errors => _errors;

    public ParseResult(ParseError parseError)
    {
        _expr = default;
        _errors = [parseError];
    }

    public ParseResult(Expr expr)
    {
        _expr = expr;
        _errors = [];
    }

    internal ParseResult AddError(ParseError parseError)
    {
        _errors = _errors.Append(parseError).ToArray();
        return this;
    }

    internal ParseResult ConsumeErrors(ParseResult other)
    {
        var newErrors = new List<ParseError>(_errors);
        newErrors.AddRange(other._errors);
        _errors = newErrors.ToArray();

        return this;
    }

    internal ParseResult FlatMap(Func<Expr?, ParseResult> f)
    {
        var result = f(IsOk ? _expr : null);

        _expr = result._expr;
        ConsumeErrors(result);

        return this;
    }

    internal ParseResult Combine(ParseResult other, Func<Expr, Expr, Expr> f)
    {
        ConsumeErrors(other);

        if (IsOk && _expr is { } expr && other is { IsOk: true, _expr: { } otherExpr })
        {
            _expr = f(expr, otherExpr);
        }

        return this;
    }
}