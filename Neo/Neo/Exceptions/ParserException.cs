using System;
using Neo.Services;

namespace Neo.Exceptions;

internal class ParserException : Exception
{
    public object IncorrectSymbol { get; }

    public ParserException(string message, object incorrectSymbol) : base(message)
    {
        IncorrectSymbol = incorrectSymbol;
    }

    public override string ToString()
    {
        return $"Found incorrect symbol while string was parsing: {{{IncorrectSymbol}}}";
    }
}