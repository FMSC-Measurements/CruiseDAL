using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityFormatter : ICustomFormatter
    {
        IEnumerable<PropertyInfo> _properties;

        public EntityFormatter(Type entityType)
        {
            _properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public string Format(string formatString, object obj, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(formatString))
            { return obj.ToString(); }

            //get a list of all properties place holders in format
            var rx = new Regex(@"\[(?<prop>[a-zA-Z]\w+)(?:(?::)(?<ifnull>\w+)?)?(?:(?::)(?<pad>(?:[-]?\d+)?[ULC]?))?\]", RegexOptions.Compiled);

            return rx.Replace(formatString, (Match m) =>
            {
                return ProcessFormatElementMatch(obj, m, formatProvider);
            });
        }

        /// <summary>
        /// helper method
        /// </summary>
        private string ProcessFormatElementMatch(object data, Match m, IFormatProvider formatProvider)
        {
            string sValue = string.Empty;
            string propName = m.Groups["prop"].Captures[0].Value;
            string ifnull = string.Empty;
            if (m.Groups["ifnull"].Captures.Count == 1)
            {
                ifnull = m.Groups["ifnull"].Captures[0].Value;
            }

            try
            {
                var property = _properties.Where(x => x.Name == propName).FirstOrDefault();

                object value = property?.GetValue(data, null);

                //Object value = _description.Fields[propName].GetFieldValue(data);
                if (value != null && value is IFormattable)
                {
                    sValue = ((IFormattable)value).ToString(null, formatProvider);
                }
                else
                {
                    sValue = (value == null) ? ifnull : value.ToString();
                }
            }
            catch
            {
                throw new FormatException("unable to resolve value for " + propName);
            }

            short pad;
            char padOpt;
            if (m.Groups["pad"].Captures.Count == 1)
            {
                try
                {
                    String sPad = m.Groups["pad"].Captures[0].Value;
                    try
                    {
                        pad = short.Parse(sPad.TrimEnd('U', 'L', 'C', 'u', 'l', 'c'));
                    }
                    catch
                    {
                        pad = 0;
                    }
                    char last = char.ToUpper(sPad[sPad.Length - 1]);
                    padOpt = (last == 'U' || last == 'L' || last == 'C') ? last : char.MinValue;

                    switch (padOpt)
                    {
                        case 'U':
                            {
                                sValue = sValue.ToUpper();
                                break;
                            }
                        case 'L':
                            {
                                sValue = sValue.ToLower();
                                break;
                            }
                        case 'C':
                            {
                                sValue = CapitalizeString(sValue);
                                break;
                            }
                    }

                    if (pad < 0)
                    {
                        sValue = sValue.PadRight(Math.Abs(pad));
                    }
                    else if (pad > 0)
                    {
                        sValue = sValue.PadLeft(pad);
                    }
                }
                catch (Exception ex)
                {
                    throw new FormatException("Format element " + propName + " pad argument invalid", ex);
                }
            }
            return sValue;
        }

        private static String CapitalizeString(String s)
        {
            char[] cArray = s.ToCharArray();
            for (int i = 0; i < cArray.Length; i++)
            {
                if (Char.IsLetter(cArray[i]))
                {
                    cArray[i] = char.ToUpper(cArray[i]);
                    break;
                }
            }
            return new String(cArray);
        }
    }
}