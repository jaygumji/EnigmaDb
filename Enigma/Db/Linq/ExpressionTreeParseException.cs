using System;

namespace Enigma.Db.Linq
{
    /// <summary>
    /// Exception that will be thrown when unexpected nodes appear in the expression tree
    /// </summary>
    public class ExpressionTreeParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeParseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ExpressionTreeParseException(string message) : base(message) { }

        /// <summary>
        /// Invalid expression count.
        /// </summary>
        /// <returns></returns>
        public static ExpressionTreeParseException InvalidExpressionCount(string operation)
        {
            return new ExpressionTreeParseException(operation + " operation invoked, but only 1 expression has been parsed");
        }
    }
}