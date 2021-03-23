using System.Collections.Generic;

namespace CruiseDAL.Schema.Cruise.Lookup
{
    public class LK_Forest : ITableDefinition
    {
        public string TableName => "LK_Forest";

        public string CreateTable =>
@"CREATE TABLE LK_Forest (
    LK_Forest_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Forest TEXT NOT NULL COLLATE NOCASE, -- needs to be text because one blm forest has non numeric forest number
    Region TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL COLLATE NOCASE,
    

    FOREIGN KEY (Region) REFERENCES LK_Region (Region) ON DELETE CASCADE,

    UNIQUE (Forest, Region)
);";

        public string InitializeTable =>
@"INSERT INTO LK_Forest (Forest, FriendlyName, Region)
VALUES
    ('02', 'Beaverhead-Deerlodge', '01'),
    ('03', 'Bitterroot', '01'),
    ('04', 'Idaho Panhandle', '01'),
    ('10', 'Flathead', '01'),
    ('11', 'Custer Gallatin', '01'),
    ('14', 'Kootenai', '01'),
    ('15', 'Helena-Lewis & Clark', '01'),
    ('16', 'Lolo', '01'),
    ('17', 'Nez Perce-Clearwater', '01'),
    ('02', 'Bighorn', '02'),
    ('03', 'Black Hills', '02'),
    ('04', 'Grand Mesa-Uncompahgre-Gunnison', '02'),
    ('06', 'Medicine Bow-Routt', '02'),
    ('07', 'Nebraska', '02'),
    ('09', 'Rio Grande', '02'),
    ('10', 'Arapaho-Roosevelt', '02'),
    ('12', 'Pike & San Isabel Nfs & Cimarron-Comanche Grasslands', '02'),
    ('13', 'San Juan', '02'),
    ('14', 'Shoshone', '02'),
    ('15', 'White River', '02'),
    ('01', 'Apache-Sitgreaves', '03'),
    ('02', 'Carson', '03'),
    ('03', 'Cibola', '03'),
    ('04', 'Coconino', '03'),
    ('05', 'Coronado', '03'),
    ('06', 'Gila', '03'),
    ('07', 'Kaibab', '03'),
    ('08', 'Lincoln', '03'),
    ('09', 'Prescott', '03'),
    ('10', 'Santa Fe', '03'),
    ('12', 'Tonto', '03'),
    ('01', 'Ashley', '04'),
    ('02', 'Boise', '04'),
    ('03', 'Bridger-Teton', '04'),
    ('07', 'Dixie', '04'),
    ('08', 'Fishlake', '04'),
    ('10', 'Manti-La Sal', '04'),
    ('12', 'Payette', '04'),
    ('13', 'Salmon-Challis', '04'),
    ('14', 'Sawtooth', '04'),
    ('15', 'Caribou-Targhee', '04'),
    ('17', 'Humboldt-Toiyabe', '04'),
    ('19', 'Uinta-Wasatch-Cache', '04'),
    ('02', 'Cleveland', '05'),
    ('03', 'Eldorado', '05'),
    ('04', 'Inyo', '05'),
    ('05', 'Klamath', '05'),
    ('06', 'Lassen', '05'),
    ('07', 'Los Padres', '05'),
    ('08', 'Mendocino', '05'),
    ('09', 'Modoc', '05'),
    ('10', 'Six Rivers', '05'),
    ('11', 'Plumas', '05'),
    ('12', 'San Bernardino', '05'),
    ('13', 'Sequoia', '05'),
    ('14', 'Shasta-Trinity', '05'),
    ('15', 'Sierra', '05'),
    ('16', 'Stanislaus', '05'),
    ('17', 'Tahoe', '05'),
    ('19', 'Lake Tahoe Basin Management Unit', '05'),
    ('01', 'Deschutes', '06'),
    ('02', 'Fremont-Winema', '06'),
    ('03', 'Gifford Pinchot', '06'),
    ('04', 'Malheur', '06'),
    ('05', 'Mt. Baker-Snoqualmie', '06'),
    ('06', 'Mt. Hood', '06'),
    ('07', 'Ochoco', '06'),
    ('09', 'Olympic', '06'),
    ('10', 'Rogue-Siskiyou', '06'),
    ('12', 'Siuslaw', '06'),
    ('14', 'Umatilla', '06'),
    ('15', 'Umpqua', '06'),
    ('16', 'Wallowa-Whitman', '06'),
    ('17', 'Okanogan-Wenatchee', '06'),
    ('18', 'Willamette', '06'),
    ('21', 'Colville', '06'),
    ('22', 'Columbia River Gorge NSA', '06'),
    ('01', 'National Forests In Alabama', '08'),
    ('02', 'Daniel Boone', '08'),
    ('03', 'Chattahoochee-Oconee', '08'),
    ('04', 'Cherokee', '08'),
    ('05', 'National Forests In Florida', '08'),
    ('06', 'Kisatchie', '08'),
    ('07', 'National Forests In Mississippi', '08'),
    ('08', 'George Washington-Jefferson', '08'),
    ('09', 'Ouachita', '08'),
    ('10', 'Ozark-St. Francis', '08'),
    ('11', 'National Forests In North Carolina', '08'),
    ('12', 'Francis Marion-Sumter', '08'),
    ('13', 'National Forests In Texas', '08'),
    ('03', 'Chippewa', '09'),
    ('04', 'Huron Manistee', '09'),
    ('05', 'Mark Twain', '09'),
    ('07', 'Ottawa', '09'),
    ('08', 'Shawnee', '09'),
    ('09', 'Superior', '09'),
    ('10', 'Hiawatha', '09'),
    ('12', 'Hoosier', '09'),
    ('13', 'Chequamegon-Nicolet', '09'),
    ('14', 'Wayne', '09'),
    ('19', 'Allegheny', '09'),
    ('20', 'Green Mountain & Finger Lakes', '09'),
    ('21', 'Monongahela', '09'),
    ('22', 'White Mountain', '09'),
    ('04', 'Chugach', '10'),
    ('05', 'Tongass', '10'),

    ('00', 'Unknown BLM Forest', '07'),
    ('00', 'Unknown DOD Forest', '11'),
    ('JBLM', 'JBLM', '11')
;";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}