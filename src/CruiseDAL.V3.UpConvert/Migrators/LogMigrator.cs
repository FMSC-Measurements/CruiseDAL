using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class LogMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.Log (
    Log_CN,
    LogID,
    TreeID,
    LogNumber,
    Grade,
    SeenDefect,
    PercentRecoverable,
    Length,
    ExportGrade,
    SmallEndDiameter,
    LargeEndDiameter,
    GrossBoardFoot,
    NetBoardFoot,
    GrossCubicFoot,
    NetCubicFoot,
    BoardFootRemoved,
    CubicFootRemoved,
    DIBClass,
    BarkThickness,
    CreatedBy
)
SELECT
    l.Log_CN,
    ifnull(
        (CASE typeof(l.Log_GUID) COLLATE NOCASE -- ckeck the type of Log_GUID
            WHEN 'TEXT' THEN --if text
                (CASE WHEN l.Log_GUID LIKE '________-____-____-____-____________' -- check to see if it is a properly formated guid
                    THEN nullif(l.Log_GUID, '00000000-0000-0000-0000-000000000000')  -- if not a empty guid return that value otherwise return null for now
                    ELSE NULL END) -- if it is not a properly formatted guid return Log_GUID
            ELSE NULL END) -- if value is not a string return null
        , (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
                || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
                || substr('AB89', 1 + (abs(random()) % 4), 1) || 
                substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS LogID, 
    t.TreeID AS TreeID, 
    l.LogNumber, 
    l.Grade, 
    l.SeenDefect,
    l.PercentRecoverable,
    l.Length,
    l.ExportGrade,
    l.SmallEndDiameter,
    l.LargeEndDiameter,
    l.GrossBoardFoot,
    l.NetBoardFoot,
    l.GrossCubicFoot,
    l.NetCubicFoot,
    l.BoardFootRemoved,
    l.CubicFootRemoved,
    l.DIBClass,
    l.BarkThickness,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.Log as l
JOIN {toDbName}.Tree AS t USING (Tree_CN);";
        }
    }
}
