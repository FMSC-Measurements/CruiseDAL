﻿using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class DeviceTableDefinition : ITableDefinition
    {
        public string TableName => "Device";

        public string CreateTable =>
@"CREATE TABLE Device (
    DeviceID TEXT NOT NULL COLLATE NOCASE, -- may be guid or may platform specific format
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Name TEXT,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    UNIQUE (DeviceID, CruiseID)
);";

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    DeviceID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Name TEXT,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    UNIQUE (DeviceID, CruiseID)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX NIX_Device_CruiseID ON Device (CruiseID);
CREATE INDEX NIX_Device_DeviceID ON Device (DeviceID);";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}