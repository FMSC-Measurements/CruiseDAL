using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.SQLite
{
    /// <summary>
    /// SQLite error codes.  Actually, this enumeration represents a return code,
    /// which may also indicate success in one of several ways (e.g. SQLITE_OK,
    /// SQLITE_ROW, and SQLITE_DONE).  Therefore, the name of this enumeration is
    /// something of a misnomer.
    /// </summary>
    internal enum SqliteResultCode
    {
        /// <summary>
        /// The error code is unknown.  This error code
        /// is only used by the managed wrapper itself.
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Successful result
        /// </summary>
        Ok /* 0 */,
        /// <summary>
        /// SQL error or missing database
        /// </summary>
        Error /* 1 */,
        /// <summary>
        /// Internal logic error in SQLite
        /// </summary>
        Internal /* 2 */,
        /// <summary>
        /// Access permission denied
        /// </summary>
        Perm /* 3 */,
        /// <summary>
        /// Callback routine requested an abort
        /// </summary>
        Abort /* 4 */,
        /// <summary>
        /// The database file is locked
        /// </summary>
        Busy /* 5 */,
        /// <summary>
        /// A table in the database is locked
        /// </summary>
        Locked /* 6 */,
        /// <summary>
        /// A malloc() failed
        /// </summary>
        NoMem /* 7 */,
        /// <summary>
        /// Attempt to write a readonly database
        /// </summary>
        ReadOnly /* 8 */,
        /// <summary>
        /// Operation terminated by sqlite3_interrupt()
        /// </summary>
        Interrupt /* 9 */,
        /// <summary>
        /// Some kind of disk I/O error occurred
        /// </summary>
        IoErr /* 10 */,
        /// <summary>
        /// The database disk image is malformed
        /// </summary>
        Corrupt /* 11 */,
        /// <summary>
        /// Unknown opcode in sqlite3_file_control()
        /// </summary>
        NotFound /* 12 */,
        /// <summary>
        /// Insertion failed because database is full
        /// </summary>
        Full /* 13 */,
        /// <summary>
        /// Unable to open the database file
        /// </summary>
        CantOpen /* 14 */,
        /// <summary>
        /// Database lock protocol error
        /// </summary>
        Protocol /* 15 */,
        /// <summary>
        /// Database is empty
        /// </summary>
        Empty /* 16 */,
        /// <summary>
        /// The database schema changed
        /// </summary>
        Schema /* 17 */,
        /// <summary>
        /// String or BLOB exceeds size limit
        /// </summary>
        TooBig /* 18 */,
        /// <summary>
        /// Abort due to constraint violation
        /// </summary>
        Constraint /* 19 */,
        /// <summary>
        /// Data type mismatch
        /// </summary>
        Mismatch /* 20 */,
        /// <summary>
        /// Library used incorrectly
        /// </summary>
        Misuse /* 21 */,
        /// <summary>
        /// Uses OS features not supported on host
        /// </summary>
        NoLfs /* 22 */,
        /// <summary>
        /// Authorization denied
        /// </summary>
        Auth /* 23 */,
        /// <summary>
        /// Auxiliary database format error
        /// </summary>
        Format /* 24 */,
        /// <summary>
        /// 2nd parameter to sqlite3_bind out of range
        /// </summary>
        Range /* 25 */,
        /// <summary>
        /// File opened that is not a database file
        /// </summary>
        NotADb /* 26 */,
        /// <summary>
        /// Notifications from sqlite3_log()
        /// </summary>
        Notice /* 27 */,
        /// <summary>
        /// Warnings from sqlite3_log()
        /// </summary>
        Warning /* 28 */,
        /// <summary>
        /// sqlite3_step() has another row ready
        /// </summary>
        Row = 100,
        /// <summary>
        /// sqlite3_step() has finished executing
        /// </summary>
        Done, /* 101 */
              /// <summary>
              /// Used to mask off extended result codes
              /// </summary>
        NonExtendedMask = 0xFF,

        /////////////////////////////////////////////////////////////////////////
        // BEGIN EXTENDED RESULT CODES
        /////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// A collation sequence was referenced by a schema and it cannot be
        /// found.
        /// </summary>
        Error_Missing_CollSeq = (Error | (1 << 8)),
        /// <summary>
        /// An internal operation failed and it may succeed if retried.
        /// </summary>
        Error_Retry = (Error | (2 << 8)),
        /// <summary>
        /// A file read operation failed.
        /// </summary>
        IoErr_Read = (IoErr | (1 << 8)),
        /// <summary>
        /// A file read operation returned less data than requested.
        /// </summary>
        IoErr_Short_Read = (IoErr | (2 << 8)),
        /// <summary>
        /// A file write operation failed.
        /// </summary>
        IoErr_Write = (IoErr | (3 << 8)),
        /// <summary>
        /// A file synchronization operation failed.
        /// </summary>
        IoErr_Fsync = (IoErr | (4 << 8)),
        /// <summary>
        /// A directory synchronization operation failed.
        /// </summary>
        IoErr_Dir_Fsync = (IoErr | (5 << 8)),
        /// <summary>
        /// A file truncate operation failed.
        /// </summary>
        IoErr_Truncate = (IoErr | (6 << 8)),
        /// <summary>
        /// A file metadata operation failed.
        /// </summary>
        IoErr_Fstat = (IoErr | (7 << 8)),
        /// <summary>
        /// A file unlock operation failed.
        /// </summary>
        IoErr_Unlock = (IoErr | (8 << 8)),
        /// <summary>
        /// A file lock operation failed.
        /// </summary>
        IoErr_RdLock = (IoErr | (9 << 8)),
        /// <summary>
        /// A file delete operation failed.
        /// </summary>
        IoErr_Delete = (IoErr | (10 << 8)),
        /// <summary>
        /// Not currently used.
        /// </summary>
        IoErr_Blocked = (IoErr | (11 << 8)),
        /// <summary>
        /// Out-of-memory during a file operation.
        /// </summary>
        IoErr_NoMem = (IoErr | (12 << 8)),
        /// <summary>
        /// A file existence/status operation failed.
        /// </summary>
        IoErr_Access = (IoErr | (13 << 8)),
        /// <summary>
        /// A check for a reserved lock failed.
        /// </summary>
        IoErr_CheckReservedLock = (IoErr | (14 << 8)),
        /// <summary>
        /// A file lock operation failed.
        /// </summary>
        IoErr_Lock = (IoErr | (15 << 8)),
        /// <summary>
        /// A file close operation failed.
        /// </summary>
        IoErr_Close = (IoErr | (16 << 8)),
        /// <summary>
        /// A directory close operation failed.
        /// </summary>
        IoErr_Dir_Close = (IoErr | (17 << 8)),
        /// <summary>
        /// A shared memory open operation failed.
        /// </summary>
        IoErr_ShmOpen = (IoErr | (18 << 8)),
        /// <summary>
        /// A shared memory size operation failed.
        /// </summary>
        IoErr_ShmSize = (IoErr | (19 << 8)),
        /// <summary>
        /// A shared memory lock operation failed.
        /// </summary>
        IoErr_ShmLock = (IoErr | (20 << 8)),
        /// <summary>
        /// A shared memory map operation failed.
        /// </summary>
        IoErr_ShmMap = (IoErr | (21 << 8)),
        /// <summary>
        /// A file seek operation failed.
        /// </summary>
        IoErr_Seek = (IoErr | (22 << 8)),
        /// <summary>
        /// A file delete operation failed because it does not exist.
        /// </summary>
        IoErr_Delete_NoEnt = (IoErr | (23 << 8)),
        /// <summary>
        /// A file memory mapping operation failed.
        /// </summary>
        IoErr_Mmap = (IoErr | (24 << 8)),
        /// <summary>
        /// The temporary directory path could not be obtained.
        /// </summary>
        IoErr_GetTempPath = (IoErr | (25 << 8)),
        /// <summary>
        /// A path string conversion operation failed.
        /// </summary>
        IoErr_ConvPath = (IoErr | (26 << 8)),
        /// <summary>
        /// Reserved.
        /// </summary>
        IoErr_VNode = (IoErr | (27 << 8)),
        /// <summary>
        /// An attempt to authenticate failed.
        /// </summary>
        IoErr_Auth = (IoErr | (28 << 8)),
        /// <summary>
        /// An attempt to begin a file system transaction failed.
        /// </summary>
        IoErr_Begin_Atomic = (IoErr | (29 << 8)),
        /// <summary>
        /// An attempt to commit a file system transaction failed.
        /// </summary>
        IoErr_Commit_Atomic = (IoErr | (30 << 8)),
        /// <summary>
        /// An attempt to rollback a file system transaction failed.
        /// </summary>
        IoErr_Rollback_Atomic = (IoErr | (31 << 8)),
        /// <summary>
        /// A database table is locked in shared-cache mode.
        /// </summary>
        Locked_SharedCache = (Locked | (1 << 8)),
        /// <summary>
        /// A database file is locked due to a recovery operation.
        /// </summary>
        Busy_Recovery = (Busy | (1 << 8)),
        /// <summary>
        /// A database file is locked due to snapshot semantics.
        /// </summary>
        Busy_Snapshot = (Busy | (2 << 8)),
        /// <summary>
        /// A database file cannot be opened because no temporary directory is available.
        /// </summary>
        CantOpen_NoTempDir = (CantOpen | (1 << 8)),
        /// <summary>
        /// A database file cannot be opened because its path represents a directory.
        /// </summary>
        CantOpen_IsDir = (CantOpen | (2 << 8)),
        /// <summary>
        /// A database file cannot be opened because its full path could not be obtained.
        /// </summary>
        CantOpen_FullPath = (CantOpen | (3 << 8)),
        /// <summary>
        /// A database file cannot be opened because a path string conversion operation failed.
        /// </summary>
        CantOpen_ConvPath = (CantOpen | (4 << 8)),
        /// <summary>
        /// A virtual table is malformed.
        /// </summary>
        Corrupt_Vtab = (Corrupt | (1 << 8)),
        /// <summary>
        /// A database file is read-only due to a recovery operation.
        /// </summary>
        ReadOnly_Recovery = (ReadOnly | (1 << 8)),
        /// <summary>
        /// A database file is read-only because a lock could not be obtained.
        /// </summary>
        ReadOnly_CantLock = (ReadOnly | (2 << 8)),
        /// <summary>
        /// A database file is read-only because it needs rollback processing.
        /// </summary>
        ReadOnly_Rollback = (ReadOnly | (3 << 8)),
        /// <summary>
        /// A database file is read-only because it was moved while open.
        /// </summary>
        ReadOnly_DbMoved = (ReadOnly | (4 << 8)),
        /// <summary>
        /// The shared-memory file is read-only and it should be read-write.
        /// </summary>
        ReadOnly_CantInit = (ReadOnly | (5 << 8)),
        /// <summary>
        /// Unable to create journal file because the directory is read-only.
        /// </summary>
        ReadOnly_Directory = (ReadOnly | (6 << 8)),
        /// <summary>
        /// An operation is being aborted due to rollback processing.
        /// </summary>
        Abort_Rollback = (Abort | (2 << 8)),
        /// <summary>
        /// A CHECK constraint failed.
        /// </summary>
        Constraint_Check = (Constraint | (1 << 8)),
        /// <summary>
        /// A commit hook produced a unsuccessful return code.
        /// </summary>
        Constraint_CommitHook = (Constraint | (2 << 8)),
        /// <summary>
        /// A FOREIGN KEY constraint failed.
        /// </summary>
        Constraint_ForeignKey = (Constraint | (3 << 8)),
        /// <summary>
        /// Not currently used.
        /// </summary>
        Constraint_Function = (Constraint | (4 << 8)),
        /// <summary>
        /// A NOT NULL constraint failed.
        /// </summary>
        Constraint_NotNull = (Constraint | (5 << 8)),
        /// <summary>
        /// A PRIMARY KEY constraint failed.
        /// </summary>
        Constraint_PrimaryKey = (Constraint | (6 << 8)),
        /// <summary>
        /// The RAISE function was used by a trigger-program.
        /// </summary>
        Constraint_Trigger = (Constraint | (7 << 8)),
        /// <summary>
        /// A UNIQUE constraint failed.
        /// </summary>
        Constraint_Unique = (Constraint | (8 << 8)),
        /// <summary>
        /// Not currently used.
        /// </summary>
        Constraint_Vtab = (Constraint | (9 << 8)),
        /// <summary>
        /// A ROWID constraint failed.
        /// </summary>
        Constraint_RowId = (Constraint | (10 << 8)),
        /// <summary>
        /// Frames were recovered from the WAL log file.
        /// </summary>
        Notice_Recover_Wal = (Notice | (1 << 8)),
        /// <summary>
        /// Pages were recovered from the journal file.
        /// </summary>
        Notice_Recover_Rollback = (Notice | (2 << 8)),
        /// <summary>
        /// An automatic index was created to process a query.
        /// </summary>
        Warning_AutoIndex = (Warning | (1 << 8)),
        /// <summary>
        /// User authentication failed.
        /// </summary>
        Auth_User = (Auth | (1 << 8)),
        /// <summary>
        /// Success.  Prevents the extension from unloading until the process
        /// terminates.
        /// </summary>
        Ok_Load_Permanently = (Ok | (1 << 8))
    }
}
