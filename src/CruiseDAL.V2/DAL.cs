using Backpack.SqlBuilder;
using CruiseDAL.Util;
using FMSC.ORM.Core;
using FMSC.ORM.EntityModel;
using FMSC.ORM.Logging;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;

#pragma warning disable RECS0122 // Initializing field with default value is redundant
#pragma warning disable RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant

namespace CruiseDAL
{
    public enum CruiseFileType { Unknown, Cruise, Template, Design, Master = 32 | Cruise, Component = 64 | Cruise, Backup = 128 | Cruise }

    public class DAL : CruiseDatastore
    {
        public const string CURENT_DBVERSION = "2.5.0";

        public CruiseFileType CruiseFileType
        {
            get
            {
                return ExtrapolateCruiseFileType(this.Path);
            }
        }

        public string User => EnvironmentInfoProvider.Instance.UserInfo;

        /// <summary>
        /// Initializes a new in memory instance of the <see cref="DAL"/> class.
        /// </summary>
        public DAL() : this(IN_MEMORY_DB_PATH, true)
        { }

        /// <summary>
        /// Creates a DAL instance for a database @ path.
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="IOException">problem working with file. wrong extension</exception>
        /// <exception cref="FileNotFoundException">file doesn't exist</exception>
        /// <param name="path"></param>
        public DAL(string path) : this(path, false)
        {
        }

        public DAL(string path, bool makeNew)
            : base(path, makeNew, new CruiseDatastoreBuilder_V2(), (IUpdater)new Updater_V2())
        {
        }

        protected override void OnConnectionOpened(DbConnection connection)
        {
            base.OnConnectionOpened(connection);
            connection.ExecuteNonQuery("PRAGMA foreign_keys=off");
        }

        protected override bool IsExtentionValid(string path)
        {
            return ExtrapolateCruiseFileType(path) != CruiseFileType.Unknown;
        }

        public static CruiseFileType ExtrapolateCruiseFileType(String path)
        {
            String normPath = path.ToLower().TrimEnd();
            if (String.IsNullOrEmpty(normPath))
            {
                return CruiseFileType.Unknown;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.m\.cruise\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return CruiseFileType.Master;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.\d+\.cruise\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return CruiseFileType.Component;
            }
            else if (normPath.EndsWith(".cruise", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Cruise;
            }
            else if (normPath.EndsWith(".cut", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Template;
            }
            else if (normPath.EndsWith(".design", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Design;
            }
            else if (normPath.EndsWith(".back-cruise", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Backup;
            }

            return CruiseFileType.Unknown;
        }

        protected CruiseFileType ReadCruiseFileType()
        {
            String s = this.ReadGlobalValue("Database", "CruiseFileType");
            try
            {
                return (CruiseFileType)Enum.Parse(typeof(CruiseFileType), s, true);
            }
            catch
            {
                return CruiseFileType.Unknown;
            }
        }

        public void Save(DataObject_Base data)
        {
            Save(data, OnConflictOption.Default);
        }

        public void Save(DataObject_Base data, OnConflictOption option, bool cache = true)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (data.IsChanged == false)
            {
                Logger.Log("save skipped because data has no changes", LogCategory.CRUD, LogLevel.Verbose);
                return;
            }

            if (!data.IsPersisted)
            {
                object primaryKey = Insert(data, option: option);

                var dal = data.DAL;
                if(dal != this)
                { data.InternalSetDAL(this); }

                if (cache && primaryKey != null)
                {
                    EntityCache cacheStore = GetEntityCache(data.GetType());

                    Debug.Assert(cacheStore.ContainsKey(primaryKey) == false, "Cache already contains entity, existing entity will be replaced");
                    if (cacheStore.ContainsKey(primaryKey))
                    {
                        cacheStore[primaryKey] = data;
                    }
                    else
                    {
                        cacheStore.Add(primaryKey, data);
                    }
                }
            }
            else
            {
                Update(data, option: option);
            }
        }
    }
}