using System;
using System.Data;

namespace FMSC.ORM.Core
{
    public interface IConnectionManager : IDisposable
    {
        object ConnectionSyncLock { get; }

        IDbConnection OpenConnection();

        void EnterConnectionHold();

        void ExitConnectionHold();

        //void ReleaseConnection();

        //void ReleaseConnection(bool force);

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        //void ReleaseTransaction();
    }
}