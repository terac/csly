using System.Text;

namespace sly.parser.syntax
{

    public class ZeroOrMoreClause<T> : IClause<T>
    {
        public IClause<T> Clause { get; set; }
        public ZeroOrMoreClause(IClause<T> clause)
        {
            Clause = clause;
        }

        public override string ToString()
        {
            return Clause.ToString() + "*";
        }

        public bool MayBeEmpty()
        {
            return true;
        }

      
    }
}