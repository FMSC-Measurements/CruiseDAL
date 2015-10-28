using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL
{
    public class EntityFormatter : ICustomFormatter
    {
        EntityDescription _description;

        public EntityFormatter(EntityDescription description)
        {
            _description = description;
        }

        public string Format(string formatString, object obj, IFormatProvider formatProvider)
        {
            if(string.IsNullOrEmpty(formatString))
            { return obj.ToString(); }

            //get a list of all propertie place holders in format
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"\[(?<prop>[a-zA-Z]\w+)(?:(?:\|)(?<ifnull>\w+))?(?:(?::)(?<pad>(?:[-]?\d+)?[ULC]?))?\]", RegexOptions.Compiled);

            return rx.Replace(formatString, this.ProcessFormatElementMatch);
        }

        /// <summary>
        /// helper method for ToString(String format, IFormatProvider fomatProvider)
        /// </summary>
        private string ProcessFormatElementMatch(Match m)
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
                Object value = _description.Fields[propName].GetFieldValue(this);
                if (value != null && value is IFormattable)
                {
                    sValue = ((IFormattable)value).ToString(null, null);
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
