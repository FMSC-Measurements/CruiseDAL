using Backpack.SqlBuilder;
using FMSC.ORM.EntityModel.Support;
using System.Data;

namespace FMSC.ORM.Sql
{
    public interface ICommandBuilder
    {
        SqlSelectBuilder BuildSelect(TableOrSubQuery source, IFieldInfoCollection fields);

        void BuildInsert(IDbCommand command,
                        object data,
                        string tableName,
                        IFieldInfoCollection fields,
                        OnConflictOption option = OnConflictOption.Default,
                        object keyValue = null,
                        bool persistKeyvalue = true);

        void BuildUpdate(IDbCommand command,
                         object data,
                         string tableName,
                         IFieldInfoCollection fields,
                         string whereExpression = null,
                         OnConflictOption option = OnConflictOption.Default,
                         object keyValue = null,
                         object extraPrams = null);

        void BuildDelete(IDbCommand command, object data, string tableName, IFieldInfoCollection fields);
    }
}