using FMSC.ORM.Core;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_6 : DbUpdateBase
    {
        public UpdateTo_3_6_6()
            : base(targetVersion: "3.6.6", sourceVersions: new[] { "3.6.5", })
        {
        }

        //protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        //{
        //    conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        //}

        //protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        //{
        //    conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        //}

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            // automatically turned back off when the transaction commits
            conn.ExecuteNonQuery("PRAGMA defer_foreign_keys=ON;");

            // When importing cruises into FScruiser and TreeAuditRuleID is used to determin if a TreeAuditRule already exists.
            // becuase when creating a cruise using a V3 template file this value gets copied to the new cruise with out being changed
            // it won't be imported if another cruise was created with the same template. 

            // to fix this, we are going to update all TreeAuditRuleIDs by XORing them with the cruiseID. this will create a 
            // new TreeAuditRuleID that is uniqe based on the cruiseID value. So that given a original TreeAuditRuleID and CruiseID
            // we will get a new TreeAuditRuleID. Any updated database will have the same new TreeAuditRuleID if the CruiseID is the same. 

            // logic for copying data from templates will also be updated so that new cruises will not have the same TreeAuditRuleIDs

            var tarMaps = conn.Query<TreeAuditRuleIDMapping>("SELECT CruiseID, TreeAuditRuleID FROM TreeAuditRule;")
                .ToArray();

            foreach(var map in tarMaps)
            {
                var cruiseIDBytes = new Guid(map.CruiseID).ToByteArray();
                var tarIDBytes = new Guid(map.TreeAuditRuleID).ToByteArray();

                var newTarIDBytes = cruiseIDBytes.Zip(tarIDBytes, (x, y) => (byte)(x ^ y)).ToArray();
                var newTarID = new Guid(newTarIDBytes);

                map.NewTreeAuditRuleID = newTarID.ToString();
            }


            // we want to go thru and remove all entries where the new TAR ID already exist
            // if it exists we want to cull it from the list of tar ids to update as well
            // the tar that already has the new id must have already been updated and we don't need to reupdate it

            var tarIDLookup = tarMaps.ToDictionary(x => x.TreeAuditRuleID); // TreeAuditRuleID uniqueness enforced by database

            foreach (var map in tarMaps)
            {
                if(tarIDLookup.ContainsKey(map.NewTreeAuditRuleID))
                {
                    var match = tarIDLookup[map.NewTreeAuditRuleID];
                    match.Ignore = true;
                    map.Ignore = true;
                }
            }

            foreach(var map in tarMaps)
            {
                if (map.Ignore) { continue; }
                conn.ExecuteNonQuery("UPDATE TreeAuditRule SET TreeAuditRuleID = @p1 WHERE TreeAuditRuleID = @p2;"
                    + "UPDATE TreeAuditRuleSelector SET TreeAuditRuleID = @p1 WHERE TreeAuditRuleID = @p2;" 
                    + "UPDATE TreeAuditResolution SET TreeAuditRuleID = @p1 WHERE TreeAuditRuleID = @p2;"
                    , new[] { map.NewTreeAuditRuleID, map.TreeAuditRuleID }, transaction, exceptionProcessor);
            }
        }


        class TreeAuditRuleIDMapping
        {
            public string CruiseID { get; set; }
            public string TreeAuditRuleID { get; set; }

            [IgnoreField]
            public string NewTreeAuditRuleID { get; set; }

            [IgnoreField]
            public bool Ignore {  get; set; }
        }
    }
}
