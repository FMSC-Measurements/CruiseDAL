using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class JoinClause : SelectSource
    {
        public TableOrSubQuery Source { get; set; }

        public List<JoinComponent> Joins { get; set; }

        public override string SourceName
        {
            get
            {
                return this.Source.SourceName;
            }
        }

        public JoinClause(TableOrSubQuery source)
        {
            this.Joins = new List<JoinComponent>();
            Source = source;
        }

        public override JoinClause Join(String table, string constraint, string alias)
        {
            this.Joins.Add(new JoinComponent(table, constraint, alias));
            return this;
        }

        public override JoinClause Join(TableOrSubQuery source, string constraint)
        {
            this.Joins.Add(new JoinComponent(source, constraint));
            return this;
        }

        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            sBuilder.AppendLine(Source.ToSQL());
            foreach (JoinComponent comp in Joins)
            {
                sBuilder.AppendLine(" " + comp.ToSQL());
            }
            return sBuilder.ToString();
        }
    }
}