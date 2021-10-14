using System.Collections.Generic;

namespace CruiseDAL.Schema.Cruise.Lookup
{
    public class LK_LoggingMethod : ITableDefinition
    {
        public string TableName => "LK_LoggingMethod";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    LK_LoggingMethod_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    LoggingMethod TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL,
    UNIQUE (LoggingMethod)
);";
        }

        public string InitializeTable =>
@"INSERT INTO LK_LoggingMethod (LoggingMethod, FriendlyName)
VALUES
    ('401', 'Manual logging'),
    ('410', 'Animal'),
    ('420', 'Tractor'),
    ('421', 'Rubber tired skidder '),
    ('422', 'Hard track (track driven from rear)'),
    ('423', 'Soft track (track driven from front)'),
    ('430', 'Single span skyline'),
    ('431', 'Single span gravity outhaul lt 1300 ft.'),
    ('432', 'Single span haulback outhaul lt 1300 ft.'),
    ('433', 'Single span gravity outhaul lt 1800 ft.'),
    ('434', 'Single span haulback outhaul lt 1800 ft.'),
    ('435', 'Single span skyline gt 1800 ft.'),
    ('440', 'Multispan'),
    ('441', 'Multispan - uphill'),
    ('442', 'Multispan - downhill'),
    ('450', 'Highlead'),
    ('451', 'Grabinski'),
    ('452', 'Track loader/jammer'),
    ('453', 'Loader logging'),
    ('454', 'Other logging'),
    ('460', 'Colddeck and swing'),
    ('461', 'Highlead colddeck/skyline swing'),
    ('462', 'Highlead colddeck/highlead swing'),
    ('464', 'Other colddeck'),
    ('470', 'Balloon'),
    ('480', 'Helicopter'),
    ('481', 'Helicopter - small'),
    ('482', 'Helicopter - medium'),
    ('483', 'Helicopter - large'),
    ('490', 'Utilizer - chip'),
    ('491', 'Mechanized systems (felling/bucking/delimbing)'),
    ('492', 'Cut to length');";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}