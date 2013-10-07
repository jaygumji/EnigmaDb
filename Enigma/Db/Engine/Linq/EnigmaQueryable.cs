using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Enigma.Modelling;

namespace Enigma.Db.Linq
{
    public class EnigmaQueryable<T> : QueryableBase<T>
    {

        public EnigmaQueryable(IEnigmaEngine engine, Model model) : base(QueryParser.CreateDefault(), new EnigmaQueryExecutor(engine, model))
        {
        }

        public EnigmaQueryable(IQueryProvider provider, Expression expression) : base(provider, expression)
        {
        }

    }
}
