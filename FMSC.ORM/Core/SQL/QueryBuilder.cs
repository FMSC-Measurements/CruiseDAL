using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public delegate IEnumerable<T> DatastoreMethod<out T>(SQLSelectBuilder builder);

    public abstract class QueryBuilder
    {
        protected SQLSelectBuilder builder;
    }

    public abstract class QueryBuilder<T> : QueryBuilder
    {
        public DatastoreMethod<T> datastoreMethod; 

        public IEnumerable<T> Select()
        {
            return datastoreMethod(builder);
            throw new NotImplementedException();
        }
    }

    public abstract class LimitQueryBuilder<T> : QueryBuilder<T>
    {
        public QueryBuilder<T> Limit(int limit, int offset)
        {
            builder.Limit(limit, offset);
            return this;
        }
    }

    public abstract class GroupByQueryBuilder<T>: LimitQueryBuilder<T>
    {
        public LimitQueryBuilder<T> GroupBy(IEnumerable<string> terms)
        {
            builder.GroupBy(terms);
            return this;
        }
    }

    public class WhereQueryBuilder<T>: GroupByQueryBuilder<T>
    {
        public GroupByQueryBuilder<T> Where(string expression)
        {
            base.builder.Where(expression);
            return this;
        }
    }

    //public class QueryBuilder<T, S> : SelectElement 
    //    where S : SelectElement
    //{
    //}

    //public static class QueryBuilderExtentions
    //{
    //    public static GroupByQueryBuilder<T> Where<T>(this WhereQueryBuilder<T> qBilder, string expression)
    //    {
    //        var whereClause = new WhereClause(expression);
    //        return qBilder;
    //    }

    //    public static LimitQueryBuilder<T> GroupBy<T>(this GroupByQueryBuilder<T> qBuilder, IEnumerable<string> terms)
    //    {
    //        var groupByClause = new GroupByClause(groupByExprs);
    //        return qBuilder;
    //    }

    //    public static QueryBuilder<T> Limit<T>(this LimitQueryBuilder<T> qBuilder, int limit, int offset)
    //    {
    //        var limitClause = new LimitClause(limit, offset);
    //        return qBuilder;
    //    }

    //}
}
