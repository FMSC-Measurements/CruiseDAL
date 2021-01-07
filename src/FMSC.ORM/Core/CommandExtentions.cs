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
#if !LEGACY
                int counter = 1;
#else 
                var commandText = System.Text.RegularExpressions.Regex.Replace(command.CommandText, "@p\\d+", "?");
                command.CommandText = commandText;

#endif
                foreach (var value in paramArgs)
                {
                    var param = command.CreateParameter();
#if LEGACY
                    //legacy behavior: leave paramater name blank 
#else
                    param.ParameterName = "@p" + counter++.ToString();
#endif

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

#if DEBUG
            var cmdText = cmd.CommandText;
            var prms = cmd.Parameters.OfType<IDbDataParameter>().ToArray();
            foreach (var match in System.Text.RegularExpressions.Regex.Matches(cmdText, "[@]\\w+").OfType<System.Text.RegularExpressions.Match>())
            {
                if (prms.Any(x => x.ParameterName == match.Value) == false)
                {
                    throw new InvalidOperationException(match.Value + " is missing");
                }
            }
#endif

            //var cmdText = cmd.CommandText;
            //var prms = cmd.Parameters.OfType<IDbDataParameter>().ToArray();
            //foreach (var match in System.Text.RegularExpressions.Regex.Matches(cmdText, "[@]\\w+").OfType<System.Text.RegularExpressions.Match>())
            //{
            //    var name = match.Value;
            //    var pName = name.TrimStart('@');
            //    var param = cmd.CreateParameter();
            //    param.ParameterName = name;
            //    param.Value = data.GetType().GetProperty(pName).GetValue(data, (object[])null) ?? DBNull.Value;

            //    cmd.Parameters.Add(param);
            //}
        }

        public static IEnumerable<string> GetPropNames(object obj)
        {
            return obj.GetType().GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Select(p => p.Name);
        }
    }
}