//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml.Serialization;
//using System.ComponentModel;
//using System.Text.RegularExpressions;

//namespace CruiseDAL
//{
//    public abstract class ReadDataObject : INotifyPropertyChanged
//    {
//        #region Properties 
//        /// <summary>
//        /// Property returns the dataObject referenced. Useful when using data binding 
//        /// </summary>
//        [XmlIgnore]
//        public object Self
//        {
//            get
//            {
//                return this;
//            }
//        }

//        public virtual DatastoreBase DAL { get; set; }

//        /// <summary>
//        /// Tag allows any suplemental object to 
//        /// be atatched to a dataobject. 
//        /// </summary>
//        [XmlIgnore]
//        [Obsolete]
//        public Object Tag { get; set; }

        //[XmlIgnore]
        //public bool PropertyChangedEventsDisabled { get; protected set; }
        //#endregion 

        //public void SuspendEvents()
        //{
        //    PropertyChangedEventsDisabled = true;
        //}

        //public void ResumeEvents()
        //{
        //    PropertyChangedEventsDisabled = false;
        //}

//        #region INotifyPropertyChanged members 

//        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
//        protected void NotifyPropertyChanged(string name, object value)
//        {
//            NotifyPropertyChanged(name);
//        }

//        protected virtual void NotifyPropertyChanged(string name)
//        {
//            if (!PropertyChangedEventsDisabled)
//            {
//                //IsValidated = false;
//                if (PropertyChanged != null)
//                {
//                    PropertyChanged(this, new PropertyChangedEventArgs(name));
//                }
//            }
//        }
//        #endregion

//        #region IFormattable Members
//        /// <summary>
//        /// replaces placehoders with property values in format string. 
//        /// [propertyName], [propertyName:nullValue], [propertyName:nullValue:pad], [propertyName::pad] 
//        /// pad option can include prefix U | L | C . for Uppercase, lowercase, capitalize
//        /// </summary>
//        public string ToString(string format, IFormatProvider formatProvider)
//        {
//            if (String.IsNullOrEmpty(format))
//            {
//                return this.ToString();
//            }

//            //get a list of all propertie place holders in format
//            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"\[(?<prop>[a-zA-Z]\w+)(?:(?:\|)(?<ifnull>\w+))?(?:(?::)(?<pad>(?:[-]?\d+)?[ULC]?))?\]", RegexOptions.Compiled);

//            return rx.Replace(format, this.ProcessFormatElementMatch);
//        }

//        /// <summary>
//        /// helper method for ToString(String format, IFormatProvider fomatProvider)
//        /// </summary>
//        private string ProcessFormatElementMatch(System.Text.RegularExpressions.Match m)
//        {
//            string sValue = string.Empty;
//            string propName = m.Groups["prop"].Captures[0].Value;
//            string ifnull = string.Empty;
//            if (m.Groups["ifnull"].Captures.Count == 1)
//            {
//                ifnull = m.Groups["ifnull"].Captures[0].Value;
//            }

//            try
//            {
//                DataObjectInfo des = DatastoreBase.GetObjectDiscription(this.GetType());
//                Object value = des.Properties[propName]._getter.Invoke(this, null);
//                if (value != null && value is IFormattable)
//                {
//                    sValue = ((IFormattable)value).ToString(null, null);
//                }
//                else
//                {
//                    sValue = (value == null) ? ifnull : value.ToString();
//                }
//            }
//            catch
//            {
//                throw new FormatException("unable to resolve value for " + propName);
//            }


//            short pad;
//            char padOpt;
//            if (m.Groups["pad"].Captures.Count == 1)
//            {
//                try
//                {

//                    String sPad = m.Groups["pad"].Captures[0].Value;
//                    try
//                    {
//                        pad = short.Parse(sPad.TrimEnd('U', 'L', 'C', 'u', 'l', 'c'));
//                    }
//                    catch
//                    {
//                        pad = 0;
//                    }
//                    char last = char.ToUpper(sPad[sPad.Length - 1]);
//                    padOpt = (last == 'U' || last == 'L' || last == 'C') ? last : char.MinValue;

//                    switch (padOpt)
//                    {
//                        case 'U':
//                            {
//                                sValue = sValue.ToUpper();
//                                break;
//                            }
//                        case 'L':
//                            {
//                                sValue = sValue.ToLower();
//                                break;
//                            }
//                        case 'C':
//                            {
//                                sValue = CapitalizeString(sValue);
//                                break;
//                            }
//                    }

//                    if (pad < 0)
//                    {
//                        sValue = sValue.PadRight(pad * -1);
//                    }
//                    else if (pad > 0)
//                    {
//                        sValue = sValue.PadLeft(pad);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    throw new FormatException("Format element " + propName + " pad argument invalid", ex);
//                }
//            }
//            return sValue;
//        }

//        private static String CapitalizeString(String s)
//        {
//            char[] cArray = s.ToCharArray();
//            for (int i = 0; i < cArray.Length; i++)
//            {
//                if (Char.IsLetter(cArray[i]))
//                {
//                    cArray[i] = char.ToUpper(cArray[i]);
//                    break;
//                }
//            }
//            return new String(cArray);
//        }
//        #endregion
//    }
//}
