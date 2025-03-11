namespace Ooze.Typed.Query.Exceptions;

internal class MemberExpressionException(string message) : Exception(message);

internal class ExpressionCreatorException(string message) : Exception(message);

internal class QueryTokenizerException(string message) : Exception(message);