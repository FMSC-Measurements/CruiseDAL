using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL
{
    public interface IDbUpdate
    {
        string TargetVersion { get; set; }
        IEnumerable<string> SourceVersions { get; set; }

        void Update(DbConnection conn, IExceptionProcessor exceptionProcessor = null);
    }
}