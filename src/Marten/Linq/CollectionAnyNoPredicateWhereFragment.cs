using System.Linq;
using System.Reflection;
using Baseline;
using Marten.Util;
using Remotion.Linq.Clauses.Expressions;

namespace Marten.Linq
{
    /// <summary>
    /// Handle Any() with JSONB_ARRAY_LENGTH introduced in PostgreSQL 9.4
    /// </summary>
    public class CollectionAnyNoPredicateWhereFragment : IWhereFragment
    {
        private readonly MemberInfo[] _members;
        private readonly SubQueryExpression _expression;

        public CollectionAnyNoPredicateWhereFragment(MemberInfo[] members, SubQueryExpression expression)
        {
            _members = members;
            _expression = expression;
        }

        public void Apply(CommandBuilder builder)
        {

            //var query = $"JSONB_ARRAY_LENGTH(COALESCE(case when data->>'{path}' is not null then data->'{path}' else '[]' end)) > 0";
             



            builder.Append("JSONB_ARRAY_LENGTH(COALESCE(case when data->>'");

            builder.Append(_members[0].Name);
            for (int i = 1; i < _members.Length; i++)
            {
                builder.Append("'->'");
                builder.Append(_members[i].Name);
            }

            builder.Append("' is not null then data->'");

            builder.Append(_members[0].Name);
            for (int i = 1; i < _members.Length; i++)
            {
                builder.Append("'->'");
                builder.Append(_members[i].Name);
            }


            builder.Append("' else '[]' end)) > 0");
        }
     
        public bool Contains(string sqlText)
        {
            return false;
        }
    }
}