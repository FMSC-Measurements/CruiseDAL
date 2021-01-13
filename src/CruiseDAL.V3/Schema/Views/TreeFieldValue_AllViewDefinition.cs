using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema.Views
{
    public class TreeFieldValue_AllViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeFieldValue_All";

        public string CreateView =>
@"CREATE VIEW TreeFieldValue_All AS
SELECT
    TreeID,
    Field,
    ValueInt,
    ValueReal,
    ValueBool,
    ValueText,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
FROM TreeFieldValue_TreeMeasurment
UNION ALL
SELECT
    TreeID,
    Field,
    ValueInt,
    ValueReal,
    ValueBool,
    ValueText,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
FROM TreeFieldValue;";
    }
}
