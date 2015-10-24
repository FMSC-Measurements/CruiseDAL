using CruiseDAL.BaseDAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace CruiseDAL.BaseDAL
{
    public class EntitySelectCommandBuilder
    {
        private SQLSelectExpression _selectCommand;

        public EntityDescription EntityDescription { get; set; }
        private DatastoreBase _dataStore;

        public EntitySelectCommandBuilder(EntityDescription entDesc, DatastoreBase dataStore)
        {
            _dataStore = dataStore;
            EntityDescription = entDesc;

            Initialize();
        }

        int CompareFieldsByOrdinal(EntityFieldInfo x, EntityFieldInfo y)
        {
            if(x._ordinal == y._ordinal) { return 0; }
            if(x._ordinal == -1) { return 1; }
            if(y._ordinal == -1) { return -1; }
            if(x._ordinal > y._ordinal) { return 1; }
            else { return -1;  }
        }

        public DbCommand BuildSellectCommand(WhereClause where)
        {
            this._selectCommand.Where = where;
            string query = _selectCommand.ToString();

            DbCommand command = _dataStore.CreateCommand(query);
            return command;

        }

        void Initialize()
        {
            SQLSelectExpression expression = new SQLSelectExpression();
            expression.TableOrSubQuery = EntityDescription.SourceName;

            List<EntityFieldInfo> fields = new List<EntityFieldInfo>(EntityDescription.Fields.Values);
            fields.Sort(CompareFieldsByOrdinal);

            List<string> columnExpressions = new List<string>();

            foreach (EntityFieldInfo fi in fields)
            {
                String colExpression = null;

                if (!string.IsNullOrEmpty(fi.SQLExpression))
                {
                    colExpression = fi.SQLExpression + " AS " + fi.FieldName;
                }
                else if (!String.IsNullOrEmpty(fi.Source) || !String.IsNullOrEmpty(EntityDescription.SourceName))
                {
                    string source = fi.Source ?? EntityDescription.SourceName;
                    colExpression = source + "." + fi.FieldName;
                }
                else
                {
                    colExpression = fi.FieldName;
                }

                columnExpressions.Add(colExpression);
            }
        }

    }
}
