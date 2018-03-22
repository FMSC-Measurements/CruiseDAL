using SqlBuilder.Dialects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    public abstract class TableConstraint
    {
        public string ConstraintName { get; set; }
    }

    public class UniqueConstraint : TableConstraint
    {
        public IEnumerable<ColumnInfo> Columns { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }

        public string ConflictOption { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ConstraintName)) { sb.Append("CONSTRAINT ").Append(ConstraintName).Append(" "); }
            sb.Append("UNIQUE ( ");
            if (Columns != null)
            {
                bool first = true;
                foreach (var col in Columns)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col.Name);
                }
            }
            else if (ColumnNames != null)
            {
                bool first = true;
                foreach (var col in ColumnNames)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col);
                }
            }
            else
            { throw new InvalidOperationException("either Columns or ColumnNames must be set"); }
            sb.Append(")");

            if (!string.IsNullOrEmpty(ConflictOption))
            {
                sb.Append(" ON CONFLICT ").Append(ConflictOption);
            }

            return sb.ToString();
        }
    }

    public class PrimaryKeyConstraint : TableConstraint
    {
        public IEnumerable<ColumnInfo> Columns { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }

        public string ConflictOption { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ConstraintName)) { sb.Append("CONSTRAINT ").Append(ConstraintName).Append(" "); }
            sb.Append("PRIMARY KEY ( ");
            if (Columns != null)
            {
                bool first = true;
                foreach (var col in Columns)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col.Name);
                }
            }
            else if (ColumnNames != null)
            {
                bool first = true;
                foreach (var col in ColumnNames)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col);
                }
            }
            else
            { throw new InvalidOperationException("either Columns or ColumnNames must be set"); }
            sb.Append(")");

            if (!string.IsNullOrEmpty(ConflictOption))
            {
                sb.Append(" ON CONFLICT ").Append(ConflictOption);
            }

            return sb.ToString();
        }
    }

    public class CheckConstraint : TableConstraint
    {
        public string Expression { get; set; }

        public CheckConstraint(string expression)
        { Expression = expression; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("CHECK (").Append(Expression).Append(")");
            return sb.ToString();
        }
    }

    public class ForeignKeyConstraint : TableConstraint
    {
        public IEnumerable<ColumnInfo> Columns { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }

        public string ForeignKeyClause { get; set; }

        //public string References { get; set; }

        //public IEnumerable<ColumnInfo> ReferenceColumns { get; set; }
        //public IEnumerable<string> ReferenceColumnNames { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ConstraintName)) { sb.Append("CONSTRAINT ").Append(ConstraintName).Append(" "); }
            sb.Append("FOREIGN KEY ( ");
            if (Columns != null)
            {
                bool first = true;
                foreach (var col in Columns)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col.Name);
                }
            }
            else if (ColumnNames != null)
            {
                bool first = true;
                foreach (var col in ColumnNames)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(col);
                }
            }
            else
            { throw new InvalidOperationException("either Columns or ColumnNames must be set"); }
            sb.Append(")");

            sb.Append(ForeignKeyClause);

            return sb.ToString();
        }
    }

    public class CreateTable
    {
        public static ISqlDialect DefaultDialect { get; set; }
        public ISqlDialect Dialect { get; set; }

        public bool Temp { get; set; }
        public bool IfNotExists { get; set; }

        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public IEnumerable<ColumnInfo> Columns { get; set; }

        public IEnumerable<string> TableConstraints { get; set; }

        public bool WithoutRowid { get; set; }

        public SqlSelectBuilder SelectStatment { get; set; }

        public CreateTable()
        {
        }

        public CreateTable(ISqlDialect dialect)
        {
            Dialect = dialect;
        }

        public override string ToString()
        {
            return ToString(Dialect ?? SqlDialect.DefaultDialect);
        }

        public string ToString(ISqlDialect dialect)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE");
            if (Temp) { sb.Append(" TEMP"); }
            sb.Append(" TABLE ");
            if (IfNotExists) { sb.Append("IF NOT EXISTS "); }
            if (!string.IsNullOrEmpty(SchemaName)) { sb.Append(SchemaName).Append("."); }
            sb.Append(TableName);

            if (SelectStatment != null)
            {
                sb.Append(" AS ").Append(SelectStatment.ToString());
            }
            else
            {
                sb.Append(" (");
                bool first = true;
                foreach (var col in Columns)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(dialect.GetColumnDefinition(col));
                }

                if (TableConstraints != null)
                {
                    foreach (var constr in TableConstraints)
                    {
                        if (!first) { sb.Append(", "); }
                        else { first = false; }
                        sb.Append(constr);
                    }
                }

                sb.Append(")");

                if (WithoutRowid) { sb.Append(" WITHOUT ROWID"); }
            }

            return sb.ToString();
        }
    }
}