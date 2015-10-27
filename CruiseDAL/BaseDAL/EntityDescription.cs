using System;
using System.Reflection;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Diagnostics;
using CruiseDAL.BaseDAL.EntityAttributes;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif


namespace CruiseDAL.BaseDAL
{
    public class EntityDescription
    {
        private SQLEntityAttribute _entityAttr;

        public Type EntityType { get; private set; }
        public String SourceName
        {
            get { return _entityAttr.SourceName; }
        }

        public FieldAttributeCollection Fields { get; set; }

        //public Dictionary<String, ReferenceAttribute> ReferenceFields { get; set; }


        protected EntityDescription()
        {
            Fields = new FieldAttributeCollection();
        }


        public EntityDescription(Type type) : this()
        {
            try
            {
                EntityType = type;

                //read Entity attribute
                object[] tAttrs = type.GetCustomAttributes(typeof(SQLEntityAttribute), true);
                _entityAttr = (SQLEntityAttribute)tAttrs[0];

                //find public properties
                foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Fields.AddField(p);
                }

                //find private properties
                foreach (PropertyInfo p in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    Fields.AddField(p);
                }
            }
            catch (Exception e)
            {
                throw new ORMException("Unable to create EntityDescription for " + type.Name, e);
            }
        }


    }

        
    

}