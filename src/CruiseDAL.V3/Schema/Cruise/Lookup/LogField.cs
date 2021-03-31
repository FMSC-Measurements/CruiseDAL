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
    DefaultHeading TEXT NOT NULL COLLATE NOCASE,
    DbType TEXT NOT NULL COLLATE NOCASE,
    CHECK (length(Field) > 0),
    UNIQUE (Field)
);";

        public string InitializeTable =>
@"INSERT INTO LogField
    (Field, DefaultHeading, DbType)
VALUES
    ('Grade', 'Grade', 'TEXT'),
    ('ExportGrade', 'Export Grade','TEXT'),
    ('SeenDefect', 'Seen Def', 'REAL'),
    ('PercentRecoverable', 'Pct Reco', 'REAL'),
    ('SmallEndDiameter', 'SED', 'REAL'),
    ('LargeEndDiameter', 'LED', 'REAL'),
    ('GrossBoardFoot', 'GrossBF', 'REAL'),
    ('NetBoardFoot', 'NetBF', 'REAL'),
    ('GrossCubicFoot', 'GrossCF', 'REAL'),
    ('NetCubicFoot', 'NetCF', 'REAL'),
    ('BoardFootRemoved', 'BFRemoved', 'REAL'),
    ('CubicFootRemoved', 'CFRemoved', 'REAL'),
    ('DIBClass', 'DIBClass', 'REAL'),
    ('BarkThickness', 'BarkThickness', 'REAL')
;";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}