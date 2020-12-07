using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class LogFieldTableDefinition : ITableDefinition
    {
        public string TableName => "LogField";

        public string CreateTable =>
@"CREATE TABLE LogField (
    LogField_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Field TEXT NOT NULL COLLATE NOCASE,
    DbType TEXT NOT NULL COLLATE NOCASE,
    CHECK (length(Field) > 0),
    UNIQUE (Field)
);";

        public string InitializeTable =>
@"INSERT INTO LogField
    (Field, DbType)
VALUES
    ('Grade', 'TEXT'),
    ('ExportGrade','TEXT'),
    ('SeenDefect', 'REAL'),
    ('PercentRecoverable', 'REAL'),
    ('SmallEndDiameter', 'REAL'),
    ('LargeEndDiameter', 'REAL'),
    ('GrossBoardFoot', 'REAL'),
    ('NetBoardFoot', 'REAL'),
    ('GrossCubicFoot', 'REAL'),
    ('NetCubicFoot', 'REAL'),
    ('BoardFootRemoved', 'REAL'),
    ('CubicFootRemoved', 'REAL'),
    ('DIBClass', 'REAL'),
    ('BarkThickness', 'REAL')
;";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}