using System;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectSource //: SelectElement
    {
        //public SelectElement ParentElement { get; set; }

        //public void Accept(SelectElement parent)
        //{
        //    this.ParentElement = parent;
        //}

        public abstract String SourceName { get; }

        public JoinClause Join(string source, string constraint)
        {
            return this.Join(source, constraint, null);
        }

        public abstract JoinClause Join(TableOrSubQuery source, string constraint);

        public abstract JoinClause Join(string table, string constraint, string alias);

        public abstract String ToSQL();

        public override string ToString()
        {
            return ToSQL();
        }
    }
}