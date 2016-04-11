using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.TestSupport
{
    public class MockDataReader : System.Data.IDataReader
    {
        object _value; 
        public object Value
        {
            get
            {
                if (Value == null) { return DBNull.Value; }
                return _value;
            }
            set
            {
                Value = value;
            }
        }

        public bool Bool { get; set; }       

        public int  Int { get; set; }

        public long Long { get; set; }

        public Single Single { get; set; }

        public double Double { get; set; }

        public string String { get; set; }

        public DateTime DateTime { get; set; }

        public Guid Guid { get; set; }

        public bool HasValue { get { return _value != null; } }

        public MockDataReader()
        { }

        public object this[string name]
        {
            get
            {
                return Value;
            }
        }

        public object this[int i]
        {
            get
            {
                return Value;
            }
        }

        #region not implemented
        public int Depth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int FieldCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsClosed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int RecordsAffected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        public bool GetBoolean(int i)
        {
            return (Value != null) ? (bool)Value : Bool;
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (HasValue) ? (DateTime)Value : this.DateTime;
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            return (HasValue) ? (Double)Value : Double;
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            return (HasValue) ? (float)Value : Single;
        }

        public Guid GetGuid(int i)
        {
            return (HasValue) ? (Guid)Value : Guid;
        }

        public short GetInt16(int i)
        {
            return (HasValue) ? (short)Value : (short)Int;
        }

        public int GetInt32(int i)
        {
            return (HasValue) ? (short)Value : Int;
        }

        public long GetInt64(int i)
        {
            return (HasValue) ? (long)Value : Long;
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (HasValue) ? (string)Value : String;
        }

        public object GetValue(int i)
        {
            return Value;
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return Value == null;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }
    }
}
