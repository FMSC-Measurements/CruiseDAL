using System;


namespace FMSC.ORM.Core.SQL
{
    public class ColumnInfo
    {
        public String Name { get; set; }
        public String DBType { get; set; }
        public bool IsPK { get; set; }
        public bool IsRequired { get; set; }
        public String Default { get; set; }


        public string GetColumnDef(bool incluedConstraint)
        {
            string columnConstraint = string.Empty;
            if (incluedConstraint)
            {
                columnConstraint += (IsPK) ? "PRIMARY KEY " + ((DBType == "INTEGER") ? "AUTOINCREMENT " : string.Empty) : string.Empty;
                columnConstraint += (IsRequired) ? "NOT NULL " : string.Empty;
                columnConstraint += (Default != null) ? "DEFAULT " + Default : string.Empty;
            }

            return Name + " " + DBType + " " + columnConstraint;
        }

    }

}