using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.SQLExpressionTypes
{
    public enum TriggerSequencing { Before, After, InsteadOf }

    public enum TriggerEvent { OnDelete, OnInsert, OnUpdate }

    public class CreateTriggerStatment : SQLDdlStatment
    {
        public string TableName { get; set; }

        public bool IsTemp { get; set; }

        public bool IfNotExists { get; set; }

        public TriggerSequencing TriggerSequencing { get; set; }

        public TriggerEvent TriggerEvent { get; set; }

        public bool ForEachRow { get; set; }

        public string WhenClause { get; set; }

        public ICollection<SQLStatment> TriggerStatments { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
