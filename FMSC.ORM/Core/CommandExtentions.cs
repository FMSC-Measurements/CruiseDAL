using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FMSC.ORM.Core
{
    public static class CommandExtentions
    {
        public static void SetParams(this IDbCommand command, object[] paramArgs)
        {
            if (paramArgs != null)
            {
                int counter = 1; 
                foreach (var value in paramArgs)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = "@p" + counter++.ToString();

                    param.Value = value ?? DBNull.Value;

                    command.Parameters.Add(param);
                }
            }
        }

        public static void AddParams(this IDbCommand cmd, object data)
        {
            if (cmd == null) { throw new ArgumentNullException("cmd"); }
            if (data == null) { return; }

            var propNames = GetPropNames(data);

            foreach (var name in propNames)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = "@" + name;
                param.Value = data.GetType().GetProperty(name).GetValue(data, (object[])null) ?? DBNull.Value;

                cmd.Parameters.Add(param);
            }
        }

        public static IEnumerable<string> GetPropNames(object obj)
        {
            return obj.GetType().GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Select(p => p.Name);
        }
    }
}