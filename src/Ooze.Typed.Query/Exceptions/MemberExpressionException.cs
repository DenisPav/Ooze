namespace Ooze.Typed.Query.Exceptions;

internal class MemberExpressionException : Exception
{
    public MemberExpressionException(string message) : base(message)
    { }
}

internal class ExpressionCreatorException : Exception
{
    public ExpressionCreatorException(string message) : base(message)
    { }
}

internal class QueryTokenizerException : Exception
{
    public QueryTokenizerException(string message) : base(message)
    { }
}