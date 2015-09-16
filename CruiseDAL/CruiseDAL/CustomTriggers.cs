using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CruiseDAL.SQLExpressionTypes;

namespace CruiseDAL
{
    public static class CreateSQL
    {
        public static List<CreateTriggerStatment> CustomTriggers; 

        public static void InitializeCustomTriggers()
        {
            CustomTriggers = new List<CreateTriggerStatment>();
                
            CustomTriggers.Add(
                new CreateTriggerStatment(){ TableName = "Stratum",
                    TriggerSequencing = TriggerSequencing.After,
                    TriggerEvent = TriggerEvent.OnInsert,
                    ForEachRow = true,
                    TriggerStatments = new InsertStatment[] { 
                        new InsertStatment(){
                            TableName = Schema.TREEFIELDSETUP._NAME,
                            ColumnList = new String[] {Schema.TREEFIELDSETUP.FIELD, Schema.TREEFIELDSETUP.HEADING, Schema.TREEFIELDSETUP.FIELDORDER, Schema.TREEFIELDSETUP.STRATUM_CN},
                            SelectStatment = "SELECT Field, FieldName, FieldOrder, NEW.Stratum_CN FROM TreeFieldSetupDefault WHERE Method = NEW.Method"
                        }
                    }});
                    
                    
                    
            

        }



    }
}
