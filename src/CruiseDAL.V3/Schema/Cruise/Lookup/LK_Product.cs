using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LK_Product : ITableDefinition
    {
        public string TableName => "LK_Product";

        public string CreateTable =>
@"CREATE TABLE LK_Product (
    LK_Product_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Product TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL COLLATE NOCASE,
    UNIQUE (Product)
);";

        public string InitializeTable =>
@"INSERT INTO LK_Product (Product, FriendlyName)
VALUES
    ('01', 'Sawtimber'),
    ('02', 'Pulpwood'),
    ('03', 'Poles'),
    ('04', 'Pilings'),
    ('05', 'Mine Props'),
    ('06', 'Posts'),
    ('07', 'Fuelwood'),
    ('08', 'Non-sawtimber'),
    ('09', 'Ties'),
    ('10', 'Coop Bolts'),
    ('11', 'Acid/Dist.'),
    ('12', 'Float Logs'),
    ('13', 'Trap Float'),
    ('14', 'Misc-Conv.'),
    ('15', 'Christmas Trees'),
    ('16', 'Nav Stores'),
    ('17', 'Non Conv.'),
    ('18', 'Cull Logs'),
    ('19', 'Sm Rnd Wd'),
    ('20', 'Grn Bio Cv'),
    ('21', 'Dry Bio Cv'),
    ('26', 'Sp Wood Pr');";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}