using System;


namespace CruiseDAL
{
    public class ColumnInfo
    {
        public String Name { get; internal set; }
        public String DBType { get; internal set; }
        public bool IsPK { get; internal set; }
    }

}