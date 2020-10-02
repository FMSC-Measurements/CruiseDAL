using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public class DeviceTableDefinition : ITableDefinition
    {
        public const string CREATE_TABLE_DEVICE =
@"CREATE TABLE Device ( 
    DeviceID TEXT NOT NULL COLLATE NOCASE, 
    Name TEXT, 

    UNIQUE (DeviceID)
);";

        public string TableName => "Device";

        public string CreateTable =>
@"CREATE TABLE Device ( 
    DeviceID TEXT NOT NULL COLLATE NOCASE, 
    Name TEXT, 

    UNIQUE (DeviceID)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}
