using System.Collections.Generic;

namespace CruiseDAL.Schema.Cruise.Lookup
{
    public class CruiseMethodSTableDefinition : ITableDefinition
    {
        public string TableName => "CruiseMethods";

        public string CreateTable =>
@"CREATE TABLE CruiseMethods (
    CruiseMethods_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Method TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL,
    IsPlotMethod BOOLEAN NOT NULL,
    UNIQUE (Method)
);";

        public string InitializeTable =>
@"INSERT INTO CruiseMethods (Method, FriendlyName, IsPlotMethod)
VALUES
    ('100', 'Classic 100%', 0),
    ('3P', 'Classic 3P' , 0),
    ('FIXCNT', 'Fixed Count', 1),
    ('3PPNT', '3P Point Biomass', 1),
    ('FIX', 'Fixed Plot' , 1),
    ('F3P', 'Fixed Plot with 3P subsample', 1),
    ('PNT', 'Point (Variable Plot)', 1),
    ('P3P', 'Point with 3P subsample', 1),
    ('STR', 'Sample Tree', 0),
    ('S3P', 'Sample Tree with 3P subsample', 0),
    --('PCMTRE', 'Point Count/Measure Tree  (Also coded as PCM)', 1),
    ('PCM', 'Point Count/Measure Tree', 1),
    ('FCM', 'Fixed Count/Measure', 1);";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}