using System;
using System.Data;
using System.Text;

namespace SqlBuilder
{
    public class ColumnInfo
    {
        public String Name { get; set; }
        public DbType DBType { get; set; }
        public string Type { get; set; }
        public bool IsPK { get; set; }
        public bool NotNull { get; set; }
        public bool AutoIncrement { get; set; }
        public String Default { get; set; }
        public bool Unique { get; set; }
        public string Check { get; set; }

        public ColumnInfo()
        { }

        public ColumnInfo(string name) : this(name, DbType.AnsiString)
        { }

        public ColumnInfo(string name, string type) :this()
        {
            Name = name;
            Type = type;
        }

        public ColumnInfo(string name, DbType dbType) : this()
        {
            Name = name;
            DBType = dbType;
        }

        public override string ToString()
        {
            return new StringBuilder("Column Info: ").Append(Name).Append(" ").Append(DBType).ToString();
        }
    }
}