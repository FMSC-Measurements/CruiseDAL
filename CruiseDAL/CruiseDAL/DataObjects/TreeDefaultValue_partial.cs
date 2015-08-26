using CruiseDAL.MappingCollections;
namespace CruiseDAL.DataObjects
{
    public partial class TreeDefaultValueDO : DataObject
    {

        ////support for depreciated member
        ///// <summary>
        ///// Depreciated
        ///// </summary>
        //public string TreeGrade
        //{
        //    get { return this.Grade; }
        //    set { this.Grade = value; }
        //}

        private TreeAuditValueCollection _treeAuditValueCollection;
        public TreeAuditValueCollection TreeAuditValues
        {
            get
            {
                if (_treeAuditValueCollection == null)
                {
                    _treeAuditValueCollection = new TreeAuditValueCollection(this);
                }
                return _treeAuditValueCollection;
            }
        }

        protected override void OnDALChanged(DatastoreBase newDAL)
        {
            base.OnDALChanged(newDAL);
            if (_treeAuditValueCollection != null)
            {
                _treeAuditValueCollection.DAL = newDAL;
            }
        }

        public override void Delete()
        {
            base.Delete();
            TreeAuditValues.Populate();
            TreeAuditValues.Clear();
            TreeAuditValues.Save();
        }
    }

}