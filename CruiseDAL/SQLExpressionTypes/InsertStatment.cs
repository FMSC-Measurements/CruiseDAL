using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.SQLExpressionTypes
{
    public class InsertStatment : SQLStatment
    {
        public string TableName { get; set; }
        public ICollection<String> ColumnList { get; set; }

        public string SelectStatment { get; set; }

        public ICollection<String> Values { get; set; }


    }
}
