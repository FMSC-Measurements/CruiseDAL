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
    State TEXT NOT NULL COLLATE NOCASE,
    

    FOREIGN KEY (Region) REFERENCES LK_Region (Region) ON DELETE CASCADE,

    UNIQUE (Forest, Region)
);";

        public string InitializeTable =>
@"INSERT INTO LK_Forest (Forest, FriendlyName, State, Region)
VALUES
    ('02', 'Beaverhead-Deerlodge', 'MT', '01'),
    ('03', 'Bitterroot', 'MT', '01'),
    ('04', 'Idaho Panhandle', 'ID', '01'),
    ('05', 'Clearwater', 'ID', '01'),
    ('08', 'Custer', 'MT', '01'),
    ('10', 'Flathead', 'MT', '01'),
    ('11', 'Gallatin', 'MT', '01'),
    ('12', 'Helena', 'MT', '01'),
    ('14', 'Kootenai', 'MT', '01'),
    ('15', 'Lewis & Clark', 'MT', '01'),
    ('16', 'Lolo', 'MT', '01'),
    ('17', 'Nezperce', 'ID', '01'),
    ('02', 'Bighorn', 'WY', '02'),
    ('03', 'BlackHills', 'SD', '02'),
    ('04', 'GMUG', 'CO', '02'),
    ('06', 'Medicine Bow-Routt', 'WY', '02'),
    ('07', 'Nebraska', 'CO', '02'),
    ('09', 'Rio Grande', 'CO', '02'),
    ('10', 'Arapaho-Roosevelt', 'CO', '02'),
    ('12', 'Pike-San Isabel', 'CO', '02'),
    ('13', 'San Juan', 'CO', '02'),
    ('14', 'Shoshone', 'WY', '02'),
    ('15', 'White River', 'CO', '02'),
    ('01', 'Apache-Sitgreaves', 'AZ', '03'),
    ('02', 'Carson', 'NM', '03'),
    ('03', 'Cibola', 'NM', '03'),
    ('04', 'Coconino', 'AZ', '03'),
    ('05', 'Coronado', 'AZ', '03'),
    ('06', 'Gila', 'NM', '03'),
    ('07', 'Kaibab', 'AZ', '03'),
    ('08', 'Lincoln', 'NM', '03'),
    ('09', 'Prescott', 'AZ', '03'),
    ('10', 'Santa Fe ', 'NM', '03'),
    ('12', 'Tonto', 'AZ', '03'),
    ('01', 'Ashley', 'UT', '04'),
    ('02', 'Boise', 'ID', '04'),
    ('03', 'Bridger-Teton', 'WY', '04'),
    ('07', 'Dixie', 'UT', '04'),
    ('08', 'Fishlake', 'UT', '04'),
    ('10', 'Manti-LaSal', 'UT', '04'),
    ('12', 'Payette', 'ID', '04'),
    ('13', 'Challis', 'ID', '04'),
    --('13', 'Salmon', 'ID', '04'),
    ('14', 'Sawtooth', 'ID', '04'),
    ('15', 'Caribou-Targhee', 'ID', '04'),
    ('17', 'Humboldt-Toiyabe', 'NV', '04'),
    ('19', 'Uinta-Wasatch-Cache', 'UT', '04'),
    ('01', 'Angeles', 'CA', '05'),
    ('02', 'Cleveland', 'CA', '05'),
    ('03', 'Eldorado', 'CA', '05'),
    ('04', 'Inyo', 'CA', '05'),
    ('05', 'Klamath', 'CA', '05'),
    ('06', 'Lassen', 'CA', '05'),
    ('07', 'Los Padres', 'CA', '05'),
    ('08', 'Mendocino', 'CA', '05'),
    ('09', 'Modoc', 'CA', '05'),
    ('10', 'Six Rivers', 'CA', '05'),
    ('11', 'Plumas', 'CA', '05'),
    ('12', 'San Bernardino', 'CA', '05'),
    ('13', 'Sequoia', 'CA', '05'),
    ('14', 'Shasta-Trinity', 'CA', '05'),
    ('15', 'Sierra', 'CA', '05'),
    ('16', 'Stanislaus', 'CA', '05'),
    ('17', 'Tahoe', 'CA', '05'),
    ('19', 'Lake Tahoe Basin', 'CA', '05'),
    ('01', 'Deschutes', 'OR', '06'),
    ('02', 'Fremont-Winema', 'OR', '06'),
    ('03', 'Gifford Pinchot', 'WA', '06'),
    ('04', 'Malheur', 'OR', '06'),
    ('05', 'Mt. Baker-Snoqualmie', 'WA', '06'),
    ('06', 'Mt. Hood', 'OR', '06'),
    ('07', 'Ochoco', 'OR', '06'),
    ('09', 'Olympic', 'WA', '06'),
    ('10', 'Rogue River-Siskiyou', 'OR', '06'),
    ('12', 'Siuslaw', 'OR', '06'),
    ('14', 'Umatilla', 'OR', '06'),
    ('15', 'Umpqua', 'OR', '06'),
    ('16', 'Wallowa-Whitman', 'OR', '06'),
    ('17', 'Okanogan-Wenatchee', 'WA', '06'),
    ('18', 'Willamette', 'OR', '06'),
    ('21', 'Colville', 'WA', '06'),
    ('01', 'National Forests Alabama', 'AL', '08'),
    ('02', 'Daniel Boone', 'KY', '08'),
    ('03', 'Chattahoochee-Oconee', 'GA', '08'),
    ('04', 'Cherokee', 'TN', '08'),
    ('05', 'National Forests Florida', 'FL', '08'),
    ('06', 'Kisatchie', 'LA', '08'),
    ('07', 'National Forests Mississippi', 'MS', '08'),
    ('08', 'George Washington-Jefferson', 'NA', '08'),
    ('09', 'Ouachita', 'AR', '08'),
    ('10', 'Ozark-St. Francis', 'AR', '08'),
    ('11', 'National Forests N.Carolina', 'NC', '08'),
    ('12', 'Francis Marion-Sumter', 'SC', '08'),
    ('13', 'National Forests Texas', 'TX', '08'),
    ('16', 'El Yunque', 'PR', '08'),
    ('60', 'Land Between the Lakes', 'KY', '08'),
    ('03', 'Chippewa', 'MN', '09'),
    ('04', 'Huron-Manistee', 'MI', '09'),
    ('05', 'Mark Twain', 'MO', '09'),
    ('07', 'Ottawa', 'MI', '09'),
    ('08', 'Shawnee', 'IL', '09'),
    ('09', 'Superior', 'MN', '09'),
    ('10', 'Hiawatha', 'MI', '09'),
    ('12', 'Hoosier', 'IN', '09'),
    ('13', 'Chequamegon/Nicolet', 'WI', '09'),
    ('14', 'Wayne', 'OH', '09'),
    ('19', 'Allegheny', 'PA', '09'),
    ('20', 'Green Mountain-Finger Lakes', 'VT', '09'),
    ('21', 'Monongahela', 'WV', '09'),
    ('22', 'White Mountain', 'NH', '09'),
    ('04', 'Chugach', 'AK', '10'),
    ('05', 'Tongass', 'AK', '10'),
    ('00', 'Unknown BLM Forest', '??', '07'),
    ('00', 'Unknown DOD Forest', '??', '11'),
    ('JBLM', 'JBLM', 'WA', '11')
;";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}