using System;

using System.Collections.Generic;
using System.Text;
using CruiseDAL.DataObjects;
using FMSC.ORM.Core.EntityModel;
using CruiseDAL.Schema;

#if ANDROID
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace CruiseDAL.MappingCollections
{
    public class CuttingUnitCollection : MappingCollection<CuttingUnitStratumDO, StratumDO, CuttingUnitDO>
    {
        public CuttingUnitCollection(StratumDO parent) : base(parent)
        {}

        protected override void addMap(CuttingUnitDO child)
        {

            CuttingUnitStratumDO newMap = new CuttingUnitStratumDO(DAL);
            newMap.CuttingUnit = child;
            newMap.Stratum = base.Parent;
            newMap.Save();
        }

        protected override CuttingUnitStratumDO retrieveMapObject(CuttingUnitDO child)
        {
            return DAL.ReadSingleRow<CuttingUnitStratumDO>(
                "WHERE CuttingUnit_CN = ? AND Stratum_CN = ?", child.CuttingUnit_CN, Parent.Stratum_CN);
        }

        protected override List<CuttingUnitDO> retrieveChildList()
        {
            //return DAL.Read<CuttingUnitDO>(createJoinChildCommand());
            return DAL.Read<CuttingUnitDO>(
                "JOIN CuttingUnitStratum Using (CuttingUnit_CN) WHERE Stratum_CN = ?;",
                Parent.Stratum_CN);
        }

//        protected override SQLiteCommand createJoinChildCommand()
//        {
//            string query = String.Format(@"SELECT cu.*, cu.rowID as rowID FROM CuttingUnit cu, CuttingUnitStratum cust 
//                                            WHERE cust.CuttingUnit_CN = cu.CuttingUnit_CN 
//                                            AND cust.Stratum_CN = {0};", Parent.Stratum_CN);
//            return new SQLiteCommand(query);
//        }

    }

    public class StratumCollection : MappingCollection<CuttingUnitStratumDO, CuttingUnitDO, StratumDO>
    {
        public StratumCollection(CuttingUnitDO parent) : base(parent) { }


        protected override void  addMap(StratumDO child)
        {
            CuttingUnitStratumDO newMap = new CuttingUnitStratumDO(DAL);
            newMap.CuttingUnit = base.Parent;
            newMap.Stratum = child;
            newMap.Save();
        }

        protected override CuttingUnitStratumDO  retrieveMapObject(StratumDO child)
        {
            return DAL.ReadSingleRow<CuttingUnitStratumDO>(
                "WHERE Stratum_CN = ? AND CuttingUnit_CN = ?", child.Stratum_CN, Parent.CuttingUnit_CN);
        }

        protected override List<StratumDO>  retrieveChildList()
        {
            //return DAL.Read<StratumDO>(createJoinChildCommand());
            return DAL.Read<StratumDO>(
                "JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnit_CN = ?;",
                Parent.CuttingUnit_CN);
        }

//        protected override SQLiteCommand  createJoinChildCommand()
//        {
//            string query = String.Format(@"SELECT st.*, st.rowID as rowID FROM Stratum st, CuttingUnitStratum cust 
//                                            WHERE cust.Stratum_CN = st.Stratum_CN
//                                            AND cust.CuttingUnit_CN = {0};", Parent.CuttingUnit_CN);
//            return new SQLiteCommand(query);
//        }
    }

    public class TreeDefaultValueCollection : MappingCollection<SampleGroupTreeDefaultValueDO, SampleGroupDO, TreeDefaultValueDO>
    {
        public TreeDefaultValueCollection(SampleGroupDO parent) : base(parent) { }


        protected override void addMap(TreeDefaultValueDO child)
        {
            SampleGroupTreeDefaultValueDO newMap = new SampleGroupTreeDefaultValueDO(DAL);
            newMap.SampleGroup = base.Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override SampleGroupTreeDefaultValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.ReadSingleRow<SampleGroupTreeDefaultValueDO>(
                "WHERE TreeDefaultValue_CN = ? AND SampleGroup_CN = ?", child.TreeDefaultValue_CN, Parent.SampleGroup_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            //return DAL.Read<TreeDefaultValueDO>(createJoinChildCommand());
            return DAL.Read<TreeDefaultValueDO>(
                "JOIN SampleGroupTreeDefaultValue Using (TreeDefaultValue_CN) WHERE SampleGroup_CN = ?;",
                Parent.SampleGroup_CN);

        }

        //protected override SQLiteCommand createJoinChildCommand()
        //{
        //    var query = string.Format("SELECT td.*, td.rowID as rowID FROM {0} td, {1} sgtd WHERE td.{2} = sgtd.{3} AND sgtd.SampleGroup_CN = {4}", 
        //        Schema.TREEDEFAULTVALUE._NAME,
        //        Schema.SAMPLEGROUPTREEDEFAULTVALUE._NAME,
        //        Schema.TREEDEFAULTVALUE.TREEDEFAULTVALUE_CN,
        //        Schema.SAMPLEGROUPTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN,
        //        Parent.SampleGroup_CN);
        //    return new SQLiteCommand(query);
        //}

        
    }

    public class TreeAuditValueCollection : MappingCollection<TreeDefaultValueTreeAuditValueDO, TreeDefaultValueDO, TreeAuditValueDO>
    {
        public TreeAuditValueCollection(TreeDefaultValueDO parent) : base(parent) { }

        protected override void addMap(TreeAuditValueDO child)
        {
            TreeDefaultValueTreeAuditValueDO newMap = new TreeDefaultValueTreeAuditValueDO(DAL);
            newMap.TreeDefaultValue = base.Parent;
            newMap.TreeAuditValue = child;
            newMap.Save();
        }

        protected override TreeDefaultValueTreeAuditValueDO retrieveMapObject(TreeAuditValueDO child)
        {
            return DAL.ReadSingleRow<TreeDefaultValueTreeAuditValueDO>(
                "WHERE TreeAuditValue_CN = ? AND TreeDefaultValue_CN = ?", child.TreeAuditValue_CN, Parent.TreeDefaultValue_CN);
        }

        protected override List<TreeAuditValueDO> retrieveChildList()
        {
            return DAL.Read<TreeAuditValueDO>(
                "JOIN TreeDefaultValueTreeAuditValue Using (TreeAuditValue_CN) WHERE TreeDefaultValue_CN = ?", Parent.TreeDefaultValue_CN);
        }
    }

    public class TreeAuditValueTreeDefaultValueCollection : MappingCollection<TreeDefaultValueTreeAuditValueDO, TreeAuditValueDO, TreeDefaultValueDO>
    {
        public TreeAuditValueTreeDefaultValueCollection(TreeAuditValueDO parent)
            : base(parent)
        { }

        protected override void addMap(TreeDefaultValueDO child)
        {
            TreeDefaultValueTreeAuditValueDO newMap = new TreeDefaultValueTreeAuditValueDO(DAL);
            newMap.TreeAuditValue = base.Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override TreeDefaultValueTreeAuditValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.ReadSingleRow<TreeDefaultValueTreeAuditValueDO>(
                "WHERE " + TREEDEFAULTVALUETREEAUDITVALUE.TREEDEFAULTVALUE_CN + " = ? AND " + TREEDEFAULTVALUETREEAUDITVALUE.TREEAUDITVALUE_CN + " = ?", child.TreeDefaultValue_CN, Parent.TreeAuditValue_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            return DAL.Read<TreeDefaultValueDO>(
                "JOIN TreeDefaultValueTreeAuditValue USING (TreeDefaultValue_CN) WHERE TreeAuditValue_CN = ?", base.Parent.TreeAuditValue_CN);
        }

    }

    public class TreeDefaultValueStatsCollection : MappingCollection<SampleGroupStatsTreeDefaultValueDO, SampleGroupStatsDO, TreeDefaultValueDO>
    {
        public TreeDefaultValueStatsCollection(SampleGroupStatsDO parent) : base(parent) { }


        protected override void addMap(TreeDefaultValueDO child)
        {
            SampleGroupStatsTreeDefaultValueDO newMap = new SampleGroupStatsTreeDefaultValueDO(DAL);
            newMap.SampleGroupStats = base.Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override SampleGroupStatsTreeDefaultValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.ReadSingleRow<SampleGroupStatsTreeDefaultValueDO>(
                "WHERE " + SAMPLEGROUPSTATSTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN + "= ? AND " + SAMPLEGROUPSTATSTREEDEFAULTVALUE.SAMPLEGROUPSTATS_CN + " = ?", child.TreeDefaultValue_CN, base.Parent.SampleGroupStats_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            //return DAL.Read<TreeDefaultValueDO>(createJoinChildCommand());
            return DAL.Read<TreeDefaultValueDO>(
                "JOIN SampleGroupStatsTreeDefaultValue Using (TreeDefaultValue_CN) WHERE SampleGroupStats_CN = ?;",
                Parent.SampleGroupStats_CN);
        }

        //protected override SQLiteCommand createJoinChildCommand()
        //{
        //    var query = string.Format("SELECT td.*, td.rowID as rowID FROM {0} td, {1} sgtd WHERE td.{2} = sgtd.{3} AND sgtd.SampleGroupStats_CN = {4}",
        //        Schema.TREEDEFAULTVALUE._NAME,
        //        Schema.SAMPLEGROUPSTATSTREEDEFAULTVALUE._NAME,
        //        Schema.TREEDEFAULTVALUE.TREEDEFAULTVALUE_CN,
        //        Schema.SAMPLEGROUPSTATSTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN,
        //        Parent.SampleGroupStats_CN);
        //    return new SQLiteCommand(query);
        //}
    }

}
