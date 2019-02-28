using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_MESSAGELOG =
            "CREATE TABLE MessageLog( " +
                "Message_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "MessageLogID TEXT NOT NULL, " +
                "Program TEXT COLLATE NOCASE, " +
                "Message TEXT, " +
                "Date TEXT DEFAULT (date('now', 'localtime')), " +
                "Time TEXT DEFAULT (time('now', 'localtime')), " +
                "Level TEXT COLLATE NOCASE DEFAULT 'N', " +
                "UNIQUE (MessageLogID)" +
            ");";
    }
}
