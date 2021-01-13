using FMSC.ORM.Util;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.EntityModel.Support
{
    public interface IInflatorLookup
    {
        EntityInflator GetEntityInflator(IDataReader reader);
    }

    public class InflatorLookup : IInflatorLookup
    {
        private static IInflatorLookup _instance;

        public static IInflatorLookup Instance
        {
            get => _instance ?? (_instance = new InflatorLookup());
            set => _instance = value;
        }

        private IDictionary<int, EntityInflator> _lookup = new Dictionary<int, EntityInflator>();

        public EntityInflator GetEntityInflator(IDataReader reader)
        {
            var fieldHash = GetFieldHash(reader);
            if (_lookup.ContainsKey(fieldHash))
            {
                return _lookup[fieldHash];
            }
            else
            {
                var inflator = new EntityInflator(reader);
                _lookup.Add(fieldHash, inflator);
                return inflator;
            }
        }

        public static int GetFieldHash(IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            var fields = new string[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                var fieldName = reader.GetName(i).ToLower(System.Globalization.CultureInfo.InvariantCulture);
                fields[i] = fieldName;
            }
            var hash = fields.CombineHashs();
            return hash;
        }
    }
}