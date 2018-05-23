using System.Collections.Generic;

namespace SqlBuilder
{
    public static class SelectExtentions
    {
        public static IAcceptsJoin Join(this IAcceptsJoin @this, TableOrSubQuery source, string constraint)
        {
            @this.Accept(new JoinClause(source, constraint));
            return @this;
        }

        public static IAcceptsJoin Join(this IAcceptsJoin @this, string tableName, string constraint)
        {
            @this.Accept(new JoinClause(new TableOrSubQuery(tableName), constraint));
            return @this;
        }

        public static IAcceptsJoin Join(this IAcceptsJoin @this, string tableName, string constraint, string alias)
        {
            @this.Accept(new JoinClause(tableName, constraint, alias));
            return @this;
        }

        public static IAcceptsJoin LeftJoin(this IAcceptsJoin @this, TableOrSubQuery source, string constraint)
        {
            @this.Accept(new JoinClause(source, constraint) { JoinType = "LEFT" });
            return @this;
        }

        public static IAcceptsJoin LeftJoin(this IAcceptsJoin @this, string tableName, string constraint)
        {
            @this.Accept(new JoinClause(new TableOrSubQuery(tableName), constraint) { JoinType = "LEFT" });
            return @this;
        }

        public static IAcceptsJoin LeftJoin(this IAcceptsJoin @this, string tableName, string constraint, string alias)
        {
            @this.Accept(new JoinClause(tableName, constraint, alias) { JoinType = "LEFT" });
            return @this;
        }

        public static IAcceptsGroupBy Where(this IAcceptsWhere @this, string expression)
        {
            @this.Accept(new WhereClause(expression));
            return @this;
        }

        public static IAcceptsOrderBy GroupBy(this IAcceptsGroupBy @this, IEnumerable<string> terms)
        {
            @this.Accept(new GroupByClause(terms));
            return @this;
        }

        public static IAcceptsOrderBy GroupBy(this IAcceptsGroupBy @this, params string[] termArgs)
        {
            @this.Accept(new GroupByClause(termArgs));
            return @this;
        }

        public static IAcceptsLimit OrderBy(this IAcceptsOrderBy @this, IEnumerable<string> terms)
        {
            @this.Accept(new OrderByClause(terms));
            return @this;
        }

        public static IAcceptsLimit OrderBy(this IAcceptsOrderBy @this, params string[] termArgs)
        {
            @this.Accept(new OrderByClause(termArgs));
            return @this;
        }

        public static void Limit(this IAcceptsLimit @this, int limit, int offset)
        {
            @this.Accept(new LimitClause(limit, offset));
            //return @this;
        }
    }
}