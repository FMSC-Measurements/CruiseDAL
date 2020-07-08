using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel;
using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.MappingCollections
{
    public class CuttingUnitCollection : MappingCollection<CuttingUnitStratumDO, StratumDO, CuttingUnitDO>
    {
        public CuttingUnitCollection(StratumDO parent) : base(parent)
        { }

        protected override void addMap(CuttingUnitDO child)
        {
            var newMap = new CuttingUnitStratumDO(DAL);
            newMap.CuttingUnit = child;
            newMap.Stratum = Parent;
            newMap.Save();
        }

        protected override CuttingUnitStratumDO retrieveMapObject(CuttingUnitDO child)
        {
            return DAL.From<CuttingUnitStratumDO>().Where("CuttingUnit_CN = @p1 AND Stratum_CN = @p2")
                .Read(child.CuttingUnit_CN, Parent.Stratum_CN).FirstOrDefault();

            //.ReadSingleRow<CuttingUnitStratumDO>(
            //    "WHERE CuttingUnit_CN = ? AND Stratum_CN = ?", child.CuttingUnit_CN, Parent.Stratum_CN);
        }

        protected override List<CuttingUnitDO> retrieveChildList()
        {
            return DAL.From<CuttingUnitDO>()
                .Join("CuttingUnitStratum", "USING (CuttingUnit_CN)")
                .Where("Stratum_CN = @p1")
                .Read(Parent.Stratum_CN).ToList();

            //.Read<CuttingUnitDO>(
            //"JOIN CuttingUnitStratum Using (CuttingUnit_CN) WHERE Stratum_CN = ?;",
            //Parent.Stratum_CN);
        }
    }

    public class StratumCollection : MappingCollection<CuttingUnitStratumDO, CuttingUnitDO, StratumDO>
    {
        public StratumCollection(CuttingUnitDO parent) : base(parent)
        {
        }

        protected override void addMap(StratumDO child)
        {
            var newMap = new CuttingUnitStratumDO(DAL);
            newMap.CuttingUnit = Parent;
            newMap.Stratum = child;
            newMap.Save();
        }

        protected override CuttingUnitStratumDO retrieveMapObject(StratumDO child)
        {
            return DAL.From<CuttingUnitStratumDO>()
                .Where("Stratum_CN = @p1 AND CuttingUnit_CN = @p2")
                .Read(child.Stratum_CN, Parent.CuttingUnit_CN)
                .FirstOrDefault();

            //.ReadSingleRow<CuttingUnitStratumDO>(
            //"WHERE Stratum_CN = ? AND CuttingUnit_CN = ?", child.Stratum_CN, Parent.CuttingUnit_CN);
        }

        protected override List<StratumDO> retrieveChildList()
        {
            return DAL.From<StratumDO>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Where("CuttingUnit_CN = @p1")
                .Read(Parent.CuttingUnit_CN).ToList();

            //.Read<StratumDO>(
            //"JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnit_CN = ?;",
            //Parent.CuttingUnit_CN);
        }
    }

    public class TreeDefaultValueCollection : MappingCollection<SampleGroupTreeDefaultValueDO, SampleGroupDO, TreeDefaultValueDO>
    {
        public TreeDefaultValueCollection(SampleGroupDO parent) : base(parent)
        {
        }

        protected override void addMap(TreeDefaultValueDO child)
        {
            var newMap = new SampleGroupTreeDefaultValueDO(DAL);
            newMap.SampleGroup = Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override SampleGroupTreeDefaultValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.From<SampleGroupTreeDefaultValueDO>()
                .Where("TreeDefaultValue_CN = @p1 AND SampleGroup_CN = @p2")
                .Read(child.TreeDefaultValue_CN, Parent.SampleGroup_CN)
                .FirstOrDefault();

            //.ReadSingleRow<SampleGroupTreeDefaultValueDO>(
            //    "WHERE TreeDefaultValue_CN = ? AND SampleGroup_CN = ?", child.TreeDefaultValue_CN, Parent.SampleGroup_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            return DAL.From<TreeDefaultValueDO>()
                .Join("SampleGroupTreeDefaultValue", "Using(TreeDefaultValue_CN)")
                .Where("SampleGroup_CN = @p1")
                .Read(Parent.SampleGroup_CN).ToList();

            //.Read<TreeDefaultValueDO>(
            //    "JOIN SampleGroupTreeDefaultValue Using (TreeDefaultValue_CN) WHERE SampleGroup_CN = ?;",
            //    Parent.SampleGroup_CN);
        }
    }

    public class TreeAuditValueCollection : MappingCollection<TreeDefaultValueTreeAuditValueDO, TreeDefaultValueDO, TreeAuditValueDO>
    {
        public TreeAuditValueCollection(TreeDefaultValueDO parent) : base(parent)
        {
        }

        protected override void addMap(TreeAuditValueDO child)
        {
            var newMap = new TreeDefaultValueTreeAuditValueDO(DAL);
            newMap.TreeDefaultValue = Parent;
            newMap.TreeAuditValue = child;
            newMap.Save();
        }

        protected override TreeDefaultValueTreeAuditValueDO retrieveMapObject(TreeAuditValueDO child)
        {
            return DAL.From<TreeDefaultValueTreeAuditValueDO>()
                .Where("TreeAuditValue_CN = @p1 AND TreeDefaultValue_CN = @p2")
                .Read(child.TreeAuditValue_CN, Parent.TreeDefaultValue_CN)
                .FirstOrDefault();

            //.ReadSingleRow<TreeDefaultValueTreeAuditValueDO>(
            //    "WHERE TreeAuditValue_CN = ? AND TreeDefaultValue_CN = ?", child.TreeAuditValue_CN, Parent.TreeDefaultValue_CN);
        }

        protected override List<TreeAuditValueDO> retrieveChildList()
        {
            return DAL.From<TreeAuditValueDO>()
                .Join("TreeDefaultValueTreeAuditValue", "Using(TreeAuditValue_CN)")
                .Where("TreeDefaultValue_CN = @p1")
                .Read(Parent.TreeDefaultValue_CN).ToList();

            //.Read<TreeAuditValueDO>(
            //"JOIN TreeDefaultValueTreeAuditValue Using (TreeAuditValue_CN) WHERE TreeDefaultValue_CN = ?", Parent.TreeDefaultValue_CN);
        }
    }

    public class TreeAuditValueTreeDefaultValueCollection : MappingCollection<TreeDefaultValueTreeAuditValueDO, TreeAuditValueDO, TreeDefaultValueDO>
    {
        public TreeAuditValueTreeDefaultValueCollection(TreeAuditValueDO parent)
            : base(parent)
        { }

        protected override void addMap(TreeDefaultValueDO child)
        {
            var newMap = new TreeDefaultValueTreeAuditValueDO(DAL);
            newMap.TreeAuditValue = Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override TreeDefaultValueTreeAuditValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.From<TreeDefaultValueTreeAuditValueDO>()
                .Where("TreeDefaultValue_CN = @p1 AND TreeAuditValue_CN = @p2")
                .Read(child.TreeDefaultValue_CN, Parent.TreeAuditValue_CN).FirstOrDefault();

            //.ReadSingleRow<TreeDefaultValueTreeAuditValueDO>(
            //    "WHERE " + TREEDEFAULTVALUETREEAUDITVALUE.TREEDEFAULTVALUE_CN + " = ? AND " + TREEDEFAULTVALUETREEAUDITVALUE.TREEAUDITVALUE_CN + " = ?", child.TreeDefaultValue_CN, Parent.TreeAuditValue_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            return DAL.From<TreeDefaultValueDO>()
                .Join("TreeDefaultValueTreeAuditValue", "USING(TreeDefaultValue_CN)")
                .Where("TreeAuditValue_CN = @p1")
                .Read(Parent.TreeAuditValue_CN).ToList();

            //.Read<TreeDefaultValueDO>(
            //"JOIN TreeDefaultValueTreeAuditValue USING (TreeDefaultValue_CN) WHERE TreeAuditValue_CN = ?", base.Parent.TreeAuditValue_CN);
        }
    }

    public class TreeDefaultValueStatsCollection : MappingCollection<SampleGroupStatsTreeDefaultValueDO, SampleGroupStatsDO, TreeDefaultValueDO>
    {
        public TreeDefaultValueStatsCollection(SampleGroupStatsDO parent) : base(parent)
        {
        }

        protected override void addMap(TreeDefaultValueDO child)
        {
            var newMap = new SampleGroupStatsTreeDefaultValueDO(DAL);
            newMap.SampleGroupStats = Parent;
            newMap.TreeDefaultValue = child;
            newMap.Save();
        }

        protected override SampleGroupStatsTreeDefaultValueDO retrieveMapObject(TreeDefaultValueDO child)
        {
            return DAL.From<SampleGroupStatsTreeDefaultValueDO>()
                .Where("TreeDefaultValue_CN = @p1 AND SampleGroupStats_CN = @p2")
                .Read(child.TreeDefaultValue_CN, Parent.SampleGroupStats_CN)
                .FirstOrDefault();

            //.ReadSingleRow<SampleGroupStatsTreeDefaultValueDO>(
            //"WHERE " + SAMPLEGROUPSTATSTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN + "= ? AND " + SAMPLEGROUPSTATSTREEDEFAULTVALUE.SAMPLEGROUPSTATS_CN + " = ?", child.TreeDefaultValue_CN, base.Parent.SampleGroupStats_CN);
        }

        protected override List<TreeDefaultValueDO> retrieveChildList()
        {
            return DAL.From<TreeDefaultValueDO>()
                .Join("SampleGroupStatsTreeDefaultValue", "Using(TreeDefaultValue_CN)")
                .Where("SampleGroupStats_CN = @p1")
                .Read(Parent.SampleGroupStats_CN).ToList();

            //.Read<TreeDefaultValueDO>(
            //"JOIN SampleGroupStatsTreeDefaultValue Using (TreeDefaultValue_CN) WHERE SampleGroupStats_CN = ?;",
            //Parent.SampleGroupStats_CN);
        }
    }
}