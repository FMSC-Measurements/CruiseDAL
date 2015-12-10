using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectComponent
    {
        //SQLSelectBuilder _builder; 
        //public SQLSelectBuilder SelectExpression
        //{
        //    get { return _builder; }
        //    set
        //    {
        //        if(_builder == value) { return; }
        //        _builder = value;
        //        OnBuilderChanged(_builder);
        //    }
        //}

        //public SelectComponent(SQLSelectBuilder builder)
        //{
        //    this.SelectExpression = builder;
        //}

        //protected abstract void OnBuilderChanged(SQLSelectBuilder builder);


        public abstract String ToSQL();

        public override string ToString()
        {
            return ToSQL();
        }
    }
}
