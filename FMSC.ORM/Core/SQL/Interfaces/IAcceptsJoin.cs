using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsJoin : IAcceptsWhere
    {
        void Accept(JoinClause joinClause);
        IAcceptsJoin Join(TableOrSubQuery source, string constraint);
        IAcceptsJoin Join(string tableName, string constraint, string alias);
    }
}
