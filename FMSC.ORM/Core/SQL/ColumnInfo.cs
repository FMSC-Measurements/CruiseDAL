using System;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class ColumnInfo
    {
        public String Name { get; set; }
        public String DBType { get; set; }
        public bool IsPK { get; set; }
        public bool IsRequired { get; set; }
        public bool AutoIncrement { get; set; }
        public String Default { get; set; }
        public bool Unique { get; set; }
        public string Check { get; set; }

        public ColumnInfo()
        { }

        public ColumnInfo(string name) : this(name, "TEXT")
        { }

        public ColumnInfo(string name, string dbType)
        {
            Name = name;
            DBType = dbType;
        }

        //[Obsolete]
        //public string GetColumnDef(bool includeDef) { throw new NotSupportedException(); }

        public override string ToString()
        {
            return new StringBuilder("Column Info: ").Append(Name).Append(" ").Append(DBType).ToString();
        }
    }

    //    public class ColumnInfo
    //    {
    //        public String Name { get; set; }
    //        public String DBType { get; set; }
    //        public bool IsPK { get; set; }
    //        public bool IsRequired { get; set; }
    //        public String Default { get; set; }

    //        public ColumnInfo()
    //        { }

    //#if !NetCF
    //        public ColumnInfo(string name, string dbType = "TEXT", bool isPk = false, bool isRequired = false, string defaultVal = null)
    //#else
    //        public ColumnInfo(string name, string dbType, bool isPk, bool isRequired, string defaultVal)
    //#endif
    //        {
    //            Name = name;
    //            DBType = dbType;
    //            IsPK = isPk;
    //            IsRequired = isRequired;
    //            Default = defaultVal;
    //        }

    //        public string GetColumnDef(bool incluedConstraint)
    //        {
    //            string columnConstraint = string.Empty;
    //            if (incluedConstraint)
    //            {
    //                columnConstraint += (IsPK) ? "PRIMARY KEY " + ((DBType == "INTEGER") ? "AUTOINCREMENT " : string.Empty) : string.Empty;
    //                columnConstraint += (IsRequired) ? "NOT NULL " : string.Empty;
    //                columnConstraint += (Default != null) ? "DEFAULT " + Default : string.Empty;
    //            }

    //            return Name + " " + DBType + " " + columnConstraint;
    //        }

    //        public override string ToString()
    //        {
    //            return GetColumnDef(true);
    //        }

    //    }
}