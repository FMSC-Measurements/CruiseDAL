using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSCORM.CodeGenEngine
{
    public class Field
    {
        public Field() { }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }

        public bool IsTransitionalPK { get; set; }

        public bool IsPK { get; set; }
        public bool IsAutoI { get; set; }
        public bool IsUnique { get; set; }
        public bool IsFK { get; set; }
        public string Ref { get; set; }
        public bool IsReq { get; set; }
        public bool NotNull { get; set; }

        public string DBType { get { return GetDBType(); } }
        public string DBDefault { get { return GetDBDefault(); } }
        
        //data object settings 
        public string AccessableType { get { return GetAccessableType(); } }
        public bool IsAccessableTypeEnum { get { return Type.StartsWith("ENUM:");} }
        public string DataObjectInitialValue { get { return this.GetDataObjectInitialValue(); } }
     
        //validation options 
        public Nullable<double> Min { get; set; }
        public Nullable<double> Max { get; set; }
        public string Values { get; set; }
        public string ErrorMessage { get; set; }

        public string Description { get; set; }

        public bool IsDepreciated { get; set; }
        public bool DontTrackChanges { get; set; }


        protected string GetAccessableType()
        {
            if (IsFK)
            {
                return "long?";
            }
            if (IsAccessableTypeEnum)
            {
                return "CruiseDAL.Enums." + Type.Substring("ENUM:".Length);//return the text after "ENUM:"
            }
            bool hasNullDefault = (Default == "null");
            switch (Type)
            {
                case "INTEGER":
                    {
                        if (hasNullDefault)
                        {
                            return "Int64?";
                        }
                        return "Int64";
                    }
                case "TEXT":
                    {
                        return "String";
                    }
                case "REAL":
                    {
                        if (hasNullDefault)
                        {
                            return "float?";
                        }
                        return "float";
                    }
                case "DOUBLE":
                    {
                        if (hasNullDefault)
                        {
                            return "Double?";
                        }
                        return "Double";
                    }
                case "DATETIME":
                    { return "String"; }
                case "BOOLEAN": { return "bool"; }
                case "BLOB": { return "byte[]"; }
                case "GUID": { return "Guid"; }

            }
            return null;


        }


        protected string GetDataObjectInitialValue()
        {
            if (IsTransitionalPK)
            {
                return null;// "Guid.NewGuid().ToString()";
            }

            if (IsAccessableTypeEnum) { return "0"; }
            else if (IsFK || String.IsNullOrEmpty(DBDefault)) { return null; }



            if (Default == "null")
            {
                return null;
            }
            else if (!string.IsNullOrEmpty(Default))
            {
                if (this.AccessableType.ToLower() == "string")
                {
                    return "\"" + Default + "\"";
                }
                else
                {
                    return Default;
                }

            }
            switch (this.AccessableType.ToLower())
            {
                case "int?":
                case "int": { return "0L"; }
                case "string": { return null; }
                case "float?":
                case "float": { return "0.0f"; }
                case "datetime": { return null; }
                case "bool?":
                case "bool": { return "false"; }
                case "byte[]": { return null; }
                case "double": { return "0.0"; }
                case "guid": { return "Guid.Empty"; }
            }
            return null; 
        }

        protected string GetDBDefault()
        {
            if(IsFK)  { return null; }
            if (IsAccessableTypeEnum) { return "0"; }
            if(!string.IsNullOrEmpty(Default))
            {
                if (Default == "null")
                {
                    return null;
                }
                else if( Type == "TEXT" )
                {
                    return "'" + Default + "'";
                }
                
            }
            switch (Type)
            {
                case "INTEGER": { return "0"; }
                case "REAL": { return "0.0"; }
                case "BOOLEAN": { return "0"; }
                case "DOUBLE": { return "0.0"; }
            }
            return null;
        }

        protected string GetDBType()
        {
            if (Type.StartsWith("ENUM:"))
            {
                return "TEXT";
            }
            else if (Type == "GUID")
            {
                return "TEXT";
            }
            return Type;
        }

    }//end class Field
}
