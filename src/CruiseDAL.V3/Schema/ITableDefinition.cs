using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public interface ITableDefinition
    {
        string TableName { get; }

        string CreateTable { get; }

        string InitializeTable { get; }

        string CreateTombstoneTable { get; }

        string CreateIndexes { get; }

        IEnumerable<string> CreateTriggers { get; }
    }
}
