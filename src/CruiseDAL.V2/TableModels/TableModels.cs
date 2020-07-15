
#pragma warning disable RECS0001 //disable warning that class is partial with only one part

using System;
using System.Xml.Serialization;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel;
using FMSC.ORM.Core;
using CruiseDAL.Schema;
using CruiseDAL.EntityAttributes;

namespace CruiseDAL.DataObjects
{
    #region Core Tables
    [Table("Sale")]
    public partial class SaleDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static SaleDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("SaleNumber", "Sale", "SaleNumber is Required"));
            _validator.Add(new FieldValidator("Region", "Sale", "Invalid Region", double.MinValue, double.MinValue, true, "01 02 03 04 05 06 07 08 09 10 11 12"));
            _validator.Add(new NotNullRule("Forest", "Sale", "Forest is Required"));
            _validator.Add(new NotNullRule("District", "Sale", "District is Required"));
            _validator.Add(new FieldValidator("CalendarYear", "Sale", "Year Out of Range", 1000, 9999, false, null));
        }

        public SaleDO() { }

        public SaleDO(SaleDO obj) : this()
        {
            SetValues(obj);
        }

        public SaleDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Sale_CN")]
        public Int64? Sale_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _salenumber;
        [XmlElement]
        [Field(Name = "SaleNumber")]
        public virtual String SaleNumber
        {
            get
            {
                return _salenumber;
            }
            set
            {
                if (_salenumber == value) { return; }
                _salenumber = value;
                this.ValidateProperty(SALE.SALENUMBER, _salenumber);
                this.NotifyPropertyChanged(SALE.SALENUMBER);
            }
        }
        private String _name;
        [XmlElement]
        [Field(Name = "Name")]
        public virtual String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) { return; }
                _name = value;
                this.ValidateProperty(SALE.NAME, _name);
                this.NotifyPropertyChanged(SALE.NAME);
            }
        }
        private String _purpose;
        [XmlElement]
        [Field(Name = "Purpose")]
        public virtual String Purpose
        {
            get
            {
                return _purpose;
            }
            set
            {
                if (_purpose == value) { return; }
                _purpose = value;
                this.ValidateProperty(SALE.PURPOSE, _purpose);
                this.NotifyPropertyChanged(SALE.PURPOSE);
            }
        }
        private String _region;
        [XmlElement]
        [Field(Name = "Region")]
        public virtual String Region
        {
            get
            {
                return _region;
            }
            set
            {
                if (_region == value) { return; }
                _region = value;
                this.ValidateProperty(SALE.REGION, _region);
                this.NotifyPropertyChanged(SALE.REGION);
            }
        }
        private String _forest;
        [XmlElement]
        [Field(Name = "Forest")]
        public virtual String Forest
        {
            get
            {
                return _forest;
            }
            set
            {
                if (_forest == value) { return; }
                _forest = value;
                this.ValidateProperty(SALE.FOREST, _forest);
                this.NotifyPropertyChanged(SALE.FOREST);
            }
        }
        private String _district;
        [XmlElement]
        [Field(Name = "District")]
        public virtual String District
        {
            get
            {
                return _district;
            }
            set
            {
                if (_district == value) { return; }
                _district = value;
                this.ValidateProperty(SALE.DISTRICT, _district);
                this.NotifyPropertyChanged(SALE.DISTRICT);
            }
        }
        private String _measurementyear;
        [XmlElement]
        [Field(Name = "MeasurementYear")]
        public virtual String MeasurementYear
        {
            get
            {
                return _measurementyear;
            }
            set
            {
                if (_measurementyear == value) { return; }
                _measurementyear = value;
                this.ValidateProperty(SALE.MEASUREMENTYEAR, _measurementyear);
                this.NotifyPropertyChanged(SALE.MEASUREMENTYEAR);
            }
        }
        private Int64 _calendaryear;
        [XmlElement]
        [Field(Name = "CalendarYear")]
        public virtual Int64 CalendarYear
        {
            get
            {
                return _calendaryear;
            }
            set
            {
                if (_calendaryear == value) { return; }
                _calendaryear = value;
                this.ValidateProperty(SALE.CALENDARYEAR, _calendaryear);
                this.NotifyPropertyChanged(SALE.CALENDARYEAR);
            }
        }
        private bool _loggradingenabled = false;
        [XmlElement]
        [Field(Name = "LogGradingEnabled")]
        public virtual bool LogGradingEnabled
        {
            get
            {
                return _loggradingenabled;
            }
            set
            {
                if (_loggradingenabled == value) { return; }
                _loggradingenabled = value;
                this.ValidateProperty(SALE.LOGGRADINGENABLED, _loggradingenabled);
                this.NotifyPropertyChanged(SALE.LOGGRADINGENABLED);
            }
        }
        private String _remarks;
        [XmlElement]
        [Field(Name = "Remarks")]
        public virtual String Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                if (_remarks == value) { return; }
                _remarks = value;
                this.ValidateProperty(SALE.REMARKS, _remarks);
                this.NotifyPropertyChanged(SALE.REMARKS);
            }
        }
        private String _defaultuom;
        [XmlElement]
        [Field(Name = "DefaultUOM")]
        public virtual String DefaultUOM
        {
            get
            {
                return _defaultuom;
            }
            set
            {
                if (_defaultuom == value) { return; }
                _defaultuom = value;
                this.ValidateProperty(SALE.DEFAULTUOM, _defaultuom);
                this.NotifyPropertyChanged(SALE.DEFAULTUOM);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("SaleNumber", this.SaleNumber) && isValid;
            isValid = ValidateProperty("Name", this.Name) && isValid;
            isValid = ValidateProperty("Purpose", this.Purpose) && isValid;
            isValid = ValidateProperty("Region", this.Region) && isValid;
            isValid = ValidateProperty("Forest", this.Forest) && isValid;
            isValid = ValidateProperty("District", this.District) && isValid;
            isValid = ValidateProperty("MeasurementYear", this.MeasurementYear) && isValid;
            isValid = ValidateProperty("CalendarYear", this.CalendarYear) && isValid;
            isValid = ValidateProperty("LogGradingEnabled", this.LogGradingEnabled) && isValid;
            isValid = ValidateProperty("Remarks", this.Remarks) && isValid;
            isValid = ValidateProperty("DefaultUOM", this.DefaultUOM) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as SaleDO);
        }

        public void SetValues(SaleDO obj)
        {
            if (obj == null) { return; }
            SaleNumber = obj.SaleNumber;
            Name = obj.Name;
            Purpose = obj.Purpose;
            Region = obj.Region;
            Forest = obj.Forest;
            District = obj.District;
            MeasurementYear = obj.MeasurementYear;
            CalendarYear = obj.CalendarYear;
            LogGradingEnabled = obj.LogGradingEnabled;
            Remarks = obj.Remarks;
            DefaultUOM = obj.DefaultUOM;
        }
    }
    [Table("CuttingUnit")]
    public partial class CuttingUnitDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static CuttingUnitDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Code", "CuttingUnit", "Code is Required"));
            _validator.Add(new FieldValidator("Area", "CuttingUnit", "Area Out of Range", 0, 9999999, true, null));
        }

        public CuttingUnitDO() { }

        public CuttingUnitDO(CuttingUnitDO obj) : this()
        {
            SetValues(obj);
        }

        public CuttingUnitDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "CuttingUnit_CN")]
        public Int64? CuttingUnit_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(CUTTINGUNIT.CODE, _code);
                this.NotifyPropertyChanged(CUTTINGUNIT.CODE);
            }
        }
        private float _area = 0.0f;
        [XmlElement]
        [Field(Name = "Area")]
        public virtual float Area
        {
            get
            {
                return _area;
            }
            set
            {
                if (Math.Abs(_area - value) < float.Epsilon) { return; }
                _area = value;
                this.ValidateProperty(CUTTINGUNIT.AREA, _area);
                this.NotifyPropertyChanged(CUTTINGUNIT.AREA);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(CUTTINGUNIT.DESCRIPTION, _description);
                this.NotifyPropertyChanged(CUTTINGUNIT.DESCRIPTION);
            }
        }
        private String _loggingmethod;
        [XmlElement]
        [Field(Name = "LoggingMethod")]
        public virtual String LoggingMethod
        {
            get
            {
                return _loggingmethod;
            }
            set
            {
                if (_loggingmethod == value) { return; }
                _loggingmethod = value;
                this.ValidateProperty(CUTTINGUNIT.LOGGINGMETHOD, _loggingmethod);
                this.NotifyPropertyChanged(CUTTINGUNIT.LOGGINGMETHOD);
            }
        }
        private String _paymentunit;
        [XmlElement]
        [Field(Name = "PaymentUnit")]
        public virtual String PaymentUnit
        {
            get
            {
                return _paymentunit;
            }
            set
            {
                if (_paymentunit == value) { return; }
                _paymentunit = value;
                this.ValidateProperty(CUTTINGUNIT.PAYMENTUNIT, _paymentunit);
                this.NotifyPropertyChanged(CUTTINGUNIT.PAYMENTUNIT);
            }
        }
        private String _tallyhistory;
        [XmlElement]
        [Field(Name = "TallyHistory")]
        public virtual String TallyHistory
        {
            get
            {
                return _tallyhistory;
            }
            set
            {
                if (_tallyhistory == value) { return; }
                _tallyhistory = value;
                this.ValidateProperty(CUTTINGUNIT.TALLYHISTORY, _tallyhistory);
                this.NotifyPropertyChanged(CUTTINGUNIT.TALLYHISTORY);
            }
        }
        private String _rx;
        [XmlElement]
        [Field(Name = "Rx")]
        public virtual String Rx
        {
            get
            {
                return _rx;
            }
            set
            {
                if (_rx == value) { return; }
                _rx = value;
                this.ValidateProperty(CUTTINGUNIT.RX, _rx);
                this.NotifyPropertyChanged(CUTTINGUNIT.RX);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("Area", this.Area) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("LoggingMethod", this.LoggingMethod) && isValid;
            isValid = ValidateProperty("PaymentUnit", this.PaymentUnit) && isValid;
            isValid = ValidateProperty("TallyHistory", this.TallyHistory) && isValid;
            isValid = ValidateProperty("Rx", this.Rx) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as CuttingUnitDO);
        }

        public void SetValues(CuttingUnitDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            Area = obj.Area;
            Description = obj.Description;
            LoggingMethod = obj.LoggingMethod;
            PaymentUnit = obj.PaymentUnit;
            TallyHistory = obj.TallyHistory;
            Rx = obj.Rx;
        }
    }
    [Table("Stratum")]
    public partial class StratumDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static StratumDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Code", "Stratum", "Code is Required"));
            _validator.Add(new NotNullRule("Method", "Stratum", "Method is Required"));
            _validator.Add(new FieldValidator("BasalAreaFactor", "Stratum", "BAF Out of Range", 0, 99.9899978637695, false, null));
            _validator.Add(new FieldValidator("FixedPlotSize", "Stratum", "Fixed Plot Size Out of Range", 0, 9999, false, null));
            _validator.Add(new FieldValidator("Month", "Stratum", "Month Out of Range", 1, 12, false, null));
            _validator.Add(new FieldValidator("Year", "Stratum", "Year Out of Range", 1900, 2199, false, null));
        }

        public StratumDO() { }

        public StratumDO(StratumDO obj) : this()
        {
            SetValues(obj);
        }

        public StratumDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Stratum_CN")]
        public Int64? Stratum_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(STRATUM.CODE, _code);
                this.NotifyPropertyChanged(STRATUM.CODE);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(STRATUM.DESCRIPTION, _description);
                this.NotifyPropertyChanged(STRATUM.DESCRIPTION);
            }
        }
        private String _method;
        [XmlElement]
        [Field(Name = "Method")]
        public virtual String Method
        {
            get
            {
                return _method;
            }
            set
            {
                if (_method == value) { return; }
                _method = value;
                this.ValidateProperty(STRATUM.METHOD, _method);
                this.NotifyPropertyChanged(STRATUM.METHOD);
            }
        }
        private float _basalareafactor = 0.0f;
        [XmlElement]
        [Field(Name = "BasalAreaFactor")]
        public virtual float BasalAreaFactor
        {
            get
            {
                return _basalareafactor;
            }
            set
            {
                if (Math.Abs(_basalareafactor - value) < float.Epsilon) { return; }
                _basalareafactor = value;
                this.ValidateProperty(STRATUM.BASALAREAFACTOR, _basalareafactor);
                this.NotifyPropertyChanged(STRATUM.BASALAREAFACTOR);
            }
        }
        private float _fixedplotsize = 0.0f;
        [XmlElement]
        [Field(Name = "FixedPlotSize")]
        public virtual float FixedPlotSize
        {
            get
            {
                return _fixedplotsize;
            }
            set
            {
                if (Math.Abs(_fixedplotsize - value) < float.Epsilon) { return; }
                _fixedplotsize = value;
                this.ValidateProperty(STRATUM.FIXEDPLOTSIZE, _fixedplotsize);
                this.NotifyPropertyChanged(STRATUM.FIXEDPLOTSIZE);
            }
        }
        private Int64 _kz3ppnt;
        [XmlElement]
        [Field(Name = "KZ3PPNT")]
        public virtual Int64 KZ3PPNT
        {
            get
            {
                return _kz3ppnt;
            }
            set
            {
                if (_kz3ppnt == value) { return; }
                _kz3ppnt = value;
                this.ValidateProperty(STRATUM.KZ3PPNT, _kz3ppnt);
                this.NotifyPropertyChanged(STRATUM.KZ3PPNT);
            }
        }
        private Int64 _samplingfrequency;
        [XmlElement]
        [Field(Name = "SamplingFrequency")]
        public virtual Int64 SamplingFrequency
        {
            get
            {
                return _samplingfrequency;
            }
            set
            {
                if (_samplingfrequency == value) { return; }
                _samplingfrequency = value;
                this.ValidateProperty(STRATUM.SAMPLINGFREQUENCY, _samplingfrequency);
                this.NotifyPropertyChanged(STRATUM.SAMPLINGFREQUENCY);
            }
        }
        private String _hotkey;
        [XmlElement]
        [Field(Name = "Hotkey")]
        public virtual String Hotkey
        {
            get
            {
                return _hotkey;
            }
            set
            {
                if (_hotkey == value) { return; }
                _hotkey = value;
                this.ValidateProperty(STRATUM.HOTKEY, _hotkey);
                this.NotifyPropertyChanged(STRATUM.HOTKEY);
            }
        }
        private String _fbscode;
        [XmlElement]
        [Field(Name = "FBSCode")]
        public virtual String FBSCode
        {
            get
            {
                return _fbscode;
            }
            set
            {
                if (_fbscode == value) { return; }
                _fbscode = value;
                this.ValidateProperty(STRATUM.FBSCODE, _fbscode);
                this.NotifyPropertyChanged(STRATUM.FBSCODE);
            }
        }
        private String _yieldcomponent = "CL";
        [XmlElement]
        [Field(Name = "YieldComponent")]
        public virtual String YieldComponent
        {
            get
            {
                return _yieldcomponent;
            }
            set
            {
                if (_yieldcomponent == value) { return; }
                _yieldcomponent = value;
                this.ValidateProperty(STRATUM.YIELDCOMPONENT, _yieldcomponent);
                this.NotifyPropertyChanged(STRATUM.YIELDCOMPONENT);
            }
        }
        private float _volumefactor = 0.333F;
        [XmlElement]
        [Field(Name = "VolumeFactor")]
        public virtual float VolumeFactor
        {
            get
            {
                return _volumefactor;
            }
            set
            {
                if (Math.Abs(_volumefactor - value) < float.Epsilon) { return; }
                _volumefactor = value;
                this.ValidateProperty(STRATUM.VOLUMEFACTOR, _volumefactor);
                this.NotifyPropertyChanged(STRATUM.VOLUMEFACTOR);
            }
        }
        private Int64 _month;
        [XmlElement]
        [Field(Name = "Month")]
        public virtual Int64 Month
        {
            get
            {
                return _month;
            }
            set
            {
                if (_month == value) { return; }
                _month = value;
                this.ValidateProperty(STRATUM.MONTH, _month);
                this.NotifyPropertyChanged(STRATUM.MONTH);
            }
        }
        private Int64 _year;
        [XmlElement]
        [Field(Name = "Year")]
        public virtual Int64 Year
        {
            get
            {
                return _year;
            }
            set
            {
                if (_year == value) { return; }
                _year = value;
                this.ValidateProperty(STRATUM.YEAR, _year);
                this.NotifyPropertyChanged(STRATUM.YEAR);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("Method", this.Method) && isValid;
            isValid = ValidateProperty("BasalAreaFactor", this.BasalAreaFactor) && isValid;
            isValid = ValidateProperty("FixedPlotSize", this.FixedPlotSize) && isValid;
            isValid = ValidateProperty("KZ3PPNT", this.KZ3PPNT) && isValid;
            isValid = ValidateProperty("SamplingFrequency", this.SamplingFrequency) && isValid;
            isValid = ValidateProperty("Hotkey", this.Hotkey) && isValid;
            isValid = ValidateProperty("FBSCode", this.FBSCode) && isValid;
            isValid = ValidateProperty("YieldComponent", this.YieldComponent) && isValid;
            isValid = ValidateProperty("VolumeFactor", this.VolumeFactor) && isValid;
            isValid = ValidateProperty("Month", this.Month) && isValid;
            isValid = ValidateProperty("Year", this.Year) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as StratumDO);
        }

        public void SetValues(StratumDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            Description = obj.Description;
            Method = obj.Method;
            BasalAreaFactor = obj.BasalAreaFactor;
            FixedPlotSize = obj.FixedPlotSize;
            KZ3PPNT = obj.KZ3PPNT;
            SamplingFrequency = obj.SamplingFrequency;
            Hotkey = obj.Hotkey;
            FBSCode = obj.FBSCode;
            YieldComponent = obj.YieldComponent;
            VolumeFactor = obj.VolumeFactor;
            Month = obj.Month;
            Year = obj.Year;
        }
    }
    [Table("CuttingUnitStratum")]
    public partial class CuttingUnitStratumDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static CuttingUnitStratumDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("CuttingUnit_CN", "CuttingUnitStratum", "CuttingUnit_CN is Required"));
            _validator.Add(new NotNullRule("Stratum_CN", "CuttingUnitStratum", "Stratum_CN is Required"));
        }

        public CuttingUnitStratumDO() { }

        public CuttingUnitStratumDO(CuttingUnitStratumDO obj) : this()
        {
            SetValues(obj);
        }

        public CuttingUnitStratumDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _cuttingunit_cn;
        [XmlIgnore]
        [Field(Name = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN
        {
            get
            {

                if (_cuttingunit != null)
                {
                    return _cuttingunit.CuttingUnit_CN;
                }
                return _cuttingunit_cn;
            }
            set
            {
                if (_cuttingunit_cn == value) { return; }
                if (value == null || value.Value == 0) { _cuttingunit = null; }
                _cuttingunit_cn = value;
                this.ValidateProperty(CUTTINGUNITSTRATUM.CUTTINGUNIT_CN, _cuttingunit_cn);
                this.NotifyPropertyChanged(CUTTINGUNITSTRATUM.CUTTINGUNIT_CN);
            }
        }
        public virtual CuttingUnitDO GetCuttingUnit()
        {
            if (DAL == null) { return null; }
            if (this.CuttingUnit_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<CuttingUnitDO>(this.CuttingUnit_CN);
            //return DAL.ReadSingleRow<CuttingUnitDO>(CUTTINGUNIT._NAME, this.CuttingUnit_CN);
        }

        private CuttingUnitDO _cuttingunit = null;
        [XmlIgnore]
        public CuttingUnitDO CuttingUnit
        {
            get
            {
                if (_cuttingunit == null)
                {
                    _cuttingunit = GetCuttingUnit();
                }
                return _cuttingunit;
            }
            set
            {
                if (_cuttingunit == value) { return; }
                _cuttingunit = value;
                CuttingUnit_CN = (value != null) ? value.CuttingUnit_CN : null;
            }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(CUTTINGUNITSTRATUM.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(CUTTINGUNITSTRATUM.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private float? _stratumarea;
        [XmlElement]
        [Field(Name = "StratumArea")]
        public virtual float? StratumArea
        {
            get
            {
                return _stratumarea;
            }
            set
            {
                if (_stratumarea == value) { return; }
                _stratumarea = value;
                this.ValidateProperty(CUTTINGUNITSTRATUM.STRATUMAREA, _stratumarea);
                this.NotifyPropertyChanged(CUTTINGUNITSTRATUM.STRATUMAREA);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("StratumArea", this.StratumArea) && isValid;
            isValid = ValidateProperty("CuttingUnit_CN", this.CuttingUnit_CN) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as CuttingUnitStratumDO);
        }

        public void SetValues(CuttingUnitStratumDO obj)
        {
            if (obj == null) { return; }
            StratumArea = obj.StratumArea;
        }
    }
    [Table("SampleGroup")]
    public partial class SampleGroupDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static SampleGroupDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "SampleGroup", "Stratum_CN is Required"));
            _validator.Add(new NotNullRule("Code", "SampleGroup", "Code is Required"));
            _validator.Add(new FieldValidator("CutLeave", "SampleGroup", "Invalid Cut Leave Value", double.MinValue, double.MinValue, true, "C L"));
            _validator.Add(new FieldValidator("UOM", "SampleGroup", "Invalid UOM ", double.MinValue, double.MinValue, true, "01 02 03 04 05"));
            _validator.Add(new NotNullRule("PrimaryProduct", "SampleGroup", "PrimaryProduct is Required"));
            _validator.Add(new FieldValidator("SamplingFrequency", "SampleGroup", "Frequency Must Be a Positive Number", 0, double.MinValue, false, null));
            _validator.Add(new FieldValidator("InsuranceFrequency", "SampleGroup", "Frequency Must Be a Positive Number", 0, double.MinValue, false, null));
            _validator.Add(new FieldValidator("KZ", "SampleGroup", "KZ Must Be a Positive Number", 0, double.MinValue, false, null));
            _validator.Add(new FieldValidator("BigBAF", "SampleGroup", "BigBAG Must Be a Postive Number", 0, double.MinValue, false, null));
        }

        public SampleGroupDO() { }

        public SampleGroupDO(SampleGroupDO obj) : this()
        {
            SetValues(obj);
        }

        public SampleGroupDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "SampleGroup_CN")]
        public Int64? SampleGroup_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(SAMPLEGROUP.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(SAMPLEGROUP.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(SAMPLEGROUP.CODE, _code);
                this.NotifyPropertyChanged(SAMPLEGROUP.CODE);
            }
        }
        private String _cutleave;
        [XmlElement]
        [Field(Name = "CutLeave")]
        public virtual String CutLeave
        {
            get
            {
                return _cutleave;
            }
            set
            {
                if (_cutleave == value) { return; }
                _cutleave = value;
                this.ValidateProperty(SAMPLEGROUP.CUTLEAVE, _cutleave);
                this.NotifyPropertyChanged(SAMPLEGROUP.CUTLEAVE);
            }
        }
        private String _uom;
        [XmlElement]
        [Field(Name = "UOM")]
        public virtual String UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                if (_uom == value) { return; }
                _uom = value;
                this.ValidateProperty(SAMPLEGROUP.UOM, _uom);
                this.NotifyPropertyChanged(SAMPLEGROUP.UOM);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(SAMPLEGROUP.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(SAMPLEGROUP.PRIMARYPRODUCT);
            }
        }
        private String _secondaryproduct;
        [XmlElement]
        [Field(Name = "SecondaryProduct")]
        public virtual String SecondaryProduct
        {
            get
            {
                return _secondaryproduct;
            }
            set
            {
                if (_secondaryproduct == value) { return; }
                _secondaryproduct = value;
                this.ValidateProperty(SAMPLEGROUP.SECONDARYPRODUCT, _secondaryproduct);
                this.NotifyPropertyChanged(SAMPLEGROUP.SECONDARYPRODUCT);
            }
        }
        private String _biomassproduct;
        [XmlElement]
        [Field(Name = "BiomassProduct")]
        public virtual String BiomassProduct
        {
            get
            {
                return _biomassproduct;
            }
            set
            {
                if (_biomassproduct == value) { return; }
                _biomassproduct = value;
                this.ValidateProperty(SAMPLEGROUP.BIOMASSPRODUCT, _biomassproduct);
                this.NotifyPropertyChanged(SAMPLEGROUP.BIOMASSPRODUCT);
            }
        }
        private String _defaultlivedead;
        [XmlElement]
        [Field(Name = "DefaultLiveDead")]
        public virtual String DefaultLiveDead
        {
            get
            {
                return _defaultlivedead;
            }
            set
            {
                if (_defaultlivedead == value) { return; }
                _defaultlivedead = value;
                this.ValidateProperty(SAMPLEGROUP.DEFAULTLIVEDEAD, _defaultlivedead);
                this.NotifyPropertyChanged(SAMPLEGROUP.DEFAULTLIVEDEAD);
            }
        }
        private Int64 _samplingfrequency;
        [XmlElement]
        [Field(Name = "SamplingFrequency")]
        public virtual Int64 SamplingFrequency
        {
            get
            {
                return _samplingfrequency;
            }
            set
            {
                if (_samplingfrequency == value) { return; }
                _samplingfrequency = value;
                this.ValidateProperty(SAMPLEGROUP.SAMPLINGFREQUENCY, _samplingfrequency);
                this.NotifyPropertyChanged(SAMPLEGROUP.SAMPLINGFREQUENCY);
            }
        }
        private Int64 _insurancefrequency;
        [XmlElement]
        [Field(Name = "InsuranceFrequency")]
        public virtual Int64 InsuranceFrequency
        {
            get
            {
                return _insurancefrequency;
            }
            set
            {
                if (_insurancefrequency == value) { return; }
                _insurancefrequency = value;
                this.ValidateProperty(SAMPLEGROUP.INSURANCEFREQUENCY, _insurancefrequency);
                this.NotifyPropertyChanged(SAMPLEGROUP.INSURANCEFREQUENCY);
            }
        }
        private Int64 _kz;
        [XmlElement]
        [Field(Name = "KZ")]
        public virtual Int64 KZ
        {
            get
            {
                return _kz;
            }
            set
            {
                if (_kz == value) { return; }
                _kz = value;
                this.ValidateProperty(SAMPLEGROUP.KZ, _kz);
                this.NotifyPropertyChanged(SAMPLEGROUP.KZ);
            }
        }
        private float _bigbaf = 0.0f;
        [XmlElement]
        [Field(Name = "BigBAF")]
        public virtual float BigBAF
        {
            get
            {
                return _bigbaf;
            }
            set
            {
                if (Math.Abs(_bigbaf - value) < float.Epsilon) { return; }
                _bigbaf = value;
                this.ValidateProperty(SAMPLEGROUP.BIGBAF, _bigbaf);
                this.NotifyPropertyChanged(SAMPLEGROUP.BIGBAF);
            }
        }
        private float _smallfps = 0.0f;
        [XmlElement]
        [Field(Name = "SmallFPS")]
        public virtual float SmallFPS
        {
            get
            {
                return _smallfps;
            }
            set
            {
                if (Math.Abs(_smallfps - value) < float.Epsilon) { return; }
                _smallfps = value;
                this.ValidateProperty(SAMPLEGROUP.SMALLFPS, _smallfps);
                this.NotifyPropertyChanged(SAMPLEGROUP.SMALLFPS);
            }
        }
        private CruiseDAL.Enums.TallyMode _tallymethod = 0;
        [XmlElement]
        [Field(Name = "TallyMethod")]
        public virtual CruiseDAL.Enums.TallyMode TallyMethod
        {
            get
            {
                return _tallymethod;
            }
            set
            {
                if (_tallymethod == value) { return; }
                _tallymethod = value;
                this.ValidateProperty(SAMPLEGROUP.TALLYMETHOD, _tallymethod);
                this.NotifyPropertyChanged(SAMPLEGROUP.TALLYMETHOD);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(SAMPLEGROUP.DESCRIPTION, _description);
                this.NotifyPropertyChanged(SAMPLEGROUP.DESCRIPTION);
            }
        }
        private String _sampleselectortype;
        [XmlElement]
        [Field(Name = "SampleSelectorType")]
        public virtual String SampleSelectorType
        {
            get
            {
                return _sampleselectortype;
            }
            set
            {
                if (_sampleselectortype == value) { return; }
                _sampleselectortype = value;
                this.ValidateProperty(SAMPLEGROUP.SAMPLESELECTORTYPE, _sampleselectortype);
                this.NotifyPropertyChanged(SAMPLEGROUP.SAMPLESELECTORTYPE);
            }
        }
        private String _sampleselectorstate;
        [XmlElement]
        [Field(Name = "SampleSelectorState")]
        public virtual String SampleSelectorState
        {
            get
            {
                return _sampleselectorstate;
            }
            set
            {
                if (_sampleselectorstate == value) { return; }
                _sampleselectorstate = value;
                this.ValidateProperty(SAMPLEGROUP.SAMPLESELECTORSTATE, _sampleselectorstate);
                this.NotifyPropertyChanged(SAMPLEGROUP.SAMPLESELECTORSTATE);
            }
        }
        private Int64 _minkpi;
        [XmlElement]
        [Field(Name = "MinKPI")]
        public virtual Int64 MinKPI
        {
            get
            {
                return _minkpi;
            }
            set
            {
                if (_minkpi == value) { return; }
                _minkpi = value;
                this.ValidateProperty(SAMPLEGROUP.MINKPI, _minkpi);
                this.NotifyPropertyChanged(SAMPLEGROUP.MINKPI);
            }
        }
        private Int64 _maxkpi;
        [XmlElement]
        [Field(Name = "MaxKPI")]
        public virtual Int64 MaxKPI
        {
            get
            {
                return _maxkpi;
            }
            set
            {
                if (_maxkpi == value) { return; }
                _maxkpi = value;
                this.ValidateProperty(SAMPLEGROUP.MAXKPI, _maxkpi);
                this.NotifyPropertyChanged(SAMPLEGROUP.MAXKPI);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("CutLeave", this.CutLeave) && isValid;
            isValid = ValidateProperty("UOM", this.UOM) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("SecondaryProduct", this.SecondaryProduct) && isValid;
            isValid = ValidateProperty("BiomassProduct", this.BiomassProduct) && isValid;
            isValid = ValidateProperty("DefaultLiveDead", this.DefaultLiveDead) && isValid;
            isValid = ValidateProperty("SamplingFrequency", this.SamplingFrequency) && isValid;
            isValid = ValidateProperty("InsuranceFrequency", this.InsuranceFrequency) && isValid;
            isValid = ValidateProperty("KZ", this.KZ) && isValid;
            isValid = ValidateProperty("BigBAF", this.BigBAF) && isValid;
            isValid = ValidateProperty("SmallFPS", this.SmallFPS) && isValid;
            isValid = ValidateProperty("TallyMethod", this.TallyMethod) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("SampleSelectorType", this.SampleSelectorType) && isValid;
            isValid = ValidateProperty("SampleSelectorState", this.SampleSelectorState) && isValid;
            isValid = ValidateProperty("MinKPI", this.MinKPI) && isValid;
            isValid = ValidateProperty("MaxKPI", this.MaxKPI) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as SampleGroupDO);
        }

        public void SetValues(SampleGroupDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            CutLeave = obj.CutLeave;
            UOM = obj.UOM;
            PrimaryProduct = obj.PrimaryProduct;
            SecondaryProduct = obj.SecondaryProduct;
            BiomassProduct = obj.BiomassProduct;
            DefaultLiveDead = obj.DefaultLiveDead;
            SamplingFrequency = obj.SamplingFrequency;
            InsuranceFrequency = obj.InsuranceFrequency;
            KZ = obj.KZ;
            BigBAF = obj.BigBAF;
            SmallFPS = obj.SmallFPS;
            TallyMethod = obj.TallyMethod;
            Description = obj.Description;
            SampleSelectorType = obj.SampleSelectorType;
            SampleSelectorState = obj.SampleSelectorState;
            MinKPI = obj.MinKPI;
            MaxKPI = obj.MaxKPI;
        }
    }
    [Table("TreeDefaultValue")]
    public partial class TreeDefaultValueDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeDefaultValueDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("PrimaryProduct", "TreeDefaultValue", "PrimaryProduct is Required"));
            _validator.Add(new NotNullRule("Species", "TreeDefaultValue", "Species is Required"));
            _validator.Add(new FieldValidator("LiveDead", "TreeDefaultValue", "Invalid Live Dead Value", double.MinValue, double.MinValue, true, "L D"));
            _validator.Add(new FieldValidator("FIAcode", "TreeDefaultValue", "Three digit FIA code", double.MinValue, double.MinValue, false, null));
            _validator.Add(new FieldValidator("CullPrimary", "TreeDefaultValue", "Cull Primary Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("HiddenPrimary", "TreeDefaultValue", "Hidden Primary Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("CullSecondary", "TreeDefaultValue", "Cull Secondary Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("HiddenSecondary", "TreeDefaultValue", "Hidden Secondary Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("Recoverable", "TreeDefaultValue", "Recoverable Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("TreeGrade", "TreeDefaultValue", "Invalid Tree Grade", double.MinValue, double.MinValue, false, "0 1 2 3 4 5 6 7 8 9"));
            _validator.Add(new FieldValidator("MerchHeightLogLength", "TreeDefaultValue", "Merch Height Log Length Out of Range", 0, 50, false, null));
            _validator.Add(new FieldValidator("FormClass", "TreeDefaultValue", "Form Class Out of Range", 0, 99, false, null));
            _validator.Add(new FieldValidator("BarkThicknessRatio", "TreeDefaultValue", "Bark Thickness Ratio Out of Range", 0, 100, false, null));
            _validator.Add(new FieldValidator("AverageZ", "TreeDefaultValue", "Average Z Out of Range", -10, 10, false, null));
        }

        public TreeDefaultValueDO() { }

        public TreeDefaultValueDO(TreeDefaultValueDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeDefaultValueDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "TreeDefaultValue_CN")]
        public Int64? TreeDefaultValue_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(TREEDEFAULTVALUE.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.PRIMARYPRODUCT);
            }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(TREEDEFAULTVALUE.SPECIES, _species);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.SPECIES);
            }
        }
        private String _livedead;
        [XmlElement]
        [Field(Name = "LiveDead")]
        public virtual String LiveDead
        {
            get
            {
                return _livedead;
            }
            set
            {
                if (_livedead == value) { return; }
                _livedead = value;
                this.ValidateProperty(TREEDEFAULTVALUE.LIVEDEAD, _livedead);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.LIVEDEAD);
            }
        }
        private Int64 _fiacode;
        [XmlElement]
        [Field(Name = "FIAcode")]
        public virtual Int64 FIAcode
        {
            get
            {
                return _fiacode;
            }
            set
            {
                if (_fiacode == value) { return; }
                _fiacode = value;
                this.ValidateProperty(TREEDEFAULTVALUE.FIACODE, _fiacode);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.FIACODE);
            }
        }
        private float _cullprimary = 0.0f;
        [XmlElement]
        [Field(Name = "CullPrimary")]
        public virtual float CullPrimary
        {
            get
            {
                return _cullprimary;
            }
            set
            {
                if (Math.Abs(_cullprimary - value) < float.Epsilon) { return; }
                _cullprimary = value;
                this.ValidateProperty(TREEDEFAULTVALUE.CULLPRIMARY, _cullprimary);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.CULLPRIMARY);
            }
        }
        private float _hiddenprimary = 0.0f;
        [XmlElement]
        [Field(Name = "HiddenPrimary")]
        public virtual float HiddenPrimary
        {
            get
            {
                return _hiddenprimary;
            }
            set
            {
                if (Math.Abs(_hiddenprimary - value) < float.Epsilon) { return; }
                _hiddenprimary = value;
                this.ValidateProperty(TREEDEFAULTVALUE.HIDDENPRIMARY, _hiddenprimary);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.HIDDENPRIMARY);
            }
        }
        private float _cullsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "CullSecondary")]
        public virtual float CullSecondary
        {
            get
            {
                return _cullsecondary;
            }
            set
            {
                if (Math.Abs(_cullsecondary - value) < float.Epsilon) { return; }
                _cullsecondary = value;
                this.ValidateProperty(TREEDEFAULTVALUE.CULLSECONDARY, _cullsecondary);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.CULLSECONDARY);
            }
        }
        private float _hiddensecondary = 0.0f;
        [XmlElement]
        [Field(Name = "HiddenSecondary")]
        public virtual float HiddenSecondary
        {
            get
            {
                return _hiddensecondary;
            }
            set
            {
                if (Math.Abs(_hiddensecondary - value) < float.Epsilon) { return; }
                _hiddensecondary = value;
                this.ValidateProperty(TREEDEFAULTVALUE.HIDDENSECONDARY, _hiddensecondary);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.HIDDENSECONDARY);
            }
        }
        private float _recoverable = 0.0f;
        [XmlElement]
        [Field(Name = "Recoverable")]
        public virtual float Recoverable
        {
            get
            {
                return _recoverable;
            }
            set
            {
                if (Math.Abs(_recoverable - value) < float.Epsilon) { return; }
                _recoverable = value;
                this.ValidateProperty(TREEDEFAULTVALUE.RECOVERABLE, _recoverable);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.RECOVERABLE);
            }
        }
        private String _contractspecies;
        [XmlElement]
        [Field(Name = "ContractSpecies")]
        public virtual String ContractSpecies
        {
            get
            {
                return _contractspecies;
            }
            set
            {
                if (_contractspecies == value) { return; }
                _contractspecies = value;
                this.ValidateProperty(TREEDEFAULTVALUE.CONTRACTSPECIES, _contractspecies);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.CONTRACTSPECIES);
            }
        }
        private String _treegrade = "0";
        [XmlElement]
        [Field(Name = "TreeGrade")]
        public virtual String TreeGrade
        {
            get
            {
                return _treegrade;
            }
            set
            {
                if (_treegrade == value) { return; }
                _treegrade = value;
                this.ValidateProperty(TREEDEFAULTVALUE.TREEGRADE, _treegrade);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.TREEGRADE);
            }
        }
        private Int64 _merchheightloglength;
        [XmlElement]
        [Field(Name = "MerchHeightLogLength")]
        public virtual Int64 MerchHeightLogLength
        {
            get
            {
                return _merchheightloglength;
            }
            set
            {
                if (_merchheightloglength == value) { return; }
                _merchheightloglength = value;
                this.ValidateProperty(TREEDEFAULTVALUE.MERCHHEIGHTLOGLENGTH, _merchheightloglength);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.MERCHHEIGHTLOGLENGTH);
            }
        }
        private String _merchheighttype = "F";
        [XmlElement]
        [Field(Name = "MerchHeightType")]
        public virtual String MerchHeightType
        {
            get
            {
                return _merchheighttype;
            }
            set
            {
                if (_merchheighttype == value) { return; }
                _merchheighttype = value;
                this.ValidateProperty(TREEDEFAULTVALUE.MERCHHEIGHTTYPE, _merchheighttype);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.MERCHHEIGHTTYPE);
            }
        }
        private float _formclass = 0.0f;
        [XmlElement]
        [Field(Name = "FormClass")]
        public virtual float FormClass
        {
            get
            {
                return _formclass;
            }
            set
            {
                if (Math.Abs(_formclass - value) < float.Epsilon) { return; }
                _formclass = value;
                this.ValidateProperty(TREEDEFAULTVALUE.FORMCLASS, _formclass);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.FORMCLASS);
            }
        }
        private float _barkthicknessratio = 0.0f;
        [XmlElement]
        [Field(Name = "BarkThicknessRatio")]
        public virtual float BarkThicknessRatio
        {
            get
            {
                return _barkthicknessratio;
            }
            set
            {
                if (Math.Abs(_barkthicknessratio - value) < float.Epsilon) { return; }
                _barkthicknessratio = value;
                this.ValidateProperty(TREEDEFAULTVALUE.BARKTHICKNESSRATIO, _barkthicknessratio);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.BARKTHICKNESSRATIO);
            }
        }
        private float _averagez = 0.0f;
        [XmlElement]
        [Field(Name = "AverageZ")]
        public virtual float AverageZ
        {
            get
            {
                return _averagez;
            }
            set
            {
                if (Math.Abs(_averagez - value) < float.Epsilon) { return; }
                _averagez = value;
                this.ValidateProperty(TREEDEFAULTVALUE.AVERAGEZ, _averagez);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.AVERAGEZ);
            }
        }
        private float _referenceheightpercent = 0.0f;
        [XmlElement]
        [Field(Name = "ReferenceHeightPercent")]
        public virtual float ReferenceHeightPercent
        {
            get
            {
                return _referenceheightpercent;
            }
            set
            {
                if (Math.Abs(_referenceheightpercent - value) < float.Epsilon) { return; }
                _referenceheightpercent = value;
                this.ValidateProperty(TREEDEFAULTVALUE.REFERENCEHEIGHTPERCENT, _referenceheightpercent);
                this.NotifyPropertyChanged(TREEDEFAULTVALUE.REFERENCEHEIGHTPERCENT);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("LiveDead", this.LiveDead) && isValid;
            isValid = ValidateProperty("FIAcode", this.FIAcode) && isValid;
            isValid = ValidateProperty("CullPrimary", this.CullPrimary) && isValid;
            isValid = ValidateProperty("HiddenPrimary", this.HiddenPrimary) && isValid;
            isValid = ValidateProperty("CullSecondary", this.CullSecondary) && isValid;
            isValid = ValidateProperty("HiddenSecondary", this.HiddenSecondary) && isValid;
            isValid = ValidateProperty("Recoverable", this.Recoverable) && isValid;
            isValid = ValidateProperty("ContractSpecies", this.ContractSpecies) && isValid;
            isValid = ValidateProperty("TreeGrade", this.TreeGrade) && isValid;
            isValid = ValidateProperty("MerchHeightLogLength", this.MerchHeightLogLength) && isValid;
            isValid = ValidateProperty("MerchHeightType", this.MerchHeightType) && isValid;
            isValid = ValidateProperty("FormClass", this.FormClass) && isValid;
            isValid = ValidateProperty("BarkThicknessRatio", this.BarkThicknessRatio) && isValid;
            isValid = ValidateProperty("AverageZ", this.AverageZ) && isValid;
            isValid = ValidateProperty("ReferenceHeightPercent", this.ReferenceHeightPercent) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeDefaultValueDO);
        }

        public void SetValues(TreeDefaultValueDO obj)
        {
            if (obj == null) { return; }
            PrimaryProduct = obj.PrimaryProduct;
            Species = obj.Species;
            LiveDead = obj.LiveDead;
            FIAcode = obj.FIAcode;
            CullPrimary = obj.CullPrimary;
            HiddenPrimary = obj.HiddenPrimary;
            CullSecondary = obj.CullSecondary;
            HiddenSecondary = obj.HiddenSecondary;
            Recoverable = obj.Recoverable;
            ContractSpecies = obj.ContractSpecies;
            TreeGrade = obj.TreeGrade;
            MerchHeightLogLength = obj.MerchHeightLogLength;
            MerchHeightType = obj.MerchHeightType;
            FormClass = obj.FormClass;
            BarkThicknessRatio = obj.BarkThicknessRatio;
            AverageZ = obj.AverageZ;
            ReferenceHeightPercent = obj.ReferenceHeightPercent;
        }
    }
    [Table("SampleGroupTreeDefaultValue")]
    public partial class SampleGroupTreeDefaultValueDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static SampleGroupTreeDefaultValueDO()
        {
            _validator = new RowValidator();
        }

        public SampleGroupTreeDefaultValueDO() { }

        public SampleGroupTreeDefaultValueDO(SampleGroupTreeDefaultValueDO obj) : this()
        {
            SetValues(obj);
        }

        public SampleGroupTreeDefaultValueDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(SAMPLEGROUPTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(SAMPLEGROUPTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }
        private long? _samplegroup_cn;
        [XmlIgnore]
        [Field(Name = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN
        {
            get
            {

                if (_samplegroup != null)
                {
                    return _samplegroup.SampleGroup_CN;
                }
                return _samplegroup_cn;
            }
            set
            {
                if (_samplegroup_cn == value) { return; }
                if (value == null || value.Value == 0) { _samplegroup = null; }
                _samplegroup_cn = value;
                this.ValidateProperty(SAMPLEGROUPTREEDEFAULTVALUE.SAMPLEGROUP_CN, _samplegroup_cn);
                this.NotifyPropertyChanged(SAMPLEGROUPTREEDEFAULTVALUE.SAMPLEGROUP_CN);
            }
        }
        public virtual SampleGroupDO GetSampleGroup()
        {
            if (DAL == null) { return null; }
            if (this.SampleGroup_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<SampleGroupDO>(this.SampleGroup_CN);
            //return DAL.ReadSingleRow<SampleGroupDO>(SAMPLEGROUP._NAME, this.SampleGroup_CN);
        }

        private SampleGroupDO _samplegroup = null;
        [XmlIgnore]
        public SampleGroupDO SampleGroup
        {
            get
            {
                if (_samplegroup == null)
                {
                    _samplegroup = GetSampleGroup();
                }
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                SampleGroup_CN = (value != null) ? value.SampleGroup_CN : null;
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            isValid = ValidateProperty("SampleGroup_CN", this.SampleGroup_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as SampleGroupTreeDefaultValueDO);
        }

        public void SetValues(SampleGroupTreeDefaultValueDO obj)
        {
            if (obj == null) { return; }
        }
    }
    [Table("Plot")]
    public partial class PlotDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static PlotDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "Plot", "Stratum_CN is Required"));
            _validator.Add(new NotNullRule("CuttingUnit_CN", "Plot", "CuttingUnit_CN is Required"));
            _validator.Add(new NotNullRule("PlotNumber", "Plot", "PlotNumber is Required"));
        }

        public PlotDO() { }

        public PlotDO(PlotDO obj) : this()
        {
            SetValues(obj);
        }

        public PlotDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Plot_CN")]
        public Int64? Plot_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(PLOT.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(PLOT.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private long? _cuttingunit_cn;
        [XmlIgnore]
        [Field(Name = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN
        {
            get
            {

                if (_cuttingunit != null)
                {
                    return _cuttingunit.CuttingUnit_CN;
                }
                return _cuttingunit_cn;
            }
            set
            {
                if (_cuttingunit_cn == value) { return; }
                if (value == null || value.Value == 0) { _cuttingunit = null; }
                _cuttingunit_cn = value;
                this.ValidateProperty(PLOT.CUTTINGUNIT_CN, _cuttingunit_cn);
                this.NotifyPropertyChanged(PLOT.CUTTINGUNIT_CN);
            }
        }
        public virtual CuttingUnitDO GetCuttingUnit()
        {
            if (DAL == null) { return null; }
            if (this.CuttingUnit_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<CuttingUnitDO>(this.CuttingUnit_CN);
            //return DAL.ReadSingleRow<CuttingUnitDO>(CUTTINGUNIT._NAME, this.CuttingUnit_CN);
        }

        private CuttingUnitDO _cuttingunit = null;
        [XmlIgnore]
        public CuttingUnitDO CuttingUnit
        {
            get
            {
                if (_cuttingunit == null)
                {
                    _cuttingunit = GetCuttingUnit();
                }
                return _cuttingunit;
            }
            set
            {
                if (_cuttingunit == value) { return; }
                _cuttingunit = value;
                CuttingUnit_CN = (value != null) ? value.CuttingUnit_CN : null;
            }
        }
        private Guid? _plot_guid;
        [XmlElement]
        [Field(Name = "Plot_GUID")]
        public virtual Guid? Plot_GUID
        {
            get
            {
                return _plot_guid;
            }
            set
            {
                if (_plot_guid == value) { return; }
                _plot_guid = value;
                this.ValidateProperty(PLOT.PLOT_GUID, _plot_guid);
                this.NotifyPropertyChanged(PLOT.PLOT_GUID);
            }
        }
        private Int64 _plotnumber;
        [XmlElement]
        [Field(Name = "PlotNumber")]
        public virtual Int64 PlotNumber
        {
            get
            {
                return _plotnumber;
            }
            set
            {
                if (_plotnumber == value) { return; }
                _plotnumber = value;
                this.ValidateProperty(PLOT.PLOTNUMBER, _plotnumber);
                this.NotifyPropertyChanged(PLOT.PLOTNUMBER);
            }
        }
        private String _isempty;
        [XmlElement]
        [Field(Name = "IsEmpty")]
        public virtual String IsEmpty
        {
            get
            {
                return _isempty;
            }
            set
            {
                if (_isempty == value) { return; }
                _isempty = value;
                this.ValidateProperty(PLOT.ISEMPTY, _isempty);
                this.NotifyPropertyChanged(PLOT.ISEMPTY);
            }
        }
        private float _slope = 0.0f;
        [XmlElement]
        [Field(Name = "Slope")]
        public virtual float Slope
        {
            get
            {
                return _slope;
            }
            set
            {
                if (Math.Abs(_slope - value) < float.Epsilon) { return; }
                _slope = value;
                this.ValidateProperty(PLOT.SLOPE, _slope);
                this.NotifyPropertyChanged(PLOT.SLOPE);
            }
        }
        private float _kpi = 0.0f;
        [XmlElement]
        [Field(Name = "KPI")]
        public virtual float KPI
        {
            get
            {
                return _kpi;
            }
            set
            {
                if (Math.Abs(_kpi - value) < float.Epsilon) { return; }
                _kpi = value;
                this.ValidateProperty(PLOT.KPI, _kpi);
                this.NotifyPropertyChanged(PLOT.KPI);
            }
        }
        private float _aspect = 0.0f;
        [XmlElement]
        [Field(Name = "Aspect")]
        public virtual float Aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                if (Math.Abs(_aspect - value) < float.Epsilon) { return; }
                _aspect = value;
                this.ValidateProperty(PLOT.ASPECT, _aspect);
                this.NotifyPropertyChanged(PLOT.ASPECT);
            }
        }
        private String _remarks;
        [XmlElement]
        [Field(Name = "Remarks")]
        public virtual String Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                if (_remarks == value) { return; }
                _remarks = value;
                this.ValidateProperty(PLOT.REMARKS, _remarks);
                this.NotifyPropertyChanged(PLOT.REMARKS);
            }
        }
        private float _xcoordinate = 0.0f;
        [XmlElement]
        [Field(Name = "XCoordinate")]
        public virtual float XCoordinate
        {
            get
            {
                return _xcoordinate;
            }
            set
            {
                if (Math.Abs(_xcoordinate - value) < float.Epsilon) { return; }
                _xcoordinate = value;
                this.ValidateProperty(PLOT.XCOORDINATE, _xcoordinate);
                this.NotifyPropertyChanged(PLOT.XCOORDINATE);
            }
        }
        private float _ycoordinate = 0.0f;
        [XmlElement]
        [Field(Name = "YCoordinate")]
        public virtual float YCoordinate
        {
            get
            {
                return _ycoordinate;
            }
            set
            {
                if (Math.Abs(_ycoordinate - value) < float.Epsilon) { return; }
                _ycoordinate = value;
                this.ValidateProperty(PLOT.YCOORDINATE, _ycoordinate);
                this.NotifyPropertyChanged(PLOT.YCOORDINATE);
            }
        }
        private float _zcoordinate = 0.0f;
        [XmlElement]
        [Field(Name = "ZCoordinate")]
        public virtual float ZCoordinate
        {
            get
            {
                return _zcoordinate;
            }
            set
            {
                if (Math.Abs(_zcoordinate - value) < float.Epsilon) { return; }
                _zcoordinate = value;
                this.ValidateProperty(PLOT.ZCOORDINATE, _zcoordinate);
                this.NotifyPropertyChanged(PLOT.ZCOORDINATE);
            }
        }
        private String _metadata;
        [XmlElement]
        [Field(Name = "MetaData")]
        public virtual String MetaData
        {
            get
            {
                return _metadata;
            }
            set
            {
                if (_metadata == value) { return; }
                _metadata = value;
                this.ValidateProperty(PLOT.METADATA, _metadata);
                this.NotifyPropertyChanged(PLOT.METADATA);
            }
        }
        private byte[] _blob;
        [XmlElement]
        [Field(Name = "Blob")]
        public virtual byte[] Blob
        {
            get
            {
                return _blob;
            }
            set
            {
                if (_blob == value) { return; }
                _blob = value;
                this.ValidateProperty(PLOT.BLOB, _blob);
                this.NotifyPropertyChanged(PLOT.BLOB);
            }
        }
        private Int64 _threeprandomvalue;
        [XmlElement]
        [Field(Name = "ThreePRandomValue")]
        public virtual Int64 ThreePRandomValue
        {
            get
            {
                return _threeprandomvalue;
            }
            set
            {
                if (_threeprandomvalue == value) { return; }
                _threeprandomvalue = value;
                this.ValidateProperty(PLOT.THREEPRANDOMVALUE, _threeprandomvalue);
                this.NotifyPropertyChanged(PLOT.THREEPRANDOMVALUE);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Plot_GUID", this.Plot_GUID) && isValid;
            isValid = ValidateProperty("PlotNumber", this.PlotNumber) && isValid;
            isValid = ValidateProperty("IsEmpty", this.IsEmpty) && isValid;
            isValid = ValidateProperty("Slope", this.Slope) && isValid;
            isValid = ValidateProperty("KPI", this.KPI) && isValid;
            isValid = ValidateProperty("Aspect", this.Aspect) && isValid;
            isValid = ValidateProperty("Remarks", this.Remarks) && isValid;
            isValid = ValidateProperty("XCoordinate", this.XCoordinate) && isValid;
            isValid = ValidateProperty("YCoordinate", this.YCoordinate) && isValid;
            isValid = ValidateProperty("ZCoordinate", this.ZCoordinate) && isValid;
            isValid = ValidateProperty("MetaData", this.MetaData) && isValid;
            isValid = ValidateProperty("Blob", this.Blob) && isValid;
            isValid = ValidateProperty("ThreePRandomValue", this.ThreePRandomValue) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            isValid = ValidateProperty("CuttingUnit_CN", this.CuttingUnit_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as PlotDO);
        }

        public void SetValues(PlotDO obj)
        {
            if (obj == null) { return; }
            Plot_GUID = obj.Plot_GUID;
            PlotNumber = obj.PlotNumber;
            IsEmpty = obj.IsEmpty;
            Slope = obj.Slope;
            KPI = obj.KPI;
            Aspect = obj.Aspect;
            Remarks = obj.Remarks;
            XCoordinate = obj.XCoordinate;
            YCoordinate = obj.YCoordinate;
            ZCoordinate = obj.ZCoordinate;
            MetaData = obj.MetaData;
            Blob = obj.Blob;
            ThreePRandomValue = obj.ThreePRandomValue;
        }
    }
    [Table("Tree")]
    public partial class TreeDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "Tree", "Stratum_CN is Required"));
            _validator.Add(new NotNullRule("CuttingUnit_CN", "Tree", "CuttingUnit_CN is Required"));
            _validator.Add(new NotNullRule("TreeNumber", "Tree", "TreeNumber is Required"));
            _validator.Add(new FieldValidator("DBH", "Tree", "DBH Out of range", 0, double.MinValue, false, null));
        }

        public TreeDO() { }

        public TreeDO(TreeDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Tree_CN")]
        public Int64? Tree_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(TREE.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(TREE.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(TREE.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(TREE.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private long? _samplegroup_cn;
        [XmlIgnore]
        [Field(Name = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN
        {
            get
            {

                if (_samplegroup != null)
                {
                    return _samplegroup.SampleGroup_CN;
                }
                return _samplegroup_cn;
            }
            set
            {
                if (_samplegroup_cn == value) { return; }
                if (value == null || value.Value == 0) { _samplegroup = null; }
                _samplegroup_cn = value;
                this.ValidateProperty(TREE.SAMPLEGROUP_CN, _samplegroup_cn);
                this.NotifyPropertyChanged(TREE.SAMPLEGROUP_CN);
            }
        }
        public virtual SampleGroupDO GetSampleGroup()
        {
            if (DAL == null) { return null; }
            if (this.SampleGroup_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<SampleGroupDO>(this.SampleGroup_CN);
            //return DAL.ReadSingleRow<SampleGroupDO>(SAMPLEGROUP._NAME, this.SampleGroup_CN);
        }

        private SampleGroupDO _samplegroup = null;
        [XmlIgnore]
        public SampleGroupDO SampleGroup
        {
            get
            {
                if (_samplegroup == null)
                {
                    _samplegroup = GetSampleGroup();
                }
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                SampleGroup_CN = (value != null) ? value.SampleGroup_CN : null;
            }
        }
        private long? _cuttingunit_cn;
        [XmlIgnore]
        [Field(Name = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN
        {
            get
            {

                if (_cuttingunit != null)
                {
                    return _cuttingunit.CuttingUnit_CN;
                }
                return _cuttingunit_cn;
            }
            set
            {
                if (_cuttingunit_cn == value) { return; }
                if (value == null || value.Value == 0) { _cuttingunit = null; }
                _cuttingunit_cn = value;
                this.ValidateProperty(TREE.CUTTINGUNIT_CN, _cuttingunit_cn);
                this.NotifyPropertyChanged(TREE.CUTTINGUNIT_CN);
            }
        }
        public virtual CuttingUnitDO GetCuttingUnit()
        {
            if (DAL == null) { return null; }
            if (this.CuttingUnit_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<CuttingUnitDO>(this.CuttingUnit_CN);
            //return DAL.ReadSingleRow<CuttingUnitDO>(CUTTINGUNIT._NAME, this.CuttingUnit_CN);
        }

        private CuttingUnitDO _cuttingunit = null;
        [XmlIgnore]
        public CuttingUnitDO CuttingUnit
        {
            get
            {
                if (_cuttingunit == null)
                {
                    _cuttingunit = GetCuttingUnit();
                }
                return _cuttingunit;
            }
            set
            {
                if (_cuttingunit == value) { return; }
                _cuttingunit = value;
                CuttingUnit_CN = (value != null) ? value.CuttingUnit_CN : null;
            }
        }
        private long? _plot_cn;
        [XmlIgnore]
        [Field(Name = "Plot_CN")]
        public virtual long? Plot_CN
        {
            get
            {

                if (_plot != null)
                {
                    return _plot.Plot_CN;
                }
                return _plot_cn;
            }
            set
            {
                if (_plot_cn == value) { return; }
                if (value == null || value.Value == 0) { _plot = null; }
                _plot_cn = value;
                this.ValidateProperty(TREE.PLOT_CN, _plot_cn);
                this.NotifyPropertyChanged(TREE.PLOT_CN);
            }
        }
        public virtual PlotDO GetPlot()
        {
            if (DAL == null) { return null; }
            if (this.Plot_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<PlotDO>(this.Plot_CN);
            //return DAL.ReadSingleRow<PlotDO>(PLOT._NAME, this.Plot_CN);
        }

        private PlotDO _plot = null;
        [XmlIgnore]
        public PlotDO Plot
        {
            get
            {
                if (_plot == null)
                {
                    _plot = GetPlot();
                }
                return _plot;
            }
            set
            {
                if (_plot == value) { return; }
                _plot = value;
                Plot_CN = (value != null) ? value.Plot_CN : null;
            }
        }
        private Guid? _tree_guid;
        [XmlElement]
        [Field(Name = "Tree_GUID")]
        public virtual Guid? Tree_GUID
        {
            get
            {
                return _tree_guid;
            }
            set
            {
                if (_tree_guid == value) { return; }
                _tree_guid = value;
                this.ValidateProperty(TREE.TREE_GUID, _tree_guid);
                this.NotifyPropertyChanged(TREE.TREE_GUID);
            }
        }
        private Int64 _treenumber;
        [XmlElement]
        [Field(Name = "TreeNumber")]
        public virtual Int64 TreeNumber
        {
            get
            {
                return _treenumber;
            }
            set
            {
                if (_treenumber == value) { return; }
                _treenumber = value;
                this.ValidateProperty(TREE.TREENUMBER, _treenumber);
                this.NotifyPropertyChanged(TREE.TREENUMBER);
            }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(TREE.SPECIES, _species);
                this.NotifyPropertyChanged(TREE.SPECIES);
            }
        }
        private String _countormeasure;
        [XmlElement]
        [Field(Name = "CountOrMeasure")]
        public virtual String CountOrMeasure
        {
            get
            {
                return _countormeasure;
            }
            set
            {
                if (_countormeasure == value) { return; }
                _countormeasure = value;
                this.ValidateProperty(TREE.COUNTORMEASURE, _countormeasure);
                this.NotifyPropertyChanged(TREE.COUNTORMEASURE);
            }
        }
        private float _treecount = 0.0f;
        [XmlElement]
        [Field(Name = "TreeCount")]
        public virtual float TreeCount
        {
            get
            {
                return _treecount;
            }
            set
            {
                if (Math.Abs(_treecount - value) < float.Epsilon) { return; }
                _treecount = value;
                this.ValidateProperty(TREE.TREECOUNT, _treecount);
                this.NotifyPropertyChanged(TREE.TREECOUNT);
            }
        }
        private float _kpi = 0.0f;
        [XmlElement]
        [Field(Name = "KPI")]
        public virtual float KPI
        {
            get
            {
                return _kpi;
            }
            set
            {
                if (Math.Abs(_kpi - value) < float.Epsilon) { return; }
                _kpi = value;
                this.ValidateProperty(TREE.KPI, _kpi);
                this.NotifyPropertyChanged(TREE.KPI);
            }
        }
        private String _stm = "N";
        [XmlElement]
        [Field(Name = "STM")]
        public virtual String STM
        {
            get
            {
                return _stm;
            }
            set
            {
                if (_stm == value) { return; }
                _stm = value;
                this.ValidateProperty(TREE.STM, _stm);
                this.NotifyPropertyChanged(TREE.STM);
            }
        }
        private float _seendefectprimary = 0.0f;
        [XmlElement]
        [Field(Name = "SeenDefectPrimary")]
        public virtual float SeenDefectPrimary
        {
            get
            {
                return _seendefectprimary;
            }
            set
            {
                if (Math.Abs(_seendefectprimary - value) < float.Epsilon) { return; }
                _seendefectprimary = value;
                this.ValidateProperty(TREE.SEENDEFECTPRIMARY, _seendefectprimary);
                this.NotifyPropertyChanged(TREE.SEENDEFECTPRIMARY);
            }
        }
        private float _seendefectsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "SeenDefectSecondary")]
        public virtual float SeenDefectSecondary
        {
            get
            {
                return _seendefectsecondary;
            }
            set
            {
                if (Math.Abs(_seendefectsecondary - value) < float.Epsilon) { return; }
                _seendefectsecondary = value;
                this.ValidateProperty(TREE.SEENDEFECTSECONDARY, _seendefectsecondary);
                this.NotifyPropertyChanged(TREE.SEENDEFECTSECONDARY);
            }
        }
        private float _recoverableprimary = 0.0f;
        [XmlElement]
        [Field(Name = "RecoverablePrimary")]
        public virtual float RecoverablePrimary
        {
            get
            {
                return _recoverableprimary;
            }
            set
            {
                if (Math.Abs(_recoverableprimary - value) < float.Epsilon) { return; }
                _recoverableprimary = value;
                this.ValidateProperty(TREE.RECOVERABLEPRIMARY, _recoverableprimary);
                this.NotifyPropertyChanged(TREE.RECOVERABLEPRIMARY);
            }
        }
        private float _hiddenprimary = 0.0f;
        [XmlElement]
        [Field(Name = "HiddenPrimary")]
        public virtual float HiddenPrimary
        {
            get
            {
                return _hiddenprimary;
            }
            set
            {
                if (Math.Abs(_hiddenprimary - value) < float.Epsilon) { return; }
                _hiddenprimary = value;
                this.ValidateProperty(TREE.HIDDENPRIMARY, _hiddenprimary);
                this.NotifyPropertyChanged(TREE.HIDDENPRIMARY);
            }
        }
        private String _initials;
        [XmlElement]
        [Field(Name = "Initials")]
        public virtual String Initials
        {
            get
            {
                return _initials;
            }
            set
            {
                if (_initials == value) { return; }
                _initials = value;
                this.ValidateProperty(TREE.INITIALS, _initials);
                this.NotifyPropertyChanged(TREE.INITIALS);
            }
        }
        private String _livedead;
        [XmlElement]
        [Field(Name = "LiveDead")]
        public virtual String LiveDead
        {
            get
            {
                return _livedead;
            }
            set
            {
                if (_livedead == value) { return; }
                _livedead = value;
                this.ValidateProperty(TREE.LIVEDEAD, _livedead);
                this.NotifyPropertyChanged(TREE.LIVEDEAD);
            }
        }
        private String _grade;
        [XmlElement]
        [Field(Name = "Grade")]
        public virtual String Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (_grade == value) { return; }
                _grade = value;
                this.ValidateProperty(TREE.GRADE, _grade);
                this.NotifyPropertyChanged(TREE.GRADE);
            }
        }
        private float _heighttofirstlivelimb = 0.0f;
        [XmlElement]
        [Field(Name = "HeightToFirstLiveLimb")]
        public virtual float HeightToFirstLiveLimb
        {
            get
            {
                return _heighttofirstlivelimb;
            }
            set
            {
                if (Math.Abs(_heighttofirstlivelimb - value) < float.Epsilon) { return; }
                _heighttofirstlivelimb = value;
                this.ValidateProperty(TREE.HEIGHTTOFIRSTLIVELIMB, _heighttofirstlivelimb);
                this.NotifyPropertyChanged(TREE.HEIGHTTOFIRSTLIVELIMB);
            }
        }
        private float _polelength = 0.0f;
        [XmlElement]
        [Field(Name = "PoleLength")]
        public virtual float PoleLength
        {
            get
            {
                return _polelength;
            }
            set
            {
                if (Math.Abs(_polelength - value) < float.Epsilon) { return; }
                _polelength = value;
                this.ValidateProperty(TREE.POLELENGTH, _polelength);
                this.NotifyPropertyChanged(TREE.POLELENGTH);
            }
        }
        private String _clearface;
        [XmlElement]
        [Field(Name = "ClearFace")]
        public virtual String ClearFace
        {
            get
            {
                return _clearface;
            }
            set
            {
                if (_clearface == value) { return; }
                _clearface = value;
                this.ValidateProperty(TREE.CLEARFACE, _clearface);
                this.NotifyPropertyChanged(TREE.CLEARFACE);
            }
        }
        private float _crownratio = 0.0f;
        [XmlElement]
        [Field(Name = "CrownRatio")]
        public virtual float CrownRatio
        {
            get
            {
                return _crownratio;
            }
            set
            {
                if (Math.Abs(_crownratio - value) < float.Epsilon) { return; }
                _crownratio = value;
                this.ValidateProperty(TREE.CROWNRATIO, _crownratio);
                this.NotifyPropertyChanged(TREE.CROWNRATIO);
            }
        }
        private float _dbh = 0.0f;
        [XmlElement]
        [Field(Name = "DBH")]
        public virtual float DBH
        {
            get
            {
                return _dbh;
            }
            set
            {
                if (Math.Abs(_dbh - value) < float.Epsilon) { return; }
                _dbh = value;
                this.ValidateProperty(TREE.DBH, _dbh);
                this.NotifyPropertyChanged(TREE.DBH);
            }
        }
        private float _drc = 0.0f;
        [XmlElement]
        [Field(Name = "DRC")]
        public virtual float DRC
        {
            get
            {
                return _drc;
            }
            set
            {
                if (Math.Abs(_drc - value) < float.Epsilon) { return; }
                _drc = value;
                this.ValidateProperty(TREE.DRC, _drc);
                this.NotifyPropertyChanged(TREE.DRC);
            }
        }
        private float _totalheight = 0.0f;
        [XmlElement]
        [Field(Name = "TotalHeight")]
        public virtual float TotalHeight
        {
            get
            {
                return _totalheight;
            }
            set
            {
                if (Math.Abs(_totalheight - value) < float.Epsilon) { return; }
                _totalheight = value;
                this.ValidateProperty(TREE.TOTALHEIGHT, _totalheight);
                this.NotifyPropertyChanged(TREE.TOTALHEIGHT);
            }
        }
        private float _merchheightprimary = 0.0f;
        [XmlElement]
        [Field(Name = "MerchHeightPrimary")]
        public virtual float MerchHeightPrimary
        {
            get
            {
                return _merchheightprimary;
            }
            set
            {
                if (Math.Abs(_merchheightprimary - value) < float.Epsilon) { return; }
                _merchheightprimary = value;
                this.ValidateProperty(TREE.MERCHHEIGHTPRIMARY, _merchheightprimary);
                this.NotifyPropertyChanged(TREE.MERCHHEIGHTPRIMARY);
            }
        }
        private float _merchheightsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "MerchHeightSecondary")]
        public virtual float MerchHeightSecondary
        {
            get
            {
                return _merchheightsecondary;
            }
            set
            {
                if (Math.Abs(_merchheightsecondary - value) < float.Epsilon) { return; }
                _merchheightsecondary = value;
                this.ValidateProperty(TREE.MERCHHEIGHTSECONDARY, _merchheightsecondary);
                this.NotifyPropertyChanged(TREE.MERCHHEIGHTSECONDARY);
            }
        }
        private float _formclass = 0.0f;
        [XmlElement]
        [Field(Name = "FormClass")]
        public virtual float FormClass
        {
            get
            {
                return _formclass;
            }
            set
            {
                if (Math.Abs(_formclass - value) < float.Epsilon) { return; }
                _formclass = value;
                this.ValidateProperty(TREE.FORMCLASS, _formclass);
                this.NotifyPropertyChanged(TREE.FORMCLASS);
            }
        }
        private float _upperstemdiameter = 0.0f;
        [XmlElement]
        [Field(Name = "UpperStemDiameter")]
        public virtual float UpperStemDiameter
        {
            get
            {
                return _upperstemdiameter;
            }
            set
            {
                if (Math.Abs(_upperstemdiameter - value) < float.Epsilon) { return; }
                _upperstemdiameter = value;
                this.ValidateProperty(TREE.UPPERSTEMDIAMETER, _upperstemdiameter);
                this.NotifyPropertyChanged(TREE.UPPERSTEMDIAMETER);
            }
        }
        private float _upperstemheight = 0.0f;
        [XmlElement]
        [Field(Name = "UpperStemHeight")]
        public virtual float UpperStemHeight
        {
            get
            {
                return _upperstemheight;
            }
            set
            {
                if (Math.Abs(_upperstemheight - value) < float.Epsilon) { return; }
                _upperstemheight = value;
                this.ValidateProperty(TREE.UPPERSTEMHEIGHT, _upperstemheight);
                this.NotifyPropertyChanged(TREE.UPPERSTEMHEIGHT);
            }
        }
        private float _dbhdoublebarkthickness = 0.0f;
        [XmlElement]
        [Field(Name = "DBHDoubleBarkThickness")]
        public virtual float DBHDoubleBarkThickness
        {
            get
            {
                return _dbhdoublebarkthickness;
            }
            set
            {
                if (Math.Abs(_dbhdoublebarkthickness - value) < float.Epsilon) { return; }
                _dbhdoublebarkthickness = value;
                this.ValidateProperty(TREE.DBHDOUBLEBARKTHICKNESS, _dbhdoublebarkthickness);
                this.NotifyPropertyChanged(TREE.DBHDOUBLEBARKTHICKNESS);
            }
        }
        private float _topdibprimary = 0.0f;
        [XmlElement]
        [Field(Name = "TopDIBPrimary")]
        public virtual float TopDIBPrimary
        {
            get
            {
                return _topdibprimary;
            }
            set
            {
                if (Math.Abs(_topdibprimary - value) < float.Epsilon) { return; }
                _topdibprimary = value;
                this.ValidateProperty(TREE.TOPDIBPRIMARY, _topdibprimary);
                this.NotifyPropertyChanged(TREE.TOPDIBPRIMARY);
            }
        }
        private float _topdibsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "TopDIBSecondary")]
        public virtual float TopDIBSecondary
        {
            get
            {
                return _topdibsecondary;
            }
            set
            {
                if (Math.Abs(_topdibsecondary - value) < float.Epsilon) { return; }
                _topdibsecondary = value;
                this.ValidateProperty(TREE.TOPDIBSECONDARY, _topdibsecondary);
                this.NotifyPropertyChanged(TREE.TOPDIBSECONDARY);
            }
        }
        private String _defectcode;
        [XmlElement]
        [Field(Name = "DefectCode")]
        public virtual String DefectCode
        {
            get
            {
                return _defectcode;
            }
            set
            {
                if (_defectcode == value) { return; }
                _defectcode = value;
                this.ValidateProperty(TREE.DEFECTCODE, _defectcode);
                this.NotifyPropertyChanged(TREE.DEFECTCODE);
            }
        }
        private float _diameteratdefect = 0.0f;
        [XmlElement]
        [Field(Name = "DiameterAtDefect")]
        public virtual float DiameterAtDefect
        {
            get
            {
                return _diameteratdefect;
            }
            set
            {
                if (Math.Abs(_diameteratdefect - value) < float.Epsilon) { return; }
                _diameteratdefect = value;
                this.ValidateProperty(TREE.DIAMETERATDEFECT, _diameteratdefect);
                this.NotifyPropertyChanged(TREE.DIAMETERATDEFECT);
            }
        }
        private float _voidpercent = 0.0f;
        [XmlElement]
        [Field(Name = "VoidPercent")]
        public virtual float VoidPercent
        {
            get
            {
                return _voidpercent;
            }
            set
            {
                if (Math.Abs(_voidpercent - value) < float.Epsilon) { return; }
                _voidpercent = value;
                this.ValidateProperty(TREE.VOIDPERCENT, _voidpercent);
                this.NotifyPropertyChanged(TREE.VOIDPERCENT);
            }
        }
        private float _slope = 0.0f;
        [XmlElement]
        [Field(Name = "Slope")]
        public virtual float Slope
        {
            get
            {
                return _slope;
            }
            set
            {
                if (Math.Abs(_slope - value) < float.Epsilon) { return; }
                _slope = value;
                this.ValidateProperty(TREE.SLOPE, _slope);
                this.NotifyPropertyChanged(TREE.SLOPE);
            }
        }
        private float _aspect = 0.0f;
        [XmlElement]
        [Field(Name = "Aspect")]
        public virtual float Aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                if (Math.Abs(_aspect - value) < float.Epsilon) { return; }
                _aspect = value;
                this.ValidateProperty(TREE.ASPECT, _aspect);
                this.NotifyPropertyChanged(TREE.ASPECT);
            }
        }
        private String _remarks;
        [XmlElement]
        [Field(Name = "Remarks")]
        public virtual String Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                if (_remarks == value) { return; }
                _remarks = value;
                this.ValidateProperty(TREE.REMARKS, _remarks);
                this.NotifyPropertyChanged(TREE.REMARKS);
            }
        }
        private Double _xcoordinate = 0.0;
        [XmlElement]
        [Field(Name = "XCoordinate")]
        public virtual Double XCoordinate
        {
            get
            {
                return _xcoordinate;
            }
            set
            {
                if (Math.Abs(_xcoordinate - value) < Double.Epsilon) { return; }
                _xcoordinate = value;
                this.ValidateProperty(TREE.XCOORDINATE, _xcoordinate);
                this.NotifyPropertyChanged(TREE.XCOORDINATE);
            }
        }
        private Double _ycoordinate = 0.0;
        [XmlElement]
        [Field(Name = "YCoordinate")]
        public virtual Double YCoordinate
        {
            get
            {
                return _ycoordinate;
            }
            set
            {
                if (Math.Abs(_ycoordinate - value) < Double.Epsilon) { return; }
                _ycoordinate = value;
                this.ValidateProperty(TREE.YCOORDINATE, _ycoordinate);
                this.NotifyPropertyChanged(TREE.YCOORDINATE);
            }
        }
        private Double _zcoordinate = 0.0;
        [XmlElement]
        [Field(Name = "ZCoordinate")]
        public virtual Double ZCoordinate
        {
            get
            {
                return _zcoordinate;
            }
            set
            {
                if (Math.Abs(_zcoordinate - value) < Double.Epsilon) { return; }
                _zcoordinate = value;
                this.ValidateProperty(TREE.ZCOORDINATE, _zcoordinate);
                this.NotifyPropertyChanged(TREE.ZCOORDINATE);
            }
        }
        private String _metadata;
        [XmlElement]
        [Field(Name = "MetaData")]
        public virtual String MetaData
        {
            get
            {
                return _metadata;
            }
            set
            {
                if (_metadata == value) { return; }
                _metadata = value;
                this.ValidateProperty(TREE.METADATA, _metadata);
                this.NotifyPropertyChanged(TREE.METADATA);
            }
        }
        private Int64 _isfallbuckscale;
        [XmlElement]
        [Field(Name = "IsFallBuckScale")]
        public virtual Int64 IsFallBuckScale
        {
            get
            {
                return _isfallbuckscale;
            }
            set
            {
                if (_isfallbuckscale == value) { return; }
                _isfallbuckscale = value;
                this.ValidateProperty(TREE.ISFALLBUCKSCALE, _isfallbuckscale);
                this.NotifyPropertyChanged(TREE.ISFALLBUCKSCALE);
            }
        }
        private float _expansionfactor = 0.0f;
        [XmlElement]
        [Field(Name = "ExpansionFactor")]
        public virtual float ExpansionFactor
        {
            get
            {
                return _expansionfactor;
            }
            set
            {
                if (Math.Abs(_expansionfactor - value) < float.Epsilon) { return; }
                _expansionfactor = value;
                this.ValidateProperty(TREE.EXPANSIONFACTOR, _expansionfactor);
                this.NotifyPropertyChanged(TREE.EXPANSIONFACTOR);
            }
        }
        private float _treefactor = 0.0f;
        [XmlElement]
        [Field(Name = "TreeFactor")]
        public virtual float TreeFactor
        {
            get
            {
                return _treefactor;
            }
            set
            {
                if (Math.Abs(_treefactor - value) < float.Epsilon) { return; }
                _treefactor = value;
                this.ValidateProperty(TREE.TREEFACTOR, _treefactor);
                this.NotifyPropertyChanged(TREE.TREEFACTOR);
            }
        }
        private float _pointfactor = 0.0f;
        [XmlElement]
        [Field(Name = "PointFactor")]
        public virtual float PointFactor
        {
            get
            {
                return _pointfactor;
            }
            set
            {
                if (Math.Abs(_pointfactor - value) < float.Epsilon) { return; }
                _pointfactor = value;
                this.ValidateProperty(TREE.POINTFACTOR, _pointfactor);
                this.NotifyPropertyChanged(TREE.POINTFACTOR);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Tree_GUID", this.Tree_GUID) && isValid;
            isValid = ValidateProperty("TreeNumber", this.TreeNumber) && isValid;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("CountOrMeasure", this.CountOrMeasure) && isValid;
            isValid = ValidateProperty("TreeCount", this.TreeCount) && isValid;
            isValid = ValidateProperty("KPI", this.KPI) && isValid;
            isValid = ValidateProperty("STM", this.STM) && isValid;
            isValid = ValidateProperty("SeenDefectPrimary", this.SeenDefectPrimary) && isValid;
            isValid = ValidateProperty("SeenDefectSecondary", this.SeenDefectSecondary) && isValid;
            isValid = ValidateProperty("RecoverablePrimary", this.RecoverablePrimary) && isValid;
            isValid = ValidateProperty("HiddenPrimary", this.HiddenPrimary) && isValid;
            isValid = ValidateProperty("Initials", this.Initials) && isValid;
            isValid = ValidateProperty("LiveDead", this.LiveDead) && isValid;
            isValid = ValidateProperty("Grade", this.Grade) && isValid;
            isValid = ValidateProperty("HeightToFirstLiveLimb", this.HeightToFirstLiveLimb) && isValid;
            isValid = ValidateProperty("PoleLength", this.PoleLength) && isValid;
            isValid = ValidateProperty("ClearFace", this.ClearFace) && isValid;
            isValid = ValidateProperty("CrownRatio", this.CrownRatio) && isValid;
            isValid = ValidateProperty("DBH", this.DBH) && isValid;
            isValid = ValidateProperty("DRC", this.DRC) && isValid;
            isValid = ValidateProperty("TotalHeight", this.TotalHeight) && isValid;
            isValid = ValidateProperty("MerchHeightPrimary", this.MerchHeightPrimary) && isValid;
            isValid = ValidateProperty("MerchHeightSecondary", this.MerchHeightSecondary) && isValid;
            isValid = ValidateProperty("FormClass", this.FormClass) && isValid;
            isValid = ValidateProperty("UpperStemDiameter", this.UpperStemDiameter) && isValid;
            isValid = ValidateProperty("UpperStemHeight", this.UpperStemHeight) && isValid;
            isValid = ValidateProperty("DBHDoubleBarkThickness", this.DBHDoubleBarkThickness) && isValid;
            isValid = ValidateProperty("TopDIBPrimary", this.TopDIBPrimary) && isValid;
            isValid = ValidateProperty("TopDIBSecondary", this.TopDIBSecondary) && isValid;
            isValid = ValidateProperty("DefectCode", this.DefectCode) && isValid;
            isValid = ValidateProperty("DiameterAtDefect", this.DiameterAtDefect) && isValid;
            isValid = ValidateProperty("VoidPercent", this.VoidPercent) && isValid;
            isValid = ValidateProperty("Slope", this.Slope) && isValid;
            isValid = ValidateProperty("Aspect", this.Aspect) && isValid;
            isValid = ValidateProperty("Remarks", this.Remarks) && isValid;
            isValid = ValidateProperty("XCoordinate", this.XCoordinate) && isValid;
            isValid = ValidateProperty("YCoordinate", this.YCoordinate) && isValid;
            isValid = ValidateProperty("ZCoordinate", this.ZCoordinate) && isValid;
            isValid = ValidateProperty("MetaData", this.MetaData) && isValid;
            isValid = ValidateProperty("IsFallBuckScale", this.IsFallBuckScale) && isValid;
            isValid = ValidateProperty("ExpansionFactor", this.ExpansionFactor) && isValid;
            isValid = ValidateProperty("TreeFactor", this.TreeFactor) && isValid;
            isValid = ValidateProperty("PointFactor", this.PointFactor) && isValid;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            isValid = ValidateProperty("SampleGroup_CN", this.SampleGroup_CN) && isValid;
            isValid = ValidateProperty("CuttingUnit_CN", this.CuttingUnit_CN) && isValid;
            isValid = ValidateProperty("Plot_CN", this.Plot_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeDO);
        }

        public void SetValues(TreeDO obj)
        {
            if (obj == null) { return; }
            Tree_GUID = obj.Tree_GUID;
            TreeNumber = obj.TreeNumber;
            Species = obj.Species;
            CountOrMeasure = obj.CountOrMeasure;
            TreeCount = obj.TreeCount;
            KPI = obj.KPI;
            STM = obj.STM;
            SeenDefectPrimary = obj.SeenDefectPrimary;
            SeenDefectSecondary = obj.SeenDefectSecondary;
            RecoverablePrimary = obj.RecoverablePrimary;
            HiddenPrimary = obj.HiddenPrimary;
            Initials = obj.Initials;
            LiveDead = obj.LiveDead;
            Grade = obj.Grade;
            HeightToFirstLiveLimb = obj.HeightToFirstLiveLimb;
            PoleLength = obj.PoleLength;
            ClearFace = obj.ClearFace;
            CrownRatio = obj.CrownRatio;
            DBH = obj.DBH;
            DRC = obj.DRC;
            TotalHeight = obj.TotalHeight;
            MerchHeightPrimary = obj.MerchHeightPrimary;
            MerchHeightSecondary = obj.MerchHeightSecondary;
            FormClass = obj.FormClass;
            UpperStemDiameter = obj.UpperStemDiameter;
            UpperStemHeight = obj.UpperStemHeight;
            DBHDoubleBarkThickness = obj.DBHDoubleBarkThickness;
            TopDIBPrimary = obj.TopDIBPrimary;
            TopDIBSecondary = obj.TopDIBSecondary;
            DefectCode = obj.DefectCode;
            DiameterAtDefect = obj.DiameterAtDefect;
            VoidPercent = obj.VoidPercent;
            Slope = obj.Slope;
            Aspect = obj.Aspect;
            Remarks = obj.Remarks;
            XCoordinate = obj.XCoordinate;
            YCoordinate = obj.YCoordinate;
            ZCoordinate = obj.ZCoordinate;
            MetaData = obj.MetaData;
            IsFallBuckScale = obj.IsFallBuckScale;
            ExpansionFactor = obj.ExpansionFactor;
            TreeFactor = obj.TreeFactor;
            PointFactor = obj.PointFactor;
        }
    }
    [Table("Log")]
    public partial class LogDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Tree_CN", "Log", "Tree_CN is Required"));
            _validator.Add(new NotNullRule("LogNumber", "Log", "LogNumber is Required"));
        }

        public LogDO() { }

        public LogDO(LogDO obj) : this()
        {
            SetValues(obj);
        }

        public LogDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Log_CN")]
        public Int64? Log_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _tree_cn;
        [XmlIgnore]
        [Field(Name = "Tree_CN")]
        public virtual long? Tree_CN
        {
            get
            {

                if (_tree != null)
                {
                    return _tree.Tree_CN;
                }
                return _tree_cn;
            }
            set
            {
                if (_tree_cn == value) { return; }
                if (value == null || value.Value == 0) { _tree = null; }
                _tree_cn = value;
                this.ValidateProperty(LOG.TREE_CN, _tree_cn);
                this.NotifyPropertyChanged(LOG.TREE_CN);
            }
        }
        public virtual TreeDO GetTree()
        {
            if (DAL == null) { return null; }
            if (this.Tree_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDO>(this.Tree_CN);
            //return DAL.ReadSingleRow<TreeDO>(TREE._NAME, this.Tree_CN);
        }

        private TreeDO _tree = null;
        [XmlIgnore]
        public TreeDO Tree
        {
            get
            {
                if (_tree == null)
                {
                    _tree = GetTree();
                }
                return _tree;
            }
            set
            {
                if (_tree == value) { return; }
                _tree = value;
                Tree_CN = (value != null) ? value.Tree_CN : null;
            }
        }
        private Guid? _log_guid;
        [XmlElement]
        [Field(Name = "Log_GUID")]
        public virtual Guid? Log_GUID
        {
            get
            {
                return _log_guid;
            }
            set
            {
                if (_log_guid == value) { return; }
                _log_guid = value;
                this.ValidateProperty(LOG.LOG_GUID, _log_guid);
                this.NotifyPropertyChanged(LOG.LOG_GUID);
            }
        }
        private String _lognumber;
        [XmlElement]
        [Field(Name = "LogNumber")]
        public virtual String LogNumber
        {
            get
            {
                return _lognumber;
            }
            set
            {
                if (_lognumber == value) { return; }
                _lognumber = value;
                this.ValidateProperty(LOG.LOGNUMBER, _lognumber);
                this.NotifyPropertyChanged(LOG.LOGNUMBER);
            }
        }
        private String _grade;
        [XmlElement]
        [Field(Name = "Grade")]
        public virtual String Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (_grade == value) { return; }
                _grade = value;
                this.ValidateProperty(LOG.GRADE, _grade);
                this.NotifyPropertyChanged(LOG.GRADE);
            }
        }
        private float _seendefect = 0.0f;
        [XmlElement]
        [Field(Name = "SeenDefect")]
        public virtual float SeenDefect
        {
            get
            {
                return _seendefect;
            }
            set
            {
                if (Math.Abs(_seendefect - value) < float.Epsilon) { return; }
                _seendefect = value;
                this.ValidateProperty(LOG.SEENDEFECT, _seendefect);
                this.NotifyPropertyChanged(LOG.SEENDEFECT);
            }
        }
        private float _percentrecoverable = 0.0f;
        [XmlElement]
        [Field(Name = "PercentRecoverable")]
        public virtual float PercentRecoverable
        {
            get
            {
                return _percentrecoverable;
            }
            set
            {
                if (Math.Abs(_percentrecoverable - value) < float.Epsilon) { return; }
                _percentrecoverable = value;
                this.ValidateProperty(LOG.PERCENTRECOVERABLE, _percentrecoverable);
                this.NotifyPropertyChanged(LOG.PERCENTRECOVERABLE);
            }
        }
        private Int64 _length;
        [XmlElement]
        [Field(Name = "Length")]
        public virtual Int64 Length
        {
            get
            {
                return _length;
            }
            set
            {
                if (_length == value) { return; }
                _length = value;
                this.ValidateProperty(LOG.LENGTH, _length);
                this.NotifyPropertyChanged(LOG.LENGTH);
            }
        }
        private String _exportgrade;
        [XmlElement]
        [Field(Name = "ExportGrade")]
        public virtual String ExportGrade
        {
            get
            {
                return _exportgrade;
            }
            set
            {
                if (_exportgrade == value) { return; }
                _exportgrade = value;
                this.ValidateProperty(LOG.EXPORTGRADE, _exportgrade);
                this.NotifyPropertyChanged(LOG.EXPORTGRADE);
            }
        }
        private float _smallenddiameter = 0.0f;
        [XmlElement]
        [Field(Name = "SmallEndDiameter")]
        public virtual float SmallEndDiameter
        {
            get
            {
                return _smallenddiameter;
            }
            set
            {
                if (Math.Abs(_smallenddiameter - value) < float.Epsilon) { return; }
                _smallenddiameter = value;
                this.ValidateProperty(LOG.SMALLENDDIAMETER, _smallenddiameter);
                this.NotifyPropertyChanged(LOG.SMALLENDDIAMETER);
            }
        }
        private float _largeenddiameter = 0.0f;
        [XmlElement]
        [Field(Name = "LargeEndDiameter")]
        public virtual float LargeEndDiameter
        {
            get
            {
                return _largeenddiameter;
            }
            set
            {
                if (Math.Abs(_largeenddiameter - value) < float.Epsilon) { return; }
                _largeenddiameter = value;
                this.ValidateProperty(LOG.LARGEENDDIAMETER, _largeenddiameter);
                this.NotifyPropertyChanged(LOG.LARGEENDDIAMETER);
            }
        }
        private float _grossboardfoot = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBoardFoot")]
        public virtual float GrossBoardFoot
        {
            get
            {
                return _grossboardfoot;
            }
            set
            {
                if (Math.Abs(_grossboardfoot - value) < float.Epsilon) { return; }
                _grossboardfoot = value;
                this.ValidateProperty(LOG.GROSSBOARDFOOT, _grossboardfoot);
                this.NotifyPropertyChanged(LOG.GROSSBOARDFOOT);
            }
        }
        private float _netboardfoot = 0.0f;
        [XmlElement]
        [Field(Name = "NetBoardFoot")]
        public virtual float NetBoardFoot
        {
            get
            {
                return _netboardfoot;
            }
            set
            {
                if (Math.Abs(_netboardfoot - value) < float.Epsilon) { return; }
                _netboardfoot = value;
                this.ValidateProperty(LOG.NETBOARDFOOT, _netboardfoot);
                this.NotifyPropertyChanged(LOG.NETBOARDFOOT);
            }
        }
        private float _grosscubicfoot = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCubicFoot")]
        public virtual float GrossCubicFoot
        {
            get
            {
                return _grosscubicfoot;
            }
            set
            {
                if (Math.Abs(_grosscubicfoot - value) < float.Epsilon) { return; }
                _grosscubicfoot = value;
                this.ValidateProperty(LOG.GROSSCUBICFOOT, _grosscubicfoot);
                this.NotifyPropertyChanged(LOG.GROSSCUBICFOOT);
            }
        }
        private float _netcubicfoot = 0.0f;
        [XmlElement]
        [Field(Name = "NetCubicFoot")]
        public virtual float NetCubicFoot
        {
            get
            {
                return _netcubicfoot;
            }
            set
            {
                if (Math.Abs(_netcubicfoot - value) < float.Epsilon) { return; }
                _netcubicfoot = value;
                this.ValidateProperty(LOG.NETCUBICFOOT, _netcubicfoot);
                this.NotifyPropertyChanged(LOG.NETCUBICFOOT);
            }
        }
        private float _boardfootremoved = 0.0f;
        [XmlElement]
        [Field(Name = "BoardFootRemoved")]
        public virtual float BoardFootRemoved
        {
            get
            {
                return _boardfootremoved;
            }
            set
            {
                if (Math.Abs(_boardfootremoved - value) < float.Epsilon) { return; }
                _boardfootremoved = value;
                this.ValidateProperty(LOG.BOARDFOOTREMOVED, _boardfootremoved);
                this.NotifyPropertyChanged(LOG.BOARDFOOTREMOVED);
            }
        }
        private float _cubicfootremoved = 0.0f;
        [XmlElement]
        [Field(Name = "CubicFootRemoved")]
        public virtual float CubicFootRemoved
        {
            get
            {
                return _cubicfootremoved;
            }
            set
            {
                if (Math.Abs(_cubicfootremoved - value) < float.Epsilon) { return; }
                _cubicfootremoved = value;
                this.ValidateProperty(LOG.CUBICFOOTREMOVED, _cubicfootremoved);
                this.NotifyPropertyChanged(LOG.CUBICFOOTREMOVED);
            }
        }
        private float _dibclass = 0.0f;
        [XmlElement]
        [Field(Name = "DIBClass")]
        public virtual float DIBClass
        {
            get
            {
                return _dibclass;
            }
            set
            {
                if (Math.Abs(_dibclass - value) < float.Epsilon) { return; }
                _dibclass = value;
                this.ValidateProperty(LOG.DIBCLASS, _dibclass);
                this.NotifyPropertyChanged(LOG.DIBCLASS);
            }
        }
        private float _barkthickness = 0.0f;
        [XmlElement]
        [Field(Name = "BarkThickness")]
        public virtual float BarkThickness
        {
            get
            {
                return _barkthickness;
            }
            set
            {
                if (Math.Abs(_barkthickness - value) < float.Epsilon) { return; }
                _barkthickness = value;
                this.ValidateProperty(LOG.BARKTHICKNESS, _barkthickness);
                this.NotifyPropertyChanged(LOG.BARKTHICKNESS);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Log_GUID", this.Log_GUID) && isValid;
            isValid = ValidateProperty("LogNumber", this.LogNumber) && isValid;
            isValid = ValidateProperty("Grade", this.Grade) && isValid;
            isValid = ValidateProperty("SeenDefect", this.SeenDefect) && isValid;
            isValid = ValidateProperty("PercentRecoverable", this.PercentRecoverable) && isValid;
            isValid = ValidateProperty("Length", this.Length) && isValid;
            isValid = ValidateProperty("ExportGrade", this.ExportGrade) && isValid;
            isValid = ValidateProperty("SmallEndDiameter", this.SmallEndDiameter) && isValid;
            isValid = ValidateProperty("LargeEndDiameter", this.LargeEndDiameter) && isValid;
            isValid = ValidateProperty("GrossBoardFoot", this.GrossBoardFoot) && isValid;
            isValid = ValidateProperty("NetBoardFoot", this.NetBoardFoot) && isValid;
            isValid = ValidateProperty("GrossCubicFoot", this.GrossCubicFoot) && isValid;
            isValid = ValidateProperty("NetCubicFoot", this.NetCubicFoot) && isValid;
            isValid = ValidateProperty("BoardFootRemoved", this.BoardFootRemoved) && isValid;
            isValid = ValidateProperty("CubicFootRemoved", this.CubicFootRemoved) && isValid;
            isValid = ValidateProperty("DIBClass", this.DIBClass) && isValid;
            isValid = ValidateProperty("BarkThickness", this.BarkThickness) && isValid;
            isValid = ValidateProperty("Tree_CN", this.Tree_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogDO);
        }

        public void SetValues(LogDO obj)
        {
            if (obj == null) { return; }
            Log_GUID = obj.Log_GUID;
            LogNumber = obj.LogNumber;
            Grade = obj.Grade;
            SeenDefect = obj.SeenDefect;
            PercentRecoverable = obj.PercentRecoverable;
            Length = obj.Length;
            ExportGrade = obj.ExportGrade;
            SmallEndDiameter = obj.SmallEndDiameter;
            LargeEndDiameter = obj.LargeEndDiameter;
            GrossBoardFoot = obj.GrossBoardFoot;
            NetBoardFoot = obj.NetBoardFoot;
            GrossCubicFoot = obj.GrossCubicFoot;
            NetCubicFoot = obj.NetCubicFoot;
            BoardFootRemoved = obj.BoardFootRemoved;
            CubicFootRemoved = obj.CubicFootRemoved;
            DIBClass = obj.DIBClass;
            BarkThickness = obj.BarkThickness;
        }
    }
    [Table("Stem")]
    public partial class StemDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static StemDO()
        {
            _validator = new RowValidator();
        }

        public StemDO() { }

        public StemDO(StemDO obj) : this()
        {
            SetValues(obj);
        }

        public StemDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Stem_CN")]
        public Int64? Stem_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _tree_cn;
        [XmlIgnore]
        [Field(Name = "Tree_CN")]
        public virtual long? Tree_CN
        {
            get
            {

                if (_tree != null)
                {
                    return _tree.Tree_CN;
                }
                return _tree_cn;
            }
            set
            {
                if (_tree_cn == value) { return; }
                if (value == null || value.Value == 0) { _tree = null; }
                _tree_cn = value;
                this.ValidateProperty(STEM.TREE_CN, _tree_cn);
                this.NotifyPropertyChanged(STEM.TREE_CN);
            }
        }
        public virtual TreeDO GetTree()
        {
            if (DAL == null) { return null; }
            if (this.Tree_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDO>(this.Tree_CN);
            //return DAL.ReadSingleRow<TreeDO>(TREE._NAME, this.Tree_CN);
        }

        private TreeDO _tree = null;
        [XmlIgnore]
        public TreeDO Tree
        {
            get
            {
                if (_tree == null)
                {
                    _tree = GetTree();
                }
                return _tree;
            }
            set
            {
                if (_tree == value) { return; }
                _tree = value;
                Tree_CN = (value != null) ? value.Tree_CN : null;
            }
        }
        private Guid? _stem_guid;
        [XmlElement]
        [Field(Name = "Stem_GUID")]
        public virtual Guid? Stem_GUID
        {
            get
            {
                return _stem_guid;
            }
            set
            {
                if (_stem_guid == value) { return; }
                _stem_guid = value;
                this.ValidateProperty(STEM.STEM_GUID, _stem_guid);
                this.NotifyPropertyChanged(STEM.STEM_GUID);
            }
        }
        private float _diameter = 0.0f;
        [XmlElement]
        [Field(Name = "Diameter")]
        public virtual float Diameter
        {
            get
            {
                return _diameter;
            }
            set
            {
                if (Math.Abs(_diameter - value) < float.Epsilon) { return; }
                _diameter = value;
                this.ValidateProperty(STEM.DIAMETER, _diameter);
                this.NotifyPropertyChanged(STEM.DIAMETER);
            }
        }
        private String _diametertype;
        [XmlElement]
        [Field(Name = "DiameterType")]
        public virtual String DiameterType
        {
            get
            {
                return _diametertype;
            }
            set
            {
                if (_diametertype == value) { return; }
                _diametertype = value;
                this.ValidateProperty(STEM.DIAMETERTYPE, _diametertype);
                this.NotifyPropertyChanged(STEM.DIAMETERTYPE);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Stem_GUID", this.Stem_GUID) && isValid;
            isValid = ValidateProperty("Diameter", this.Diameter) && isValid;
            isValid = ValidateProperty("DiameterType", this.DiameterType) && isValid;
            isValid = ValidateProperty("Tree_CN", this.Tree_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as StemDO);
        }

        public void SetValues(StemDO obj)
        {
            if (obj == null) { return; }
            Stem_GUID = obj.Stem_GUID;
            Diameter = obj.Diameter;
            DiameterType = obj.DiameterType;
        }
    }
    [Table("CountTree")]
    public partial class CountTreeDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static CountTreeDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("SampleGroup_CN", "CountTree", "SampleGroup_CN is Required"));
            _validator.Add(new NotNullRule("CuttingUnit_CN", "CountTree", "CuttingUnit_CN is Required"));
        }

        public CountTreeDO() { }

        public CountTreeDO(CountTreeDO obj) : this()
        {
            SetValues(obj);
        }

        public CountTreeDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "CountTree_CN")]
        public Int64? CountTree_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _samplegroup_cn;
        [XmlIgnore]
        [Field(Name = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN
        {
            get
            {

                if (_samplegroup != null)
                {
                    return _samplegroup.SampleGroup_CN;
                }
                return _samplegroup_cn;
            }
            set
            {
                if (_samplegroup_cn == value) { return; }
                if (value == null || value.Value == 0) { _samplegroup = null; }
                _samplegroup_cn = value;
                this.ValidateProperty(COUNTTREE.SAMPLEGROUP_CN, _samplegroup_cn);
                this.NotifyPropertyChanged(COUNTTREE.SAMPLEGROUP_CN);
            }
        }
        public virtual SampleGroupDO GetSampleGroup()
        {
            if (DAL == null) { return null; }
            if (this.SampleGroup_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<SampleGroupDO>(this.SampleGroup_CN);
            //return DAL.ReadSingleRow<SampleGroupDO>(SAMPLEGROUP._NAME, this.SampleGroup_CN);
        }

        private SampleGroupDO _samplegroup = null;
        [XmlIgnore]
        public SampleGroupDO SampleGroup
        {
            get
            {
                if (_samplegroup == null)
                {
                    _samplegroup = GetSampleGroup();
                }
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                SampleGroup_CN = (value != null) ? value.SampleGroup_CN : null;
            }
        }
        private long? _cuttingunit_cn;
        [XmlIgnore]
        [Field(Name = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN
        {
            get
            {

                if (_cuttingunit != null)
                {
                    return _cuttingunit.CuttingUnit_CN;
                }
                return _cuttingunit_cn;
            }
            set
            {
                if (_cuttingunit_cn == value) { return; }
                if (value == null || value.Value == 0) { _cuttingunit = null; }
                _cuttingunit_cn = value;
                this.ValidateProperty(COUNTTREE.CUTTINGUNIT_CN, _cuttingunit_cn);
                this.NotifyPropertyChanged(COUNTTREE.CUTTINGUNIT_CN);
            }
        }
        public virtual CuttingUnitDO GetCuttingUnit()
        {
            if (DAL == null) { return null; }
            if (this.CuttingUnit_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<CuttingUnitDO>(this.CuttingUnit_CN);
            //return DAL.ReadSingleRow<CuttingUnitDO>(CUTTINGUNIT._NAME, this.CuttingUnit_CN);
        }

        private CuttingUnitDO _cuttingunit = null;
        [XmlIgnore]
        public CuttingUnitDO CuttingUnit
        {
            get
            {
                if (_cuttingunit == null)
                {
                    _cuttingunit = GetCuttingUnit();
                }
                return _cuttingunit;
            }
            set
            {
                if (_cuttingunit == value) { return; }
                _cuttingunit = value;
                CuttingUnit_CN = (value != null) ? value.CuttingUnit_CN : null;
            }
        }
        private long? _tally_cn;
        [XmlIgnore]
        [Field(Name = "Tally_CN")]
        public virtual long? Tally_CN
        {
            get
            {

                if (_tally != null)
                {
                    return _tally.Tally_CN;
                }
                return _tally_cn;
            }
            set
            {
                if (_tally_cn == value) { return; }
                if (value == null || value.Value == 0) { _tally = null; }
                _tally_cn = value;
                this.ValidateProperty(COUNTTREE.TALLY_CN, _tally_cn);
                this.NotifyPropertyChanged(COUNTTREE.TALLY_CN);
            }
        }
        public virtual TallyDO GetTally()
        {
            if (DAL == null) { return null; }
            if (this.Tally_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TallyDO>(this.Tally_CN);
            //return DAL.ReadSingleRow<TallyDO>(TALLY._NAME, this.Tally_CN);
        }

        private TallyDO _tally = null;
        [XmlIgnore]
        public TallyDO Tally
        {
            get
            {
                if (_tally == null)
                {
                    _tally = GetTally();
                }
                return _tally;
            }
            set
            {
                if (_tally == value) { return; }
                _tally = value;
                Tally_CN = (value != null) ? value.Tally_CN : null;
            }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(COUNTTREE.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(COUNTTREE.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }
        private long? _component_cn;
        [XmlIgnore]
        [Field(Name = "Component_CN")]
        public virtual long? Component_CN
        {
            get
            {

                if (_component != null)
                {
                    return _component.Component_CN;
                }
                return _component_cn;
            }
            set
            {
                if (_component_cn == value) { return; }
                if (value == null || value.Value == 0) { _component = null; }
                _component_cn = value;
                this.ValidateProperty(COUNTTREE.COMPONENT_CN, _component_cn);
                this.NotifyPropertyChanged(COUNTTREE.COMPONENT_CN);
            }
        }
        public virtual ComponentDO GetComponent()
        {
            if (DAL == null) { return null; }
            if (this.Component_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<ComponentDO>(this.Component_CN);
            //return DAL.ReadSingleRow<ComponentDO>(COMPONENT._NAME, this.Component_CN);
        }

        private ComponentDO _component = null;
        [XmlIgnore]
        public ComponentDO Component
        {
            get
            {
                if (_component == null)
                {
                    _component = GetComponent();
                }
                return _component;
            }
            set
            {
                if (_component == value) { return; }
                _component = value;
                Component_CN = (value != null) ? value.Component_CN : null;
            }
        }
        private Int64 _treecount;
        [XmlElement]
        [Field(Name = "TreeCount")]
        public virtual Int64 TreeCount
        {
            get
            {
                return _treecount;
            }
            set
            {
                if (_treecount == value) { return; }
                _treecount = value;
                this.ValidateProperty(COUNTTREE.TREECOUNT, _treecount);
                this.NotifyPropertyChanged(COUNTTREE.TREECOUNT);
            }
        }
        private Int64 _sumkpi;
        [XmlElement]
        [Field(Name = "SumKPI")]
        public virtual Int64 SumKPI
        {
            get
            {
                return _sumkpi;
            }
            set
            {
                if (_sumkpi == value) { return; }
                _sumkpi = value;
                this.ValidateProperty(COUNTTREE.SUMKPI, _sumkpi);
                this.NotifyPropertyChanged(COUNTTREE.SUMKPI);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TreeCount", this.TreeCount) && isValid;
            isValid = ValidateProperty("SumKPI", this.SumKPI) && isValid;
            isValid = ValidateProperty("SampleGroup_CN", this.SampleGroup_CN) && isValid;
            isValid = ValidateProperty("CuttingUnit_CN", this.CuttingUnit_CN) && isValid;
            isValid = ValidateProperty("Tally_CN", this.Tally_CN) && isValid;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            isValid = ValidateProperty("Component_CN", this.Component_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as CountTreeDO);
        }

        public void SetValues(CountTreeDO obj)
        {
            if (obj == null) { return; }
            TreeCount = obj.TreeCount;
            SumKPI = obj.SumKPI;
        }
    }
    [Table("Tally")]
    public partial class TallyDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TallyDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Hotkey", "Tally", "Hotkey is Required"));
            _validator.Add(new NotNullRule("Description", "Tally", "Description is Required"));
        }

        public TallyDO() { }

        public TallyDO(TallyDO obj) : this()
        {
            SetValues(obj);
        }

        public TallyDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Tally_CN")]
        public Int64? Tally_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _hotkey;
        [XmlElement]
        [Field(Name = "Hotkey")]
        public virtual String Hotkey
        {
            get
            {
                return _hotkey;
            }
            set
            {
                if (_hotkey == value) { return; }
                _hotkey = value;
                this.ValidateProperty(TALLY.HOTKEY, _hotkey);
                this.NotifyPropertyChanged(TALLY.HOTKEY);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(TALLY.DESCRIPTION, _description);
                this.NotifyPropertyChanged(TALLY.DESCRIPTION);
            }
        }
        private String _indicatorvalue;
        [XmlElement]
        [Field(Name = "IndicatorValue")]
        public virtual String IndicatorValue
        {
            get
            {
                return _indicatorvalue;
            }
            set
            {
                if (_indicatorvalue == value) { return; }
                _indicatorvalue = value;
                this.ValidateProperty(TALLY.INDICATORVALUE, _indicatorvalue);
                this.NotifyPropertyChanged(TALLY.INDICATORVALUE);
            }
        }
        private String _indicatortype;
        [XmlElement]
        [Field(Name = "IndicatorType")]
        public virtual String IndicatorType
        {
            get
            {
                return _indicatortype;
            }
            set
            {
                if (_indicatortype == value) { return; }
                _indicatortype = value;
                this.ValidateProperty(TALLY.INDICATORTYPE, _indicatortype);
                this.NotifyPropertyChanged(TALLY.INDICATORTYPE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Hotkey", this.Hotkey) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("IndicatorValue", this.IndicatorValue) && isValid;
            isValid = ValidateProperty("IndicatorType", this.IndicatorType) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TallyDO);
        }

        public void SetValues(TallyDO obj)
        {
            if (obj == null) { return; }
            Hotkey = obj.Hotkey;
            Description = obj.Description;
            IndicatorValue = obj.IndicatorValue;
            IndicatorType = obj.IndicatorType;
        }
    }
    [Table("TreeEstimate")]
    public partial class TreeEstimateDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeEstimateDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("KPI", "TreeEstimate", "KPI is Required"));
        }

        public TreeEstimateDO() { }

        public TreeEstimateDO(TreeEstimateDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeEstimateDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "TreeEstimate_CN")]
        public Int64? TreeEstimate_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _counttree_cn;
        [XmlIgnore]
        [Field(Name = "CountTree_CN")]
        public virtual long? CountTree_CN
        {
            get
            {

                if (_counttree != null)
                {
                    return _counttree.CountTree_CN;
                }
                return _counttree_cn;
            }
            set
            {
                if (_counttree_cn == value) { return; }
                if (value == null || value.Value == 0) { _counttree = null; }
                _counttree_cn = value;
                this.ValidateProperty(TREEESTIMATE.COUNTTREE_CN, _counttree_cn);
                this.NotifyPropertyChanged(TREEESTIMATE.COUNTTREE_CN);
            }
        }
        public virtual CountTreeDO GetCountTree()
        {
            if (DAL == null) { return null; }
            if (this.CountTree_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<CountTreeDO>(this.CountTree_CN);
            //return DAL.ReadSingleRow<CountTreeDO>(COUNTTREE._NAME, this.CountTree_CN);
        }

        private CountTreeDO _counttree = null;
        [XmlIgnore]
        public CountTreeDO CountTree
        {
            get
            {
                if (_counttree == null)
                {
                    _counttree = GetCountTree();
                }
                return _counttree;
            }
            set
            {
                if (_counttree == value) { return; }
                _counttree = value;
                CountTree_CN = (value != null) ? value.CountTree_CN : null;
            }
        }
        private Guid? _treeestimate_guid;
        [XmlElement]
        [Field(Name = "TreeEstimate_GUID")]
        public virtual Guid? TreeEstimate_GUID
        {
            get
            {
                return _treeestimate_guid;
            }
            set
            {
                if (_treeestimate_guid == value) { return; }
                _treeestimate_guid = value;
                this.ValidateProperty(TREEESTIMATE.TREEESTIMATE_GUID, _treeestimate_guid);
                this.NotifyPropertyChanged(TREEESTIMATE.TREEESTIMATE_GUID);
            }
        }
        private float _kpi = 0.0f;
        [XmlElement]
        [Field(Name = "KPI")]
        public virtual float KPI
        {
            get
            {
                return _kpi;
            }
            set
            {
                if (Math.Abs(_kpi - value) < float.Epsilon) { return; }
                _kpi = value;
                this.ValidateProperty(TREEESTIMATE.KPI, _kpi);
                this.NotifyPropertyChanged(TREEESTIMATE.KPI);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TreeEstimate_GUID", this.TreeEstimate_GUID) && isValid;
            isValid = ValidateProperty("KPI", this.KPI) && isValid;
            isValid = ValidateProperty("CountTree_CN", this.CountTree_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeEstimateDO);
        }

        public void SetValues(TreeEstimateDO obj)
        {
            if (obj == null) { return; }
            TreeEstimate_GUID = obj.TreeEstimate_GUID;
            KPI = obj.KPI;
        }
    }
    [Table("FixCNTTallyClass")]
    public partial class FixCNTTallyClassDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static FixCNTTallyClassDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "FixCNTTallyClass", "Stratum_CN is Required"));
        }

        public FixCNTTallyClassDO() { }

        public FixCNTTallyClassDO(FixCNTTallyClassDO obj) : this()
        {
            SetValues(obj);
        }

        public FixCNTTallyClassDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "FixCNTTallyClass_CN")]
        public Int64? FixCNTTallyClass_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(FIXCNTTALLYCLASS.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(FIXCNTTALLYCLASS.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private Int64 _fieldname;
        [XmlElement]
        [Field(Name = "FieldName")]
        public virtual Int64 FieldName
        {
            get
            {
                return _fieldname;
            }
            set
            {
                if (_fieldname == value) { return; }
                _fieldname = value;
                this.ValidateProperty(FIXCNTTALLYCLASS.FIELDNAME, _fieldname);
                this.NotifyPropertyChanged(FIXCNTTALLYCLASS.FIELDNAME);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("FieldName", this.FieldName) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as FixCNTTallyClassDO);
        }

        public void SetValues(FixCNTTallyClassDO obj)
        {
            if (obj == null) { return; }
            FieldName = obj.FieldName;
        }
    }
    [Table("FixCNTTallyPopulation")]
    public partial class FixCNTTallyPopulationDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static FixCNTTallyPopulationDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("FixCNTTallyClass_CN", "FixCNTTallyPopulation", "FixCNTTallyClass_CN is Required"));
            _validator.Add(new NotNullRule("SampleGroup_CN", "FixCNTTallyPopulation", "SampleGroup_CN is Required"));
            _validator.Add(new NotNullRule("TreeDefaultValue_CN", "FixCNTTallyPopulation", "TreeDefaultValue_CN is Required"));
        }

        public FixCNTTallyPopulationDO() { }

        public FixCNTTallyPopulationDO(FixCNTTallyPopulationDO obj) : this()
        {
            SetValues(obj);
        }

        public FixCNTTallyPopulationDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "FixCNTTallyPopulation_CN")]
        public Int64? FixCNTTallyPopulation_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _fixcnttallyclass_cn;
        [XmlIgnore]
        [Field(Name = "FixCNTTallyClass_CN")]
        public virtual long? FixCNTTallyClass_CN
        {
            get
            {

                if (_fixcnttallyclass != null)
                {
                    return _fixcnttallyclass.FixCNTTallyClass_CN;
                }
                return _fixcnttallyclass_cn;
            }
            set
            {
                if (_fixcnttallyclass_cn == value) { return; }
                if (value == null || value.Value == 0) { _fixcnttallyclass = null; }
                _fixcnttallyclass_cn = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.FIXCNTTALLYCLASS_CN, _fixcnttallyclass_cn);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.FIXCNTTALLYCLASS_CN);
            }
        }
        public virtual FixCNTTallyClassDO GetFixCNTTallyClass()
        {
            if (DAL == null) { return null; }
            if (this.FixCNTTallyClass_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<FixCNTTallyClassDO>(this.FixCNTTallyClass_CN);
            //return DAL.ReadSingleRow<FixCNTTallyClassDO>(FIXCNTTALLYCLASS._NAME, this.FixCNTTallyClass_CN);
        }

        private FixCNTTallyClassDO _fixcnttallyclass = null;
        [XmlIgnore]
        public FixCNTTallyClassDO FixCNTTallyClass
        {
            get
            {
                if (_fixcnttallyclass == null)
                {
                    _fixcnttallyclass = GetFixCNTTallyClass();
                }
                return _fixcnttallyclass;
            }
            set
            {
                if (_fixcnttallyclass == value) { return; }
                _fixcnttallyclass = value;
                FixCNTTallyClass_CN = (value != null) ? value.FixCNTTallyClass_CN : null;
            }
        }
        private long? _samplegroup_cn;
        [XmlIgnore]
        [Field(Name = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN
        {
            get
            {

                if (_samplegroup != null)
                {
                    return _samplegroup.SampleGroup_CN;
                }
                return _samplegroup_cn;
            }
            set
            {
                if (_samplegroup_cn == value) { return; }
                if (value == null || value.Value == 0) { _samplegroup = null; }
                _samplegroup_cn = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.SAMPLEGROUP_CN, _samplegroup_cn);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.SAMPLEGROUP_CN);
            }
        }
        public virtual SampleGroupDO GetSampleGroup()
        {
            if (DAL == null) { return null; }
            if (this.SampleGroup_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<SampleGroupDO>(this.SampleGroup_CN);
            //return DAL.ReadSingleRow<SampleGroupDO>(SAMPLEGROUP._NAME, this.SampleGroup_CN);
        }

        private SampleGroupDO _samplegroup = null;
        [XmlIgnore]
        public SampleGroupDO SampleGroup
        {
            get
            {
                if (_samplegroup == null)
                {
                    _samplegroup = GetSampleGroup();
                }
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                SampleGroup_CN = (value != null) ? value.SampleGroup_CN : null;
            }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }
        private Int64 _intervalsize;
        [XmlElement]
        [Field(Name = "IntervalSize")]
        public virtual Int64 IntervalSize
        {
            get
            {
                return _intervalsize;
            }
            set
            {
                if (_intervalsize == value) { return; }
                _intervalsize = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.INTERVALSIZE, _intervalsize);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.INTERVALSIZE);
            }
        }
        private Int64 _min;
        [XmlElement]
        [Field(Name = "Min")]
        public virtual Int64 Min
        {
            get
            {
                return _min;
            }
            set
            {
                if (_min == value) { return; }
                _min = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.MIN, _min);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.MIN);
            }
        }
        private Int64 _max;
        [XmlElement]
        [Field(Name = "Max")]
        public virtual Int64 Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (_max == value) { return; }
                _max = value;
                this.ValidateProperty(FIXCNTTALLYPOPULATION.MAX, _max);
                this.NotifyPropertyChanged(FIXCNTTALLYPOPULATION.MAX);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("IntervalSize", this.IntervalSize) && isValid;
            isValid = ValidateProperty("Min", this.Min) && isValid;
            isValid = ValidateProperty("Max", this.Max) && isValid;
            isValid = ValidateProperty("FixCNTTallyClass_CN", this.FixCNTTallyClass_CN) && isValid;
            isValid = ValidateProperty("SampleGroup_CN", this.SampleGroup_CN) && isValid;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as FixCNTTallyPopulationDO);
        }

        public void SetValues(FixCNTTallyPopulationDO obj)
        {
            if (obj == null) { return; }
            IntervalSize = obj.IntervalSize;
            Min = obj.Min;
            Max = obj.Max;
        }
    }
    #endregion
    #region Processing Tables
    [Table("VolumeEquation")]
    public partial class VolumeEquationDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static VolumeEquationDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Species", "VolumeEquation", "Species is Required"));
            _validator.Add(new NotNullRule("PrimaryProduct", "VolumeEquation", "PrimaryProduct is Required"));
            _validator.Add(new NotNullRule("VolumeEquationNumber", "VolumeEquation", "VolumeEquationNumber is Required"));
        }

        public VolumeEquationDO() { }

        public VolumeEquationDO(VolumeEquationDO obj) : this()
        {
            SetValues(obj);
        }

        public VolumeEquationDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(VOLUMEEQUATION.SPECIES, _species);
                this.NotifyPropertyChanged(VOLUMEEQUATION.SPECIES);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(VOLUMEEQUATION.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(VOLUMEEQUATION.PRIMARYPRODUCT);
            }
        }
        private String _volumeequationnumber;
        [XmlElement]
        [Field(Name = "VolumeEquationNumber")]
        public virtual String VolumeEquationNumber
        {
            get
            {
                return _volumeequationnumber;
            }
            set
            {
                if (_volumeequationnumber == value) { return; }
                _volumeequationnumber = value;
                this.ValidateProperty(VOLUMEEQUATION.VOLUMEEQUATIONNUMBER, _volumeequationnumber);
                this.NotifyPropertyChanged(VOLUMEEQUATION.VOLUMEEQUATIONNUMBER);
            }
        }
        private float _stumpheight = 0.0f;
        [XmlElement]
        [Field(Name = "StumpHeight")]
        public virtual float StumpHeight
        {
            get
            {
                return _stumpheight;
            }
            set
            {
                if (Math.Abs(_stumpheight - value) < float.Epsilon) { return; }
                _stumpheight = value;
                this.ValidateProperty(VOLUMEEQUATION.STUMPHEIGHT, _stumpheight);
                this.NotifyPropertyChanged(VOLUMEEQUATION.STUMPHEIGHT);
            }
        }
        private float _topdibprimary = 0.0f;
        [XmlElement]
        [Field(Name = "TopDIBPrimary")]
        public virtual float TopDIBPrimary
        {
            get
            {
                return _topdibprimary;
            }
            set
            {
                if (Math.Abs(_topdibprimary - value) < float.Epsilon) { return; }
                _topdibprimary = value;
                this.ValidateProperty(VOLUMEEQUATION.TOPDIBPRIMARY, _topdibprimary);
                this.NotifyPropertyChanged(VOLUMEEQUATION.TOPDIBPRIMARY);
            }
        }
        private float _topdibsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "TopDIBSecondary")]
        public virtual float TopDIBSecondary
        {
            get
            {
                return _topdibsecondary;
            }
            set
            {
                if (Math.Abs(_topdibsecondary - value) < float.Epsilon) { return; }
                _topdibsecondary = value;
                this.ValidateProperty(VOLUMEEQUATION.TOPDIBSECONDARY, _topdibsecondary);
                this.NotifyPropertyChanged(VOLUMEEQUATION.TOPDIBSECONDARY);
            }
        }
        private Int64 _calctotal;
        [XmlElement]
        [Field(Name = "CalcTotal")]
        public virtual Int64 CalcTotal
        {
            get
            {
                return _calctotal;
            }
            set
            {
                if (_calctotal == value) { return; }
                _calctotal = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCTOTAL, _calctotal);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCTOTAL);
            }
        }
        private Int64 _calcboard;
        [XmlElement]
        [Field(Name = "CalcBoard")]
        public virtual Int64 CalcBoard
        {
            get
            {
                return _calcboard;
            }
            set
            {
                if (_calcboard == value) { return; }
                _calcboard = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCBOARD, _calcboard);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCBOARD);
            }
        }
        private Int64 _calccubic;
        [XmlElement]
        [Field(Name = "CalcCubic")]
        public virtual Int64 CalcCubic
        {
            get
            {
                return _calccubic;
            }
            set
            {
                if (_calccubic == value) { return; }
                _calccubic = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCCUBIC, _calccubic);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCCUBIC);
            }
        }
        private Int64 _calccord;
        [XmlElement]
        [Field(Name = "CalcCord")]
        public virtual Int64 CalcCord
        {
            get
            {
                return _calccord;
            }
            set
            {
                if (_calccord == value) { return; }
                _calccord = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCCORD, _calccord);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCCORD);
            }
        }
        private Int64 _calctopwood;
        [XmlElement]
        [Field(Name = "CalcTopwood")]
        public virtual Int64 CalcTopwood
        {
            get
            {
                return _calctopwood;
            }
            set
            {
                if (_calctopwood == value) { return; }
                _calctopwood = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCTOPWOOD, _calctopwood);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCTOPWOOD);
            }
        }
        private Int64 _calcbiomass;
        [XmlElement]
        [Field(Name = "CalcBiomass")]
        public virtual Int64 CalcBiomass
        {
            get
            {
                return _calcbiomass;
            }
            set
            {
                if (_calcbiomass == value) { return; }
                _calcbiomass = value;
                this.ValidateProperty(VOLUMEEQUATION.CALCBIOMASS, _calcbiomass);
                this.NotifyPropertyChanged(VOLUMEEQUATION.CALCBIOMASS);
            }
        }
        private float _trim = 0.0f;
        [XmlElement]
        [Field(Name = "Trim")]
        public virtual float Trim
        {
            get
            {
                return _trim;
            }
            set
            {
                if (Math.Abs(_trim - value) < float.Epsilon) { return; }
                _trim = value;
                this.ValidateProperty(VOLUMEEQUATION.TRIM, _trim);
                this.NotifyPropertyChanged(VOLUMEEQUATION.TRIM);
            }
        }
        private Int64 _segmentationlogic;
        [XmlElement]
        [Field(Name = "SegmentationLogic")]
        public virtual Int64 SegmentationLogic
        {
            get
            {
                return _segmentationlogic;
            }
            set
            {
                if (_segmentationlogic == value) { return; }
                _segmentationlogic = value;
                this.ValidateProperty(VOLUMEEQUATION.SEGMENTATIONLOGIC, _segmentationlogic);
                this.NotifyPropertyChanged(VOLUMEEQUATION.SEGMENTATIONLOGIC);
            }
        }
        private float _minloglengthprimary = 0.0f;
        [XmlElement]
        [Field(Name = "MinLogLengthPrimary")]
        public virtual float MinLogLengthPrimary
        {
            get
            {
                return _minloglengthprimary;
            }
            set
            {
                if (Math.Abs(_minloglengthprimary - value) < float.Epsilon) { return; }
                _minloglengthprimary = value;
                this.ValidateProperty(VOLUMEEQUATION.MINLOGLENGTHPRIMARY, _minloglengthprimary);
                this.NotifyPropertyChanged(VOLUMEEQUATION.MINLOGLENGTHPRIMARY);
            }
        }
        private float _maxloglengthprimary = 0.0f;
        [XmlElement]
        [Field(Name = "MaxLogLengthPrimary")]
        public virtual float MaxLogLengthPrimary
        {
            get
            {
                return _maxloglengthprimary;
            }
            set
            {
                if (Math.Abs(_maxloglengthprimary - value) < float.Epsilon) { return; }
                _maxloglengthprimary = value;
                this.ValidateProperty(VOLUMEEQUATION.MAXLOGLENGTHPRIMARY, _maxloglengthprimary);
                this.NotifyPropertyChanged(VOLUMEEQUATION.MAXLOGLENGTHPRIMARY);
            }
        }
        private float _minmerchlength = 0.0f;
        [XmlElement]
        [Field(Name = "MinMerchLength")]
        public virtual float MinMerchLength
        {
            get
            {
                return _minmerchlength;
            }
            set
            {
                if (Math.Abs(_minmerchlength - value) < float.Epsilon) { return; }
                _minmerchlength = value;
                this.ValidateProperty(VOLUMEEQUATION.MINMERCHLENGTH, _minmerchlength);
                this.NotifyPropertyChanged(VOLUMEEQUATION.MINMERCHLENGTH);
            }
        }
        private String _model;
        [XmlElement]
        [Field(Name = "Model")]
        public virtual String Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model == value) { return; }
                _model = value;
                this.ValidateProperty(VOLUMEEQUATION.MODEL, _model);
                this.NotifyPropertyChanged(VOLUMEEQUATION.MODEL);
            }
        }
        private String _commonspeciesname;
        [XmlElement]
        [Field(Name = "CommonSpeciesName")]
        public virtual String CommonSpeciesName
        {
            get
            {
                return _commonspeciesname;
            }
            set
            {
                if (_commonspeciesname == value) { return; }
                _commonspeciesname = value;
                this.ValidateProperty(VOLUMEEQUATION.COMMONSPECIESNAME, _commonspeciesname);
                this.NotifyPropertyChanged(VOLUMEEQUATION.COMMONSPECIESNAME);
            }
        }
        private Int64 _merchmodflag;
        [XmlElement]
        [Field(Name = "MerchModFlag")]
        public virtual Int64 MerchModFlag
        {
            get
            {
                return _merchmodflag;
            }
            set
            {
                if (_merchmodflag == value) { return; }
                _merchmodflag = value;
                this.ValidateProperty(VOLUMEEQUATION.MERCHMODFLAG, _merchmodflag);
                this.NotifyPropertyChanged(VOLUMEEQUATION.MERCHMODFLAG);
            }
        }
        private Int64 _evenoddsegment;
        [XmlElement]
        [Field(Name = "EvenOddSegment")]
        public virtual Int64 EvenOddSegment
        {
            get
            {
                return _evenoddsegment;
            }
            set
            {
                if (_evenoddsegment == value) { return; }
                _evenoddsegment = value;
                this.ValidateProperty(VOLUMEEQUATION.EVENODDSEGMENT, _evenoddsegment);
                this.NotifyPropertyChanged(VOLUMEEQUATION.EVENODDSEGMENT);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("VolumeEquationNumber", this.VolumeEquationNumber) && isValid;
            isValid = ValidateProperty("StumpHeight", this.StumpHeight) && isValid;
            isValid = ValidateProperty("TopDIBPrimary", this.TopDIBPrimary) && isValid;
            isValid = ValidateProperty("TopDIBSecondary", this.TopDIBSecondary) && isValid;
            isValid = ValidateProperty("CalcTotal", this.CalcTotal) && isValid;
            isValid = ValidateProperty("CalcBoard", this.CalcBoard) && isValid;
            isValid = ValidateProperty("CalcCubic", this.CalcCubic) && isValid;
            isValid = ValidateProperty("CalcCord", this.CalcCord) && isValid;
            isValid = ValidateProperty("CalcTopwood", this.CalcTopwood) && isValid;
            isValid = ValidateProperty("CalcBiomass", this.CalcBiomass) && isValid;
            isValid = ValidateProperty("Trim", this.Trim) && isValid;
            isValid = ValidateProperty("SegmentationLogic", this.SegmentationLogic) && isValid;
            isValid = ValidateProperty("MinLogLengthPrimary", this.MinLogLengthPrimary) && isValid;
            isValid = ValidateProperty("MaxLogLengthPrimary", this.MaxLogLengthPrimary) && isValid;
            isValid = ValidateProperty("MinMerchLength", this.MinMerchLength) && isValid;
            isValid = ValidateProperty("Model", this.Model) && isValid;
            isValid = ValidateProperty("CommonSpeciesName", this.CommonSpeciesName) && isValid;
            isValid = ValidateProperty("MerchModFlag", this.MerchModFlag) && isValid;
            isValid = ValidateProperty("EvenOddSegment", this.EvenOddSegment) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as VolumeEquationDO);
        }

        public void SetValues(VolumeEquationDO obj)
        {
            if (obj == null) { return; }
            Species = obj.Species;
            PrimaryProduct = obj.PrimaryProduct;
            VolumeEquationNumber = obj.VolumeEquationNumber;
            StumpHeight = obj.StumpHeight;
            TopDIBPrimary = obj.TopDIBPrimary;
            TopDIBSecondary = obj.TopDIBSecondary;
            CalcTotal = obj.CalcTotal;
            CalcBoard = obj.CalcBoard;
            CalcCubic = obj.CalcCubic;
            CalcCord = obj.CalcCord;
            CalcTopwood = obj.CalcTopwood;
            CalcBiomass = obj.CalcBiomass;
            Trim = obj.Trim;
            SegmentationLogic = obj.SegmentationLogic;
            MinLogLengthPrimary = obj.MinLogLengthPrimary;
            MaxLogLengthPrimary = obj.MaxLogLengthPrimary;
            MinMerchLength = obj.MinMerchLength;
            Model = obj.Model;
            CommonSpeciesName = obj.CommonSpeciesName;
            MerchModFlag = obj.MerchModFlag;
            EvenOddSegment = obj.EvenOddSegment;
        }
    }
    [Table("BiomassEquation")]
    public partial class BiomassEquationDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static BiomassEquationDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Species", "BiomassEquation", "Species is Required"));
            _validator.Add(new NotNullRule("Product", "BiomassEquation", "Product is Required"));
            _validator.Add(new NotNullRule("Component", "BiomassEquation", "Component is Required"));
            _validator.Add(new NotNullRule("LiveDead", "BiomassEquation", "LiveDead is Required"));
            _validator.Add(new NotNullRule("FIAcode", "BiomassEquation", "FIAcode is Required"));
        }

        public BiomassEquationDO() { }

        public BiomassEquationDO(BiomassEquationDO obj) : this()
        {
            SetValues(obj);
        }

        public BiomassEquationDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(BIOMASSEQUATION.SPECIES, _species);
                this.NotifyPropertyChanged(BIOMASSEQUATION.SPECIES);
            }
        }
        private String _product;
        [XmlElement]
        [Field(Name = "Product")]
        public virtual String Product
        {
            get
            {
                return _product;
            }
            set
            {
                if (_product == value) { return; }
                _product = value;
                this.ValidateProperty(BIOMASSEQUATION.PRODUCT, _product);
                this.NotifyPropertyChanged(BIOMASSEQUATION.PRODUCT);
            }
        }
        private String _component;
        [XmlElement]
        [Field(Name = "Component")]
        public virtual String Component
        {
            get
            {
                return _component;
            }
            set
            {
                if (_component == value) { return; }
                _component = value;
                this.ValidateProperty(BIOMASSEQUATION.COMPONENT, _component);
                this.NotifyPropertyChanged(BIOMASSEQUATION.COMPONENT);
            }
        }
        private String _livedead;
        [XmlElement]
        [Field(Name = "LiveDead")]
        public virtual String LiveDead
        {
            get
            {
                return _livedead;
            }
            set
            {
                if (_livedead == value) { return; }
                _livedead = value;
                this.ValidateProperty(BIOMASSEQUATION.LIVEDEAD, _livedead);
                this.NotifyPropertyChanged(BIOMASSEQUATION.LIVEDEAD);
            }
        }
        private Int64 _fiacode;
        [XmlElement]
        [Field(Name = "FIAcode")]
        public virtual Int64 FIAcode
        {
            get
            {
                return _fiacode;
            }
            set
            {
                if (_fiacode == value) { return; }
                _fiacode = value;
                this.ValidateProperty(BIOMASSEQUATION.FIACODE, _fiacode);
                this.NotifyPropertyChanged(BIOMASSEQUATION.FIACODE);
            }
        }
        private String _equation;
        [XmlElement]
        [Field(Name = "Equation")]
        public virtual String Equation
        {
            get
            {
                return _equation;
            }
            set
            {
                if (_equation == value) { return; }
                _equation = value;
                this.ValidateProperty(BIOMASSEQUATION.EQUATION, _equation);
                this.NotifyPropertyChanged(BIOMASSEQUATION.EQUATION);
            }
        }
        private float _percentmoisture = 0.0f;
        [XmlElement]
        [Field(Name = "PercentMoisture")]
        public virtual float PercentMoisture
        {
            get
            {
                return _percentmoisture;
            }
            set
            {
                if (Math.Abs(_percentmoisture - value) < float.Epsilon) { return; }
                _percentmoisture = value;
                this.ValidateProperty(BIOMASSEQUATION.PERCENTMOISTURE, _percentmoisture);
                this.NotifyPropertyChanged(BIOMASSEQUATION.PERCENTMOISTURE);
            }
        }
        private float _percentremoved = 0.0f;
        [XmlElement]
        [Field(Name = "PercentRemoved")]
        public virtual float PercentRemoved
        {
            get
            {
                return _percentremoved;
            }
            set
            {
                if (Math.Abs(_percentremoved - value) < float.Epsilon) { return; }
                _percentremoved = value;
                this.ValidateProperty(BIOMASSEQUATION.PERCENTREMOVED, _percentremoved);
                this.NotifyPropertyChanged(BIOMASSEQUATION.PERCENTREMOVED);
            }
        }
        private String _metadata;
        [XmlElement]
        [Field(Name = "MetaData")]
        public virtual String MetaData
        {
            get
            {
                return _metadata;
            }
            set
            {
                if (_metadata == value) { return; }
                _metadata = value;
                this.ValidateProperty(BIOMASSEQUATION.METADATA, _metadata);
                this.NotifyPropertyChanged(BIOMASSEQUATION.METADATA);
            }
        }
        private float _weightfactorprimary = 0.0f;
        [XmlElement]
        [Field(Name = "WeightFactorPrimary")]
        public virtual float WeightFactorPrimary
        {
            get
            {
                return _weightfactorprimary;
            }
            set
            {
                if (Math.Abs(_weightfactorprimary - value) < float.Epsilon) { return; }
                _weightfactorprimary = value;
                this.ValidateProperty(BIOMASSEQUATION.WEIGHTFACTORPRIMARY, _weightfactorprimary);
                this.NotifyPropertyChanged(BIOMASSEQUATION.WEIGHTFACTORPRIMARY);
            }
        }
        private float _weightfactorsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "WeightFactorSecondary")]
        public virtual float WeightFactorSecondary
        {
            get
            {
                return _weightfactorsecondary;
            }
            set
            {
                if (Math.Abs(_weightfactorsecondary - value) < float.Epsilon) { return; }
                _weightfactorsecondary = value;
                this.ValidateProperty(BIOMASSEQUATION.WEIGHTFACTORSECONDARY, _weightfactorsecondary);
                this.NotifyPropertyChanged(BIOMASSEQUATION.WEIGHTFACTORSECONDARY);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("Product", this.Product) && isValid;
            isValid = ValidateProperty("Component", this.Component) && isValid;
            isValid = ValidateProperty("LiveDead", this.LiveDead) && isValid;
            isValid = ValidateProperty("FIAcode", this.FIAcode) && isValid;
            isValid = ValidateProperty("Equation", this.Equation) && isValid;
            isValid = ValidateProperty("PercentMoisture", this.PercentMoisture) && isValid;
            isValid = ValidateProperty("PercentRemoved", this.PercentRemoved) && isValid;
            isValid = ValidateProperty("MetaData", this.MetaData) && isValid;
            isValid = ValidateProperty("WeightFactorPrimary", this.WeightFactorPrimary) && isValid;
            isValid = ValidateProperty("WeightFactorSecondary", this.WeightFactorSecondary) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as BiomassEquationDO);
        }

        public void SetValues(BiomassEquationDO obj)
        {
            if (obj == null) { return; }
            Species = obj.Species;
            Product = obj.Product;
            Component = obj.Component;
            LiveDead = obj.LiveDead;
            FIAcode = obj.FIAcode;
            Equation = obj.Equation;
            PercentMoisture = obj.PercentMoisture;
            PercentRemoved = obj.PercentRemoved;
            MetaData = obj.MetaData;
            WeightFactorPrimary = obj.WeightFactorPrimary;
            WeightFactorSecondary = obj.WeightFactorSecondary;
        }
    }
    [Table("ValueEquation")]
    public partial class ValueEquationDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ValueEquationDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Species", "ValueEquation", "Species is Required"));
            _validator.Add(new NotNullRule("PrimaryProduct", "ValueEquation", "PrimaryProduct is Required"));
        }

        public ValueEquationDO() { }

        public ValueEquationDO(ValueEquationDO obj) : this()
        {
            SetValues(obj);
        }

        public ValueEquationDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(VALUEEQUATION.SPECIES, _species);
                this.NotifyPropertyChanged(VALUEEQUATION.SPECIES);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(VALUEEQUATION.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(VALUEEQUATION.PRIMARYPRODUCT);
            }
        }
        private String _valueequationnumber;
        [XmlElement]
        [Field(Name = "ValueEquationNumber")]
        public virtual String ValueEquationNumber
        {
            get
            {
                return _valueequationnumber;
            }
            set
            {
                if (_valueequationnumber == value) { return; }
                _valueequationnumber = value;
                this.ValidateProperty(VALUEEQUATION.VALUEEQUATIONNUMBER, _valueequationnumber);
                this.NotifyPropertyChanged(VALUEEQUATION.VALUEEQUATIONNUMBER);
            }
        }
        private String _grade;
        [XmlElement]
        [Field(Name = "Grade")]
        public virtual String Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (_grade == value) { return; }
                _grade = value;
                this.ValidateProperty(VALUEEQUATION.GRADE, _grade);
                this.NotifyPropertyChanged(VALUEEQUATION.GRADE);
            }
        }
        private float _coefficient1 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient1")]
        public virtual float Coefficient1
        {
            get
            {
                return _coefficient1;
            }
            set
            {
                if (Math.Abs(_coefficient1 - value) < float.Epsilon) { return; }
                _coefficient1 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT1, _coefficient1);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT1);
            }
        }
        private float _coefficient2 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient2")]
        public virtual float Coefficient2
        {
            get
            {
                return _coefficient2;
            }
            set
            {
                if (Math.Abs(_coefficient2 - value) < float.Epsilon) { return; }
                _coefficient2 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT2, _coefficient2);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT2);
            }
        }
        private float _coefficient3 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient3")]
        public virtual float Coefficient3
        {
            get
            {
                return _coefficient3;
            }
            set
            {
                if (Math.Abs(_coefficient3 - value) < float.Epsilon) { return; }
                _coefficient3 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT3, _coefficient3);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT3);
            }
        }
        private float _coefficient4 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient4")]
        public virtual float Coefficient4
        {
            get
            {
                return _coefficient4;
            }
            set
            {
                if (Math.Abs(_coefficient4 - value) < float.Epsilon) { return; }
                _coefficient4 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT4, _coefficient4);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT4);
            }
        }
        private float _coefficient5 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient5")]
        public virtual float Coefficient5
        {
            get
            {
                return _coefficient5;
            }
            set
            {
                if (Math.Abs(_coefficient5 - value) < float.Epsilon) { return; }
                _coefficient5 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT5, _coefficient5);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT5);
            }
        }
        private float _coefficient6 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient6")]
        public virtual float Coefficient6
        {
            get
            {
                return _coefficient6;
            }
            set
            {
                if (Math.Abs(_coefficient6 - value) < float.Epsilon) { return; }
                _coefficient6 = value;
                this.ValidateProperty(VALUEEQUATION.COEFFICIENT6, _coefficient6);
                this.NotifyPropertyChanged(VALUEEQUATION.COEFFICIENT6);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("ValueEquationNumber", this.ValueEquationNumber) && isValid;
            isValid = ValidateProperty("Grade", this.Grade) && isValid;
            isValid = ValidateProperty("Coefficient1", this.Coefficient1) && isValid;
            isValid = ValidateProperty("Coefficient2", this.Coefficient2) && isValid;
            isValid = ValidateProperty("Coefficient3", this.Coefficient3) && isValid;
            isValid = ValidateProperty("Coefficient4", this.Coefficient4) && isValid;
            isValid = ValidateProperty("Coefficient5", this.Coefficient5) && isValid;
            isValid = ValidateProperty("Coefficient6", this.Coefficient6) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ValueEquationDO);
        }

        public void SetValues(ValueEquationDO obj)
        {
            if (obj == null) { return; }
            Species = obj.Species;
            PrimaryProduct = obj.PrimaryProduct;
            ValueEquationNumber = obj.ValueEquationNumber;
            Grade = obj.Grade;
            Coefficient1 = obj.Coefficient1;
            Coefficient2 = obj.Coefficient2;
            Coefficient3 = obj.Coefficient3;
            Coefficient4 = obj.Coefficient4;
            Coefficient5 = obj.Coefficient5;
            Coefficient6 = obj.Coefficient6;
        }
    }
    [Table("QualityAdjEquation")]
    public partial class QualityAdjEquationDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static QualityAdjEquationDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Species", "QualityAdjEquation", "Species is Required"));
        }

        public QualityAdjEquationDO() { }

        public QualityAdjEquationDO(QualityAdjEquationDO obj) : this()
        {
            SetValues(obj);
        }

        public QualityAdjEquationDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(QUALITYADJEQUATION.SPECIES, _species);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.SPECIES);
            }
        }
        private String _qualityadjeq;
        [XmlElement]
        [Field(Name = "QualityAdjEq")]
        public virtual String QualityAdjEq
        {
            get
            {
                return _qualityadjeq;
            }
            set
            {
                if (_qualityadjeq == value) { return; }
                _qualityadjeq = value;
                this.ValidateProperty(QUALITYADJEQUATION.QUALITYADJEQ, _qualityadjeq);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.QUALITYADJEQ);
            }
        }
        private Int64 _year;
        [XmlElement]
        [Field(Name = "Year")]
        public virtual Int64 Year
        {
            get
            {
                return _year;
            }
            set
            {
                if (_year == value) { return; }
                _year = value;
                this.ValidateProperty(QUALITYADJEQUATION.YEAR, _year);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.YEAR);
            }
        }
        private String _grade;
        [XmlElement]
        [Field(Name = "Grade")]
        public virtual String Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (_grade == value) { return; }
                _grade = value;
                this.ValidateProperty(QUALITYADJEQUATION.GRADE, _grade);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.GRADE);
            }
        }
        private float _coefficient1 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient1")]
        public virtual float Coefficient1
        {
            get
            {
                return _coefficient1;
            }
            set
            {
                if (Math.Abs(_coefficient1 - value) < float.Epsilon) { return; }
                _coefficient1 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT1, _coefficient1);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT1);
            }
        }
        private float _coefficient2 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient2")]
        public virtual float Coefficient2
        {
            get
            {
                return _coefficient2;
            }
            set
            {
                if (Math.Abs(_coefficient2 - value) < float.Epsilon) { return; }
                _coefficient2 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT2, _coefficient2);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT2);
            }
        }
        private float _coefficient3 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient3")]
        public virtual float Coefficient3
        {
            get
            {
                return _coefficient3;
            }
            set
            {
                if (Math.Abs(_coefficient3 - value) < float.Epsilon) { return; }
                _coefficient3 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT3, _coefficient3);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT3);
            }
        }
        private float _coefficient4 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient4")]
        public virtual float Coefficient4
        {
            get
            {
                return _coefficient4;
            }
            set
            {
                if (Math.Abs(_coefficient4 - value) < float.Epsilon) { return; }
                _coefficient4 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT4, _coefficient4);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT4);
            }
        }
        private float _coefficient5 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient5")]
        public virtual float Coefficient5
        {
            get
            {
                return _coefficient5;
            }
            set
            {
                if (Math.Abs(_coefficient5 - value) < float.Epsilon) { return; }
                _coefficient5 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT5, _coefficient5);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT5);
            }
        }
        private float _coefficient6 = 0.0f;
        [XmlElement]
        [Field(Name = "Coefficient6")]
        public virtual float Coefficient6
        {
            get
            {
                return _coefficient6;
            }
            set
            {
                if (Math.Abs(_coefficient6 - value) < float.Epsilon) { return; }
                _coefficient6 = value;
                this.ValidateProperty(QUALITYADJEQUATION.COEFFICIENT6, _coefficient6);
                this.NotifyPropertyChanged(QUALITYADJEQUATION.COEFFICIENT6);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("QualityAdjEq", this.QualityAdjEq) && isValid;
            isValid = ValidateProperty("Year", this.Year) && isValid;
            isValid = ValidateProperty("Grade", this.Grade) && isValid;
            isValid = ValidateProperty("Coefficient1", this.Coefficient1) && isValid;
            isValid = ValidateProperty("Coefficient2", this.Coefficient2) && isValid;
            isValid = ValidateProperty("Coefficient3", this.Coefficient3) && isValid;
            isValid = ValidateProperty("Coefficient4", this.Coefficient4) && isValid;
            isValid = ValidateProperty("Coefficient5", this.Coefficient5) && isValid;
            isValid = ValidateProperty("Coefficient6", this.Coefficient6) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as QualityAdjEquationDO);
        }

        public void SetValues(QualityAdjEquationDO obj)
        {
            if (obj == null) { return; }
            Species = obj.Species;
            QualityAdjEq = obj.QualityAdjEq;
            Year = obj.Year;
            Grade = obj.Grade;
            Coefficient1 = obj.Coefficient1;
            Coefficient2 = obj.Coefficient2;
            Coefficient3 = obj.Coefficient3;
            Coefficient4 = obj.Coefficient4;
            Coefficient5 = obj.Coefficient5;
            Coefficient6 = obj.Coefficient6;
        }
    }
    [Table("Reports")]
    public partial class ReportsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ReportsDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("ReportID", "Reports", "ReportID is Required"));
        }

        public ReportsDO() { }

        public ReportsDO(ReportsDO obj) : this()
        {
            SetValues(obj);
        }

        public ReportsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _reportid;
        [XmlElement]
        [Field(Name = "ReportID")]
        public virtual String ReportID
        {
            get
            {
                return _reportid;
            }
            set
            {
                if (_reportid == value) { return; }
                _reportid = value;
                this.ValidateProperty(REPORTS.REPORTID, _reportid);
                this.NotifyPropertyChanged(REPORTS.REPORTID);
            }
        }
        private bool _selected = false;
        [XmlElement]
        [Field(Name = "Selected")]
        public virtual bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected == value) { return; }
                _selected = value;
                this.ValidateProperty(REPORTS.SELECTED, _selected);
                this.NotifyPropertyChanged(REPORTS.SELECTED);
            }
        }
        private String _title;
        [XmlElement]
        [Field(Name = "Title")]
        public virtual String Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value) { return; }
                _title = value;
                this.ValidateProperty(REPORTS.TITLE, _title);
                this.NotifyPropertyChanged(REPORTS.TITLE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("ReportID", this.ReportID) && isValid;
            isValid = ValidateProperty("Selected", this.Selected) && isValid;
            isValid = ValidateProperty("Title", this.Title) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ReportsDO);
        }

        public void SetValues(ReportsDO obj)
        {
            if (obj == null) { return; }
            ReportID = obj.ReportID;
            Selected = obj.Selected;
            Title = obj.Title;
        }
    }
    [Table("TreeCalculatedValues")]
    public partial class TreeCalculatedValuesDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeCalculatedValuesDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Tree_CN", "TreeCalculatedValues", "Tree_CN is Required"));
        }

        public TreeCalculatedValuesDO() { }

        public TreeCalculatedValuesDO(TreeCalculatedValuesDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeCalculatedValuesDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "TreeCalcValues_CN")]
        public Int64? TreeCalcValues_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _tree_cn;
        [XmlIgnore]
        [Field(Name = "Tree_CN")]
        public virtual long? Tree_CN
        {
            get
            {

                if (_tree != null)
                {
                    return _tree.Tree_CN;
                }
                return _tree_cn;
            }
            set
            {
                if (_tree_cn == value) { return; }
                if (value == null || value.Value == 0) { _tree = null; }
                _tree_cn = value;
                this.ValidateProperty(TREECALCULATEDVALUES.TREE_CN, _tree_cn);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.TREE_CN);
            }
        }
        public virtual TreeDO GetTree()
        {
            if (DAL == null) { return null; }
            if (this.Tree_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDO>(this.Tree_CN);
            //return DAL.ReadSingleRow<TreeDO>(TREE._NAME, this.Tree_CN);
        }

        private TreeDO _tree = null;
        [XmlIgnore]
        public TreeDO Tree
        {
            get
            {
                if (_tree == null)
                {
                    _tree = GetTree();
                }
                return _tree;
            }
            set
            {
                if (_tree == value) { return; }
                _tree = value;
                Tree_CN = (value != null) ? value.Tree_CN : null;
            }
        }
        private float _totalcubicvolume = 0.0f;
        [XmlElement]
        [Field(Name = "TotalCubicVolume")]
        public virtual float TotalCubicVolume
        {
            get
            {
                return _totalcubicvolume;
            }
            set
            {
                if (Math.Abs(_totalcubicvolume - value) < float.Epsilon) { return; }
                _totalcubicvolume = value;
                this.ValidateProperty(TREECALCULATEDVALUES.TOTALCUBICVOLUME, _totalcubicvolume);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.TOTALCUBICVOLUME);
            }
        }
        private float _grossbdftpp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBDFTPP")]
        public virtual float GrossBDFTPP
        {
            get
            {
                return _grossbdftpp;
            }
            set
            {
                if (Math.Abs(_grossbdftpp - value) < float.Epsilon) { return; }
                _grossbdftpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSBDFTPP, _grossbdftpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSBDFTPP);
            }
        }
        private float _netbdftpp = 0.0f;
        [XmlElement]
        [Field(Name = "NetBDFTPP")]
        public virtual float NetBDFTPP
        {
            get
            {
                return _netbdftpp;
            }
            set
            {
                if (Math.Abs(_netbdftpp - value) < float.Epsilon) { return; }
                _netbdftpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NETBDFTPP, _netbdftpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NETBDFTPP);
            }
        }
        private float _grosscuftpp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCUFTPP")]
        public virtual float GrossCUFTPP
        {
            get
            {
                return _grosscuftpp;
            }
            set
            {
                if (Math.Abs(_grosscuftpp - value) < float.Epsilon) { return; }
                _grosscuftpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSCUFTPP, _grosscuftpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSCUFTPP);
            }
        }
        private float _netcuftpp = 0.0f;
        [XmlElement]
        [Field(Name = "NetCUFTPP")]
        public virtual float NetCUFTPP
        {
            get
            {
                return _netcuftpp;
            }
            set
            {
                if (Math.Abs(_netcuftpp - value) < float.Epsilon) { return; }
                _netcuftpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NETCUFTPP, _netcuftpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NETCUFTPP);
            }
        }
        private float _cordspp = 0.0f;
        [XmlElement]
        [Field(Name = "CordsPP")]
        public virtual float CordsPP
        {
            get
            {
                return _cordspp;
            }
            set
            {
                if (Math.Abs(_cordspp - value) < float.Epsilon) { return; }
                _cordspp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.CORDSPP, _cordspp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.CORDSPP);
            }
        }
        private float _grossbdftremvpp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBDFTRemvPP")]
        public virtual float GrossBDFTRemvPP
        {
            get
            {
                return _grossbdftremvpp;
            }
            set
            {
                if (Math.Abs(_grossbdftremvpp - value) < float.Epsilon) { return; }
                _grossbdftremvpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSBDFTREMVPP, _grossbdftremvpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSBDFTREMVPP);
            }
        }
        private float _grosscuftremvpp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCUFTRemvPP")]
        public virtual float GrossCUFTRemvPP
        {
            get
            {
                return _grosscuftremvpp;
            }
            set
            {
                if (Math.Abs(_grosscuftremvpp - value) < float.Epsilon) { return; }
                _grosscuftremvpp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSCUFTREMVPP, _grosscuftremvpp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSCUFTREMVPP);
            }
        }
        private float _grossbdftsp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBDFTSP")]
        public virtual float GrossBDFTSP
        {
            get
            {
                return _grossbdftsp;
            }
            set
            {
                if (Math.Abs(_grossbdftsp - value) < float.Epsilon) { return; }
                _grossbdftsp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSBDFTSP, _grossbdftsp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSBDFTSP);
            }
        }
        private float _netbdftsp = 0.0f;
        [XmlElement]
        [Field(Name = "NetBDFTSP")]
        public virtual float NetBDFTSP
        {
            get
            {
                return _netbdftsp;
            }
            set
            {
                if (Math.Abs(_netbdftsp - value) < float.Epsilon) { return; }
                _netbdftsp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NETBDFTSP, _netbdftsp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NETBDFTSP);
            }
        }
        private float _grosscuftsp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCUFTSP")]
        public virtual float GrossCUFTSP
        {
            get
            {
                return _grosscuftsp;
            }
            set
            {
                if (Math.Abs(_grosscuftsp - value) < float.Epsilon) { return; }
                _grosscuftsp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSCUFTSP, _grosscuftsp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSCUFTSP);
            }
        }
        private float _netcuftsp = 0.0f;
        [XmlElement]
        [Field(Name = "NetCUFTSP")]
        public virtual float NetCUFTSP
        {
            get
            {
                return _netcuftsp;
            }
            set
            {
                if (Math.Abs(_netcuftsp - value) < float.Epsilon) { return; }
                _netcuftsp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NETCUFTSP, _netcuftsp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NETCUFTSP);
            }
        }
        private float _cordssp = 0.0f;
        [XmlElement]
        [Field(Name = "CordsSP")]
        public virtual float CordsSP
        {
            get
            {
                return _cordssp;
            }
            set
            {
                if (Math.Abs(_cordssp - value) < float.Epsilon) { return; }
                _cordssp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.CORDSSP, _cordssp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.CORDSSP);
            }
        }
        private float _grosscuftremvsp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCUFTRemvSP")]
        public virtual float GrossCUFTRemvSP
        {
            get
            {
                return _grosscuftremvsp;
            }
            set
            {
                if (Math.Abs(_grosscuftremvsp - value) < float.Epsilon) { return; }
                _grosscuftremvsp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSCUFTREMVSP, _grosscuftremvsp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSCUFTREMVSP);
            }
        }
        private float _numberlogsms = 0.0f;
        [XmlElement]
        [Field(Name = "NumberlogsMS")]
        public virtual float NumberlogsMS
        {
            get
            {
                return _numberlogsms;
            }
            set
            {
                if (Math.Abs(_numberlogsms - value) < float.Epsilon) { return; }
                _numberlogsms = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NUMBERLOGSMS, _numberlogsms);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NUMBERLOGSMS);
            }
        }
        private float _numberlogstpw = 0.0f;
        [XmlElement]
        [Field(Name = "NumberlogsTPW")]
        public virtual float NumberlogsTPW
        {
            get
            {
                return _numberlogstpw;
            }
            set
            {
                if (Math.Abs(_numberlogstpw - value) < float.Epsilon) { return; }
                _numberlogstpw = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NUMBERLOGSTPW, _numberlogstpw);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NUMBERLOGSTPW);
            }
        }
        private float _grossbdftrp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBDFTRP")]
        public virtual float GrossBDFTRP
        {
            get
            {
                return _grossbdftrp;
            }
            set
            {
                if (Math.Abs(_grossbdftrp - value) < float.Epsilon) { return; }
                _grossbdftrp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSBDFTRP, _grossbdftrp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSBDFTRP);
            }
        }
        private float _grosscuftrp = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCUFTRP")]
        public virtual float GrossCUFTRP
        {
            get
            {
                return _grosscuftrp;
            }
            set
            {
                if (Math.Abs(_grosscuftrp - value) < float.Epsilon) { return; }
                _grosscuftrp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSCUFTRP, _grosscuftrp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSCUFTRP);
            }
        }
        private float _cordsrp = 0.0f;
        [XmlElement]
        [Field(Name = "CordsRP")]
        public virtual float CordsRP
        {
            get
            {
                return _cordsrp;
            }
            set
            {
                if (Math.Abs(_cordsrp - value) < float.Epsilon) { return; }
                _cordsrp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.CORDSRP, _cordsrp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.CORDSRP);
            }
        }
        private float _grossbdftintl = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBDFTIntl")]
        public virtual float GrossBDFTIntl
        {
            get
            {
                return _grossbdftintl;
            }
            set
            {
                if (Math.Abs(_grossbdftintl - value) < float.Epsilon) { return; }
                _grossbdftintl = value;
                this.ValidateProperty(TREECALCULATEDVALUES.GROSSBDFTINTL, _grossbdftintl);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.GROSSBDFTINTL);
            }
        }
        private float _netbdftintl = 0.0f;
        [XmlElement]
        [Field(Name = "NetBDFTIntl")]
        public virtual float NetBDFTIntl
        {
            get
            {
                return _netbdftintl;
            }
            set
            {
                if (Math.Abs(_netbdftintl - value) < float.Epsilon) { return; }
                _netbdftintl = value;
                this.ValidateProperty(TREECALCULATEDVALUES.NETBDFTINTL, _netbdftintl);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.NETBDFTINTL);
            }
        }
        private float _biomassmainstemprimary = 0.0f;
        [XmlElement]
        [Field(Name = "BiomassMainStemPrimary")]
        public virtual float BiomassMainStemPrimary
        {
            get
            {
                return _biomassmainstemprimary;
            }
            set
            {
                if (Math.Abs(_biomassmainstemprimary - value) < float.Epsilon) { return; }
                _biomassmainstemprimary = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSMAINSTEMPRIMARY, _biomassmainstemprimary);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSMAINSTEMPRIMARY);
            }
        }
        private float _biomassmainstemsecondary = 0.0f;
        [XmlElement]
        [Field(Name = "BiomassMainStemSecondary")]
        public virtual float BiomassMainStemSecondary
        {
            get
            {
                return _biomassmainstemsecondary;
            }
            set
            {
                if (Math.Abs(_biomassmainstemsecondary - value) < float.Epsilon) { return; }
                _biomassmainstemsecondary = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSMAINSTEMSECONDARY, _biomassmainstemsecondary);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSMAINSTEMSECONDARY);
            }
        }
        private float _valuepp = 0.0f;
        [XmlElement]
        [Field(Name = "ValuePP")]
        public virtual float ValuePP
        {
            get
            {
                return _valuepp;
            }
            set
            {
                if (Math.Abs(_valuepp - value) < float.Epsilon) { return; }
                _valuepp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.VALUEPP, _valuepp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.VALUEPP);
            }
        }
        private float _valuesp = 0.0f;
        [XmlElement]
        [Field(Name = "ValueSP")]
        public virtual float ValueSP
        {
            get
            {
                return _valuesp;
            }
            set
            {
                if (Math.Abs(_valuesp - value) < float.Epsilon) { return; }
                _valuesp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.VALUESP, _valuesp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.VALUESP);
            }
        }
        private float _valuerp = 0.0f;
        [XmlElement]
        [Field(Name = "ValueRP")]
        public virtual float ValueRP
        {
            get
            {
                return _valuerp;
            }
            set
            {
                if (Math.Abs(_valuerp - value) < float.Epsilon) { return; }
                _valuerp = value;
                this.ValidateProperty(TREECALCULATEDVALUES.VALUERP, _valuerp);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.VALUERP);
            }
        }
        private float _biomassprod = 0.0f;
        [XmlElement]
        [Field(Name = "BiomassProd")]
        public virtual float BiomassProd
        {
            get
            {
                return _biomassprod;
            }
            set
            {
                if (Math.Abs(_biomassprod - value) < float.Epsilon) { return; }
                _biomassprod = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSPROD, _biomassprod);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSPROD);
            }
        }
        private float _biomasstotalstem = 0.0f;
        [XmlElement]
        [Field(Name = "Biomasstotalstem")]
        public virtual float Biomasstotalstem
        {
            get
            {
                return _biomasstotalstem;
            }
            set
            {
                if (Math.Abs(_biomasstotalstem - value) < float.Epsilon) { return; }
                _biomasstotalstem = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSTOTALSTEM, _biomasstotalstem);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSTOTALSTEM);
            }
        }
        private float _biomasslivebranches = 0.0f;
        [XmlElement]
        [Field(Name = "Biomasslivebranches")]
        public virtual float Biomasslivebranches
        {
            get
            {
                return _biomasslivebranches;
            }
            set
            {
                if (Math.Abs(_biomasslivebranches - value) < float.Epsilon) { return; }
                _biomasslivebranches = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSLIVEBRANCHES, _biomasslivebranches);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSLIVEBRANCHES);
            }
        }
        private float _biomassdeadbranches = 0.0f;
        [XmlElement]
        [Field(Name = "Biomassdeadbranches")]
        public virtual float Biomassdeadbranches
        {
            get
            {
                return _biomassdeadbranches;
            }
            set
            {
                if (Math.Abs(_biomassdeadbranches - value) < float.Epsilon) { return; }
                _biomassdeadbranches = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSDEADBRANCHES, _biomassdeadbranches);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSDEADBRANCHES);
            }
        }
        private float _biomassfoliage = 0.0f;
        [XmlElement]
        [Field(Name = "Biomassfoliage")]
        public virtual float Biomassfoliage
        {
            get
            {
                return _biomassfoliage;
            }
            set
            {
                if (Math.Abs(_biomassfoliage - value) < float.Epsilon) { return; }
                _biomassfoliage = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSFOLIAGE, _biomassfoliage);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSFOLIAGE);
            }
        }
        private float _biomasstip = 0.0f;
        [XmlElement]
        [Field(Name = "BiomassTip")]
        public virtual float BiomassTip
        {
            get
            {
                return _biomasstip;
            }
            set
            {
                if (Math.Abs(_biomasstip - value) < float.Epsilon) { return; }
                _biomasstip = value;
                this.ValidateProperty(TREECALCULATEDVALUES.BIOMASSTIP, _biomasstip);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.BIOMASSTIP);
            }
        }
        private float _tipwoodvolume = 0.0f;
        [XmlElement]
        [Field(Name = "TipwoodVolume")]
        public virtual float TipwoodVolume
        {
            get
            {
                return _tipwoodvolume;
            }
            set
            {
                if (Math.Abs(_tipwoodvolume - value) < float.Epsilon) { return; }
                _tipwoodvolume = value;
                this.ValidateProperty(TREECALCULATEDVALUES.TIPWOODVOLUME, _tipwoodvolume);
                this.NotifyPropertyChanged(TREECALCULATEDVALUES.TIPWOODVOLUME);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TotalCubicVolume", this.TotalCubicVolume) && isValid;
            isValid = ValidateProperty("GrossBDFTPP", this.GrossBDFTPP) && isValid;
            isValid = ValidateProperty("NetBDFTPP", this.NetBDFTPP) && isValid;
            isValid = ValidateProperty("GrossCUFTPP", this.GrossCUFTPP) && isValid;
            isValid = ValidateProperty("NetCUFTPP", this.NetCUFTPP) && isValid;
            isValid = ValidateProperty("CordsPP", this.CordsPP) && isValid;
            isValid = ValidateProperty("GrossBDFTRemvPP", this.GrossBDFTRemvPP) && isValid;
            isValid = ValidateProperty("GrossCUFTRemvPP", this.GrossCUFTRemvPP) && isValid;
            isValid = ValidateProperty("GrossBDFTSP", this.GrossBDFTSP) && isValid;
            isValid = ValidateProperty("NetBDFTSP", this.NetBDFTSP) && isValid;
            isValid = ValidateProperty("GrossCUFTSP", this.GrossCUFTSP) && isValid;
            isValid = ValidateProperty("NetCUFTSP", this.NetCUFTSP) && isValid;
            isValid = ValidateProperty("CordsSP", this.CordsSP) && isValid;
            isValid = ValidateProperty("GrossCUFTRemvSP", this.GrossCUFTRemvSP) && isValid;
            isValid = ValidateProperty("NumberlogsMS", this.NumberlogsMS) && isValid;
            isValid = ValidateProperty("NumberlogsTPW", this.NumberlogsTPW) && isValid;
            isValid = ValidateProperty("GrossBDFTRP", this.GrossBDFTRP) && isValid;
            isValid = ValidateProperty("GrossCUFTRP", this.GrossCUFTRP) && isValid;
            isValid = ValidateProperty("CordsRP", this.CordsRP) && isValid;
            isValid = ValidateProperty("GrossBDFTIntl", this.GrossBDFTIntl) && isValid;
            isValid = ValidateProperty("NetBDFTIntl", this.NetBDFTIntl) && isValid;
            isValid = ValidateProperty("BiomassMainStemPrimary", this.BiomassMainStemPrimary) && isValid;
            isValid = ValidateProperty("BiomassMainStemSecondary", this.BiomassMainStemSecondary) && isValid;
            isValid = ValidateProperty("ValuePP", this.ValuePP) && isValid;
            isValid = ValidateProperty("ValueSP", this.ValueSP) && isValid;
            isValid = ValidateProperty("ValueRP", this.ValueRP) && isValid;
            isValid = ValidateProperty("BiomassProd", this.BiomassProd) && isValid;
            isValid = ValidateProperty("Biomasstotalstem", this.Biomasstotalstem) && isValid;
            isValid = ValidateProperty("Biomasslivebranches", this.Biomasslivebranches) && isValid;
            isValid = ValidateProperty("Biomassdeadbranches", this.Biomassdeadbranches) && isValid;
            isValid = ValidateProperty("Biomassfoliage", this.Biomassfoliage) && isValid;
            isValid = ValidateProperty("BiomassTip", this.BiomassTip) && isValid;
            isValid = ValidateProperty("TipwoodVolume", this.TipwoodVolume) && isValid;
            isValid = ValidateProperty("Tree_CN", this.Tree_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeCalculatedValuesDO);
        }

        public void SetValues(TreeCalculatedValuesDO obj)
        {
            if (obj == null) { return; }
            TotalCubicVolume = obj.TotalCubicVolume;
            GrossBDFTPP = obj.GrossBDFTPP;
            NetBDFTPP = obj.NetBDFTPP;
            GrossCUFTPP = obj.GrossCUFTPP;
            NetCUFTPP = obj.NetCUFTPP;
            CordsPP = obj.CordsPP;
            GrossBDFTRemvPP = obj.GrossBDFTRemvPP;
            GrossCUFTRemvPP = obj.GrossCUFTRemvPP;
            GrossBDFTSP = obj.GrossBDFTSP;
            NetBDFTSP = obj.NetBDFTSP;
            GrossCUFTSP = obj.GrossCUFTSP;
            NetCUFTSP = obj.NetCUFTSP;
            CordsSP = obj.CordsSP;
            GrossCUFTRemvSP = obj.GrossCUFTRemvSP;
            NumberlogsMS = obj.NumberlogsMS;
            NumberlogsTPW = obj.NumberlogsTPW;
            GrossBDFTRP = obj.GrossBDFTRP;
            GrossCUFTRP = obj.GrossCUFTRP;
            CordsRP = obj.CordsRP;
            GrossBDFTIntl = obj.GrossBDFTIntl;
            NetBDFTIntl = obj.NetBDFTIntl;
            BiomassMainStemPrimary = obj.BiomassMainStemPrimary;
            BiomassMainStemSecondary = obj.BiomassMainStemSecondary;
            ValuePP = obj.ValuePP;
            ValueSP = obj.ValueSP;
            ValueRP = obj.ValueRP;
            BiomassProd = obj.BiomassProd;
            Biomasstotalstem = obj.Biomasstotalstem;
            Biomasslivebranches = obj.Biomasslivebranches;
            Biomassdeadbranches = obj.Biomassdeadbranches;
            Biomassfoliage = obj.Biomassfoliage;
            BiomassTip = obj.BiomassTip;
            TipwoodVolume = obj.TipwoodVolume;
        }
    }
    [Table("LCD")]
    public partial class LCDDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LCDDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("CutLeave", "LCD", "CutLeave is Required"));
            _validator.Add(new NotNullRule("Stratum", "LCD", "Stratum is Required"));
            _validator.Add(new NotNullRule("SampleGroup", "LCD", "SampleGroup is Required"));
            _validator.Add(new NotNullRule("Species", "LCD", "Species is Required"));
            _validator.Add(new NotNullRule("PrimaryProduct", "LCD", "PrimaryProduct is Required"));
            _validator.Add(new NotNullRule("SecondaryProduct", "LCD", "SecondaryProduct is Required"));
            _validator.Add(new NotNullRule("UOM", "LCD", "UOM is Required"));
            _validator.Add(new NotNullRule("LiveDead", "LCD", "LiveDead is Required"));
            _validator.Add(new NotNullRule("Yield", "LCD", "Yield is Required"));
            _validator.Add(new NotNullRule("ContractSpecies", "LCD", "ContractSpecies is Required"));
            _validator.Add(new NotNullRule("TreeGrade", "LCD", "TreeGrade is Required"));
        }

        public LCDDO() { }

        public LCDDO(LCDDO obj) : this()
        {
            SetValues(obj);
        }

        public LCDDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "LCD_CN")]
        public Int64? LCD_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _cutleave;
        [XmlElement]
        [Field(Name = "CutLeave")]
        public virtual String CutLeave
        {
            get
            {
                return _cutleave;
            }
            set
            {
                if (_cutleave == value) { return; }
                _cutleave = value;
                this.ValidateProperty(LCD.CUTLEAVE, _cutleave);
                this.NotifyPropertyChanged(LCD.CUTLEAVE);
            }
        }
        private String _stratum;
        [XmlElement]
        [Field(Name = "Stratum")]
        public virtual String Stratum
        {
            get
            {
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                this.ValidateProperty(LCD.STRATUM, _stratum);
                this.NotifyPropertyChanged(LCD.STRATUM);
            }
        }
        private String _samplegroup;
        [XmlElement]
        [Field(Name = "SampleGroup")]
        public virtual String SampleGroup
        {
            get
            {
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                this.ValidateProperty(LCD.SAMPLEGROUP, _samplegroup);
                this.NotifyPropertyChanged(LCD.SAMPLEGROUP);
            }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(LCD.SPECIES, _species);
                this.NotifyPropertyChanged(LCD.SPECIES);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(LCD.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(LCD.PRIMARYPRODUCT);
            }
        }
        private String _secondaryproduct;
        [XmlElement]
        [Field(Name = "SecondaryProduct")]
        public virtual String SecondaryProduct
        {
            get
            {
                return _secondaryproduct;
            }
            set
            {
                if (_secondaryproduct == value) { return; }
                _secondaryproduct = value;
                this.ValidateProperty(LCD.SECONDARYPRODUCT, _secondaryproduct);
                this.NotifyPropertyChanged(LCD.SECONDARYPRODUCT);
            }
        }
        private String _uom;
        [XmlElement]
        [Field(Name = "UOM")]
        public virtual String UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                if (_uom == value) { return; }
                _uom = value;
                this.ValidateProperty(LCD.UOM, _uom);
                this.NotifyPropertyChanged(LCD.UOM);
            }
        }
        private String _livedead;
        [XmlElement]
        [Field(Name = "LiveDead")]
        public virtual String LiveDead
        {
            get
            {
                return _livedead;
            }
            set
            {
                if (_livedead == value) { return; }
                _livedead = value;
                this.ValidateProperty(LCD.LIVEDEAD, _livedead);
                this.NotifyPropertyChanged(LCD.LIVEDEAD);
            }
        }
        private String _yield;
        [XmlElement]
        [Field(Name = "Yield")]
        public virtual String Yield
        {
            get
            {
                return _yield;
            }
            set
            {
                if (_yield == value) { return; }
                _yield = value;
                this.ValidateProperty(LCD.YIELD, _yield);
                this.NotifyPropertyChanged(LCD.YIELD);
            }
        }
        private String _contractspecies;
        [XmlElement]
        [Field(Name = "ContractSpecies")]
        public virtual String ContractSpecies
        {
            get
            {
                return _contractspecies;
            }
            set
            {
                if (_contractspecies == value) { return; }
                _contractspecies = value;
                this.ValidateProperty(LCD.CONTRACTSPECIES, _contractspecies);
                this.NotifyPropertyChanged(LCD.CONTRACTSPECIES);
            }
        }
        private String _treegrade;
        [XmlElement]
        [Field(Name = "TreeGrade")]
        public virtual String TreeGrade
        {
            get
            {
                return _treegrade;
            }
            set
            {
                if (_treegrade == value) { return; }
                _treegrade = value;
                this.ValidateProperty(LCD.TREEGRADE, _treegrade);
                this.NotifyPropertyChanged(LCD.TREEGRADE);
            }
        }
        private String _stm;
        [XmlElement]
        [Field(Name = "STM")]
        public virtual String STM
        {
            get
            {
                return _stm;
            }
            set
            {
                if (_stm == value) { return; }
                _stm = value;
                this.ValidateProperty(LCD.STM, _stm);
                this.NotifyPropertyChanged(LCD.STM);
            }
        }
        private Double _firststagetrees = 0.0;
        [XmlElement]
        [Field(Name = "FirstStageTrees")]
        public virtual Double FirstStageTrees
        {
            get
            {
                return _firststagetrees;
            }
            set
            {
                if (Math.Abs(_firststagetrees - value) < Double.Epsilon) { return; }
                _firststagetrees = value;
                this.ValidateProperty(LCD.FIRSTSTAGETREES, _firststagetrees);
                this.NotifyPropertyChanged(LCD.FIRSTSTAGETREES);
            }
        }
        private Double _measuredtrees = 0.0;
        [XmlElement]
        [Field(Name = "MeasuredTrees")]
        public virtual Double MeasuredTrees
        {
            get
            {
                return _measuredtrees;
            }
            set
            {
                if (Math.Abs(_measuredtrees - value) < Double.Epsilon) { return; }
                _measuredtrees = value;
                this.ValidateProperty(LCD.MEASUREDTREES, _measuredtrees);
                this.NotifyPropertyChanged(LCD.MEASUREDTREES);
            }
        }
        private Double _talliedtrees = 0.0;
        [XmlElement]
        [Field(Name = "TalliedTrees")]
        public virtual Double TalliedTrees
        {
            get
            {
                return _talliedtrees;
            }
            set
            {
                if (Math.Abs(_talliedtrees - value) < Double.Epsilon) { return; }
                _talliedtrees = value;
                this.ValidateProperty(LCD.TALLIEDTREES, _talliedtrees);
                this.NotifyPropertyChanged(LCD.TALLIEDTREES);
            }
        }
        private Double _sumkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumKPI")]
        public virtual Double SumKPI
        {
            get
            {
                return _sumkpi;
            }
            set
            {
                if (Math.Abs(_sumkpi - value) < Double.Epsilon) { return; }
                _sumkpi = value;
                this.ValidateProperty(LCD.SUMKPI, _sumkpi);
                this.NotifyPropertyChanged(LCD.SUMKPI);
            }
        }
        private Double _summeasuredkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumMeasuredKPI")]
        public virtual Double SumMeasuredKPI
        {
            get
            {
                return _summeasuredkpi;
            }
            set
            {
                if (Math.Abs(_summeasuredkpi - value) < Double.Epsilon) { return; }
                _summeasuredkpi = value;
                this.ValidateProperty(LCD.SUMMEASUREDKPI, _summeasuredkpi);
                this.NotifyPropertyChanged(LCD.SUMMEASUREDKPI);
            }
        }
        private Double _sumexpanfactor = 0.0;
        [XmlElement]
        [Field(Name = "SumExpanFactor")]
        public virtual Double SumExpanFactor
        {
            get
            {
                return _sumexpanfactor;
            }
            set
            {
                if (Math.Abs(_sumexpanfactor - value) < Double.Epsilon) { return; }
                _sumexpanfactor = value;
                this.ValidateProperty(LCD.SUMEXPANFACTOR, _sumexpanfactor);
                this.NotifyPropertyChanged(LCD.SUMEXPANFACTOR);
            }
        }
        private Double _sumdbhob = 0.0;
        [XmlElement]
        [Field(Name = "SumDBHOB")]
        public virtual Double SumDBHOB
        {
            get
            {
                return _sumdbhob;
            }
            set
            {
                if (Math.Abs(_sumdbhob - value) < Double.Epsilon) { return; }
                _sumdbhob = value;
                this.ValidateProperty(LCD.SUMDBHOB, _sumdbhob);
                this.NotifyPropertyChanged(LCD.SUMDBHOB);
            }
        }
        private Double _sumdbhobsqrd = 0.0;
        [XmlElement]
        [Field(Name = "SumDBHOBsqrd")]
        public virtual Double SumDBHOBsqrd
        {
            get
            {
                return _sumdbhobsqrd;
            }
            set
            {
                if (Math.Abs(_sumdbhobsqrd - value) < Double.Epsilon) { return; }
                _sumdbhobsqrd = value;
                this.ValidateProperty(LCD.SUMDBHOBSQRD, _sumdbhobsqrd);
                this.NotifyPropertyChanged(LCD.SUMDBHOBSQRD);
            }
        }
        private Double _sumtothgt = 0.0;
        [XmlElement]
        [Field(Name = "SumTotHgt")]
        public virtual Double SumTotHgt
        {
            get
            {
                return _sumtothgt;
            }
            set
            {
                if (Math.Abs(_sumtothgt - value) < Double.Epsilon) { return; }
                _sumtothgt = value;
                this.ValidateProperty(LCD.SUMTOTHGT, _sumtothgt);
                this.NotifyPropertyChanged(LCD.SUMTOTHGT);
            }
        }
        private Double _sumhgtupstem = 0.0;
        [XmlElement]
        [Field(Name = "SumHgtUpStem")]
        public virtual Double SumHgtUpStem
        {
            get
            {
                return _sumhgtupstem;
            }
            set
            {
                if (Math.Abs(_sumhgtupstem - value) < Double.Epsilon) { return; }
                _sumhgtupstem = value;
                this.ValidateProperty(LCD.SUMHGTUPSTEM, _sumhgtupstem);
                this.NotifyPropertyChanged(LCD.SUMHGTUPSTEM);
            }
        }
        private Double _summerchhgtprim = 0.0;
        [XmlElement]
        [Field(Name = "SumMerchHgtPrim")]
        public virtual Double SumMerchHgtPrim
        {
            get
            {
                return _summerchhgtprim;
            }
            set
            {
                if (Math.Abs(_summerchhgtprim - value) < Double.Epsilon) { return; }
                _summerchhgtprim = value;
                this.ValidateProperty(LCD.SUMMERCHHGTPRIM, _summerchhgtprim);
                this.NotifyPropertyChanged(LCD.SUMMERCHHGTPRIM);
            }
        }
        private Double _summerchhgtsecond = 0.0;
        [XmlElement]
        [Field(Name = "SumMerchHgtSecond")]
        public virtual Double SumMerchHgtSecond
        {
            get
            {
                return _summerchhgtsecond;
            }
            set
            {
                if (Math.Abs(_summerchhgtsecond - value) < Double.Epsilon) { return; }
                _summerchhgtsecond = value;
                this.ValidateProperty(LCD.SUMMERCHHGTSECOND, _summerchhgtsecond);
                this.NotifyPropertyChanged(LCD.SUMMERCHHGTSECOND);
            }
        }
        private Double _sumlogsms = 0.0;
        [XmlElement]
        [Field(Name = "SumLogsMS")]
        public virtual Double SumLogsMS
        {
            get
            {
                return _sumlogsms;
            }
            set
            {
                if (Math.Abs(_sumlogsms - value) < Double.Epsilon) { return; }
                _sumlogsms = value;
                this.ValidateProperty(LCD.SUMLOGSMS, _sumlogsms);
                this.NotifyPropertyChanged(LCD.SUMLOGSMS);
            }
        }
        private Double _sumtotcubic = 0.0;
        [XmlElement]
        [Field(Name = "SumTotCubic")]
        public virtual Double SumTotCubic
        {
            get
            {
                return _sumtotcubic;
            }
            set
            {
                if (Math.Abs(_sumtotcubic - value) < Double.Epsilon) { return; }
                _sumtotcubic = value;
                this.ValidateProperty(LCD.SUMTOTCUBIC, _sumtotcubic);
                this.NotifyPropertyChanged(LCD.SUMTOTCUBIC);
            }
        }
        private Double _sumgbdft = 0.0;
        [XmlElement]
        [Field(Name = "SumGBDFT")]
        public virtual Double SumGBDFT
        {
            get
            {
                return _sumgbdft;
            }
            set
            {
                if (Math.Abs(_sumgbdft - value) < Double.Epsilon) { return; }
                _sumgbdft = value;
                this.ValidateProperty(LCD.SUMGBDFT, _sumgbdft);
                this.NotifyPropertyChanged(LCD.SUMGBDFT);
            }
        }
        private Double _sumnbdft = 0.0;
        [XmlElement]
        [Field(Name = "SumNBDFT")]
        public virtual Double SumNBDFT
        {
            get
            {
                return _sumnbdft;
            }
            set
            {
                if (Math.Abs(_sumnbdft - value) < Double.Epsilon) { return; }
                _sumnbdft = value;
                this.ValidateProperty(LCD.SUMNBDFT, _sumnbdft);
                this.NotifyPropertyChanged(LCD.SUMNBDFT);
            }
        }
        private Double _sumgcuft = 0.0;
        [XmlElement]
        [Field(Name = "SumGCUFT")]
        public virtual Double SumGCUFT
        {
            get
            {
                return _sumgcuft;
            }
            set
            {
                if (Math.Abs(_sumgcuft - value) < Double.Epsilon) { return; }
                _sumgcuft = value;
                this.ValidateProperty(LCD.SUMGCUFT, _sumgcuft);
                this.NotifyPropertyChanged(LCD.SUMGCUFT);
            }
        }
        private Double _sumncuft = 0.0;
        [XmlElement]
        [Field(Name = "SumNCUFT")]
        public virtual Double SumNCUFT
        {
            get
            {
                return _sumncuft;
            }
            set
            {
                if (Math.Abs(_sumncuft - value) < Double.Epsilon) { return; }
                _sumncuft = value;
                this.ValidateProperty(LCD.SUMNCUFT, _sumncuft);
                this.NotifyPropertyChanged(LCD.SUMNCUFT);
            }
        }
        private Double _sumgbdftremv = 0.0;
        [XmlElement]
        [Field(Name = "SumGBDFTremv")]
        public virtual Double SumGBDFTremv
        {
            get
            {
                return _sumgbdftremv;
            }
            set
            {
                if (Math.Abs(_sumgbdftremv - value) < Double.Epsilon) { return; }
                _sumgbdftremv = value;
                this.ValidateProperty(LCD.SUMGBDFTREMV, _sumgbdftremv);
                this.NotifyPropertyChanged(LCD.SUMGBDFTREMV);
            }
        }
        private Double _sumgcuftremv = 0.0;
        [XmlElement]
        [Field(Name = "SumGCUFTremv")]
        public virtual Double SumGCUFTremv
        {
            get
            {
                return _sumgcuftremv;
            }
            set
            {
                if (Math.Abs(_sumgcuftremv - value) < Double.Epsilon) { return; }
                _sumgcuftremv = value;
                this.ValidateProperty(LCD.SUMGCUFTREMV, _sumgcuftremv);
                this.NotifyPropertyChanged(LCD.SUMGCUFTREMV);
            }
        }
        private Double _sumcords = 0.0;
        [XmlElement]
        [Field(Name = "SumCords")]
        public virtual Double SumCords
        {
            get
            {
                return _sumcords;
            }
            set
            {
                if (Math.Abs(_sumcords - value) < Double.Epsilon) { return; }
                _sumcords = value;
                this.ValidateProperty(LCD.SUMCORDS, _sumcords);
                this.NotifyPropertyChanged(LCD.SUMCORDS);
            }
        }
        private Double _sumwgtmsp = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtMSP")]
        public virtual Double SumWgtMSP
        {
            get
            {
                return _sumwgtmsp;
            }
            set
            {
                if (Math.Abs(_sumwgtmsp - value) < Double.Epsilon) { return; }
                _sumwgtmsp = value;
                this.ValidateProperty(LCD.SUMWGTMSP, _sumwgtmsp);
                this.NotifyPropertyChanged(LCD.SUMWGTMSP);
            }
        }
        private Double _sumvalue = 0.0;
        [XmlElement]
        [Field(Name = "SumValue")]
        public virtual Double SumValue
        {
            get
            {
                return _sumvalue;
            }
            set
            {
                if (Math.Abs(_sumvalue - value) < Double.Epsilon) { return; }
                _sumvalue = value;
                this.ValidateProperty(LCD.SUMVALUE, _sumvalue);
                this.NotifyPropertyChanged(LCD.SUMVALUE);
            }
        }
        private Double _sumgbdfttop = 0.0;
        [XmlElement]
        [Field(Name = "SumGBDFTtop")]
        public virtual Double SumGBDFTtop
        {
            get
            {
                return _sumgbdfttop;
            }
            set
            {
                if (Math.Abs(_sumgbdfttop - value) < Double.Epsilon) { return; }
                _sumgbdfttop = value;
                this.ValidateProperty(LCD.SUMGBDFTTOP, _sumgbdfttop);
                this.NotifyPropertyChanged(LCD.SUMGBDFTTOP);
            }
        }
        private Double _sumnbdfttop = 0.0;
        [XmlElement]
        [Field(Name = "SumNBDFTtop")]
        public virtual Double SumNBDFTtop
        {
            get
            {
                return _sumnbdfttop;
            }
            set
            {
                if (Math.Abs(_sumnbdfttop - value) < Double.Epsilon) { return; }
                _sumnbdfttop = value;
                this.ValidateProperty(LCD.SUMNBDFTTOP, _sumnbdfttop);
                this.NotifyPropertyChanged(LCD.SUMNBDFTTOP);
            }
        }
        private Double _sumgcufttop = 0.0;
        [XmlElement]
        [Field(Name = "SumGCUFTtop")]
        public virtual Double SumGCUFTtop
        {
            get
            {
                return _sumgcufttop;
            }
            set
            {
                if (Math.Abs(_sumgcufttop - value) < Double.Epsilon) { return; }
                _sumgcufttop = value;
                this.ValidateProperty(LCD.SUMGCUFTTOP, _sumgcufttop);
                this.NotifyPropertyChanged(LCD.SUMGCUFTTOP);
            }
        }
        private Double _sumncufttop = 0.0;
        [XmlElement]
        [Field(Name = "SumNCUFTtop")]
        public virtual Double SumNCUFTtop
        {
            get
            {
                return _sumncufttop;
            }
            set
            {
                if (Math.Abs(_sumncufttop - value) < Double.Epsilon) { return; }
                _sumncufttop = value;
                this.ValidateProperty(LCD.SUMNCUFTTOP, _sumncufttop);
                this.NotifyPropertyChanged(LCD.SUMNCUFTTOP);
            }
        }
        private Double _sumcordstop = 0.0;
        [XmlElement]
        [Field(Name = "SumCordsTop")]
        public virtual Double SumCordsTop
        {
            get
            {
                return _sumcordstop;
            }
            set
            {
                if (Math.Abs(_sumcordstop - value) < Double.Epsilon) { return; }
                _sumcordstop = value;
                this.ValidateProperty(LCD.SUMCORDSTOP, _sumcordstop);
                this.NotifyPropertyChanged(LCD.SUMCORDSTOP);
            }
        }
        private Double _sumwgtmss = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtMSS")]
        public virtual Double SumWgtMSS
        {
            get
            {
                return _sumwgtmss;
            }
            set
            {
                if (Math.Abs(_sumwgtmss - value) < Double.Epsilon) { return; }
                _sumwgtmss = value;
                this.ValidateProperty(LCD.SUMWGTMSS, _sumwgtmss);
                this.NotifyPropertyChanged(LCD.SUMWGTMSS);
            }
        }
        private Double _sumtopvalue = 0.0;
        [XmlElement]
        [Field(Name = "SumTopValue")]
        public virtual Double SumTopValue
        {
            get
            {
                return _sumtopvalue;
            }
            set
            {
                if (Math.Abs(_sumtopvalue - value) < Double.Epsilon) { return; }
                _sumtopvalue = value;
                this.ValidateProperty(LCD.SUMTOPVALUE, _sumtopvalue);
                this.NotifyPropertyChanged(LCD.SUMTOPVALUE);
            }
        }
        private Double _sumlogstop = 0.0;
        [XmlElement]
        [Field(Name = "SumLogsTop")]
        public virtual Double SumLogsTop
        {
            get
            {
                return _sumlogstop;
            }
            set
            {
                if (Math.Abs(_sumlogstop - value) < Double.Epsilon) { return; }
                _sumlogstop = value;
                this.ValidateProperty(LCD.SUMLOGSTOP, _sumlogstop);
                this.NotifyPropertyChanged(LCD.SUMLOGSTOP);
            }
        }
        private Double _sumbdftrecv = 0.0;
        [XmlElement]
        [Field(Name = "SumBDFTrecv")]
        public virtual Double SumBDFTrecv
        {
            get
            {
                return _sumbdftrecv;
            }
            set
            {
                if (Math.Abs(_sumbdftrecv - value) < Double.Epsilon) { return; }
                _sumbdftrecv = value;
                this.ValidateProperty(LCD.SUMBDFTRECV, _sumbdftrecv);
                this.NotifyPropertyChanged(LCD.SUMBDFTRECV);
            }
        }
        private Double _sumcuftrecv = 0.0;
        [XmlElement]
        [Field(Name = "SumCUFTrecv")]
        public virtual Double SumCUFTrecv
        {
            get
            {
                return _sumcuftrecv;
            }
            set
            {
                if (Math.Abs(_sumcuftrecv - value) < Double.Epsilon) { return; }
                _sumcuftrecv = value;
                this.ValidateProperty(LCD.SUMCUFTRECV, _sumcuftrecv);
                this.NotifyPropertyChanged(LCD.SUMCUFTRECV);
            }
        }
        private Double _sumcordsrecv = 0.0;
        [XmlElement]
        [Field(Name = "SumCordsRecv")]
        public virtual Double SumCordsRecv
        {
            get
            {
                return _sumcordsrecv;
            }
            set
            {
                if (Math.Abs(_sumcordsrecv - value) < Double.Epsilon) { return; }
                _sumcordsrecv = value;
                this.ValidateProperty(LCD.SUMCORDSRECV, _sumcordsrecv);
                this.NotifyPropertyChanged(LCD.SUMCORDSRECV);
            }
        }
        private Double _sumvaluerecv = 0.0;
        [XmlElement]
        [Field(Name = "SumValueRecv")]
        public virtual Double SumValueRecv
        {
            get
            {
                return _sumvaluerecv;
            }
            set
            {
                if (Math.Abs(_sumvaluerecv - value) < Double.Epsilon) { return; }
                _sumvaluerecv = value;
                this.ValidateProperty(LCD.SUMVALUERECV, _sumvaluerecv);
                this.NotifyPropertyChanged(LCD.SUMVALUERECV);
            }
        }
        private Double _biomassproduct = 0.0;
        [XmlElement]
        [Field(Name = "BiomassProduct")]
        public virtual Double BiomassProduct
        {
            get
            {
                return _biomassproduct;
            }
            set
            {
                if (Math.Abs(_biomassproduct - value) < Double.Epsilon) { return; }
                _biomassproduct = value;
                this.ValidateProperty(LCD.BIOMASSPRODUCT, _biomassproduct);
                this.NotifyPropertyChanged(LCD.BIOMASSPRODUCT);
            }
        }
        private Double _sumwgtbat = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtBAT")]
        public virtual Double SumWgtBAT
        {
            get
            {
                return _sumwgtbat;
            }
            set
            {
                if (Math.Abs(_sumwgtbat - value) < Double.Epsilon) { return; }
                _sumwgtbat = value;
                this.ValidateProperty(LCD.SUMWGTBAT, _sumwgtbat);
                this.NotifyPropertyChanged(LCD.SUMWGTBAT);
            }
        }
        private Double _sumwgtbbl = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtBBL")]
        public virtual Double SumWgtBBL
        {
            get
            {
                return _sumwgtbbl;
            }
            set
            {
                if (Math.Abs(_sumwgtbbl - value) < Double.Epsilon) { return; }
                _sumwgtbbl = value;
                this.ValidateProperty(LCD.SUMWGTBBL, _sumwgtbbl);
                this.NotifyPropertyChanged(LCD.SUMWGTBBL);
            }
        }
        private Double _sumwgtbbd = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtBBD")]
        public virtual Double SumWgtBBD
        {
            get
            {
                return _sumwgtbbd;
            }
            set
            {
                if (Math.Abs(_sumwgtbbd - value) < Double.Epsilon) { return; }
                _sumwgtbbd = value;
                this.ValidateProperty(LCD.SUMWGTBBD, _sumwgtbbd);
                this.NotifyPropertyChanged(LCD.SUMWGTBBD);
            }
        }
        private Double _sumwgtbft = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtBFT")]
        public virtual Double SumWgtBFT
        {
            get
            {
                return _sumwgtbft;
            }
            set
            {
                if (Math.Abs(_sumwgtbft - value) < Double.Epsilon) { return; }
                _sumwgtbft = value;
                this.ValidateProperty(LCD.SUMWGTBFT, _sumwgtbft);
                this.NotifyPropertyChanged(LCD.SUMWGTBFT);
            }
        }
        private Double _sumwgttip = 0.0;
        [XmlElement]
        [Field(Name = "SumWgtTip")]
        public virtual Double SumWgtTip
        {
            get
            {
                return _sumwgttip;
            }
            set
            {
                if (Math.Abs(_sumwgttip - value) < Double.Epsilon) { return; }
                _sumwgttip = value;
                this.ValidateProperty(LCD.SUMWGTTIP, _sumwgttip);
                this.NotifyPropertyChanged(LCD.SUMWGTTIP);
            }
        }
        private Double _sumtipwood = 0.0;
        [XmlElement]
        [Field(Name = "SumTipwood")]
        public virtual Double SumTipwood
        {
            get
            {
                return _sumtipwood;
            }
            set
            {
                if (Math.Abs(_sumtipwood - value) < Double.Epsilon) { return; }
                _sumtipwood = value;
                this.ValidateProperty(LCD.SUMTIPWOOD, _sumtipwood);
                this.NotifyPropertyChanged(LCD.SUMTIPWOOD);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("CutLeave", this.CutLeave) && isValid;
            isValid = ValidateProperty("Stratum", this.Stratum) && isValid;
            isValid = ValidateProperty("SampleGroup", this.SampleGroup) && isValid;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("SecondaryProduct", this.SecondaryProduct) && isValid;
            isValid = ValidateProperty("UOM", this.UOM) && isValid;
            isValid = ValidateProperty("LiveDead", this.LiveDead) && isValid;
            isValid = ValidateProperty("Yield", this.Yield) && isValid;
            isValid = ValidateProperty("ContractSpecies", this.ContractSpecies) && isValid;
            isValid = ValidateProperty("TreeGrade", this.TreeGrade) && isValid;
            isValid = ValidateProperty("STM", this.STM) && isValid;
            isValid = ValidateProperty("FirstStageTrees", this.FirstStageTrees) && isValid;
            isValid = ValidateProperty("MeasuredTrees", this.MeasuredTrees) && isValid;
            isValid = ValidateProperty("TalliedTrees", this.TalliedTrees) && isValid;
            isValid = ValidateProperty("SumKPI", this.SumKPI) && isValid;
            isValid = ValidateProperty("SumMeasuredKPI", this.SumMeasuredKPI) && isValid;
            isValid = ValidateProperty("SumExpanFactor", this.SumExpanFactor) && isValid;
            isValid = ValidateProperty("SumDBHOB", this.SumDBHOB) && isValid;
            isValid = ValidateProperty("SumDBHOBsqrd", this.SumDBHOBsqrd) && isValid;
            isValid = ValidateProperty("SumTotHgt", this.SumTotHgt) && isValid;
            isValid = ValidateProperty("SumHgtUpStem", this.SumHgtUpStem) && isValid;
            isValid = ValidateProperty("SumMerchHgtPrim", this.SumMerchHgtPrim) && isValid;
            isValid = ValidateProperty("SumMerchHgtSecond", this.SumMerchHgtSecond) && isValid;
            isValid = ValidateProperty("SumLogsMS", this.SumLogsMS) && isValid;
            isValid = ValidateProperty("SumTotCubic", this.SumTotCubic) && isValid;
            isValid = ValidateProperty("SumGBDFT", this.SumGBDFT) && isValid;
            isValid = ValidateProperty("SumNBDFT", this.SumNBDFT) && isValid;
            isValid = ValidateProperty("SumGCUFT", this.SumGCUFT) && isValid;
            isValid = ValidateProperty("SumNCUFT", this.SumNCUFT) && isValid;
            isValid = ValidateProperty("SumGBDFTremv", this.SumGBDFTremv) && isValid;
            isValid = ValidateProperty("SumGCUFTremv", this.SumGCUFTremv) && isValid;
            isValid = ValidateProperty("SumCords", this.SumCords) && isValid;
            isValid = ValidateProperty("SumWgtMSP", this.SumWgtMSP) && isValid;
            isValid = ValidateProperty("SumValue", this.SumValue) && isValid;
            isValid = ValidateProperty("SumGBDFTtop", this.SumGBDFTtop) && isValid;
            isValid = ValidateProperty("SumNBDFTtop", this.SumNBDFTtop) && isValid;
            isValid = ValidateProperty("SumGCUFTtop", this.SumGCUFTtop) && isValid;
            isValid = ValidateProperty("SumNCUFTtop", this.SumNCUFTtop) && isValid;
            isValid = ValidateProperty("SumCordsTop", this.SumCordsTop) && isValid;
            isValid = ValidateProperty("SumWgtMSS", this.SumWgtMSS) && isValid;
            isValid = ValidateProperty("SumTopValue", this.SumTopValue) && isValid;
            isValid = ValidateProperty("SumLogsTop", this.SumLogsTop) && isValid;
            isValid = ValidateProperty("SumBDFTrecv", this.SumBDFTrecv) && isValid;
            isValid = ValidateProperty("SumCUFTrecv", this.SumCUFTrecv) && isValid;
            isValid = ValidateProperty("SumCordsRecv", this.SumCordsRecv) && isValid;
            isValid = ValidateProperty("SumValueRecv", this.SumValueRecv) && isValid;
            isValid = ValidateProperty("BiomassProduct", this.BiomassProduct) && isValid;
            isValid = ValidateProperty("SumWgtBAT", this.SumWgtBAT) && isValid;
            isValid = ValidateProperty("SumWgtBBL", this.SumWgtBBL) && isValid;
            isValid = ValidateProperty("SumWgtBBD", this.SumWgtBBD) && isValid;
            isValid = ValidateProperty("SumWgtBFT", this.SumWgtBFT) && isValid;
            isValid = ValidateProperty("SumWgtTip", this.SumWgtTip) && isValid;
            isValid = ValidateProperty("SumTipwood", this.SumTipwood) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LCDDO);
        }

        public void SetValues(LCDDO obj)
        {
            if (obj == null) { return; }
            CutLeave = obj.CutLeave;
            Stratum = obj.Stratum;
            SampleGroup = obj.SampleGroup;
            Species = obj.Species;
            PrimaryProduct = obj.PrimaryProduct;
            SecondaryProduct = obj.SecondaryProduct;
            UOM = obj.UOM;
            LiveDead = obj.LiveDead;
            Yield = obj.Yield;
            ContractSpecies = obj.ContractSpecies;
            TreeGrade = obj.TreeGrade;
            STM = obj.STM;
            FirstStageTrees = obj.FirstStageTrees;
            MeasuredTrees = obj.MeasuredTrees;
            TalliedTrees = obj.TalliedTrees;
            SumKPI = obj.SumKPI;
            SumMeasuredKPI = obj.SumMeasuredKPI;
            SumExpanFactor = obj.SumExpanFactor;
            SumDBHOB = obj.SumDBHOB;
            SumDBHOBsqrd = obj.SumDBHOBsqrd;
            SumTotHgt = obj.SumTotHgt;
            SumHgtUpStem = obj.SumHgtUpStem;
            SumMerchHgtPrim = obj.SumMerchHgtPrim;
            SumMerchHgtSecond = obj.SumMerchHgtSecond;
            SumLogsMS = obj.SumLogsMS;
            SumTotCubic = obj.SumTotCubic;
            SumGBDFT = obj.SumGBDFT;
            SumNBDFT = obj.SumNBDFT;
            SumGCUFT = obj.SumGCUFT;
            SumNCUFT = obj.SumNCUFT;
            SumGBDFTremv = obj.SumGBDFTremv;
            SumGCUFTremv = obj.SumGCUFTremv;
            SumCords = obj.SumCords;
            SumWgtMSP = obj.SumWgtMSP;
            SumValue = obj.SumValue;
            SumGBDFTtop = obj.SumGBDFTtop;
            SumNBDFTtop = obj.SumNBDFTtop;
            SumGCUFTtop = obj.SumGCUFTtop;
            SumNCUFTtop = obj.SumNCUFTtop;
            SumCordsTop = obj.SumCordsTop;
            SumWgtMSS = obj.SumWgtMSS;
            SumTopValue = obj.SumTopValue;
            SumLogsTop = obj.SumLogsTop;
            SumBDFTrecv = obj.SumBDFTrecv;
            SumCUFTrecv = obj.SumCUFTrecv;
            SumCordsRecv = obj.SumCordsRecv;
            SumValueRecv = obj.SumValueRecv;
            BiomassProduct = obj.BiomassProduct;
            SumWgtBAT = obj.SumWgtBAT;
            SumWgtBBL = obj.SumWgtBBL;
            SumWgtBBD = obj.SumWgtBBD;
            SumWgtBFT = obj.SumWgtBFT;
            SumWgtTip = obj.SumWgtTip;
            SumTipwood = obj.SumTipwood;
        }
    }
    [Table("POP")]
    public partial class POPDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static POPDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("CutLeave", "POP", "CutLeave is Required"));
            _validator.Add(new NotNullRule("Stratum", "POP", "Stratum is Required"));
            _validator.Add(new NotNullRule("SampleGroup", "POP", "SampleGroup is Required"));
            _validator.Add(new NotNullRule("PrimaryProduct", "POP", "PrimaryProduct is Required"));
            _validator.Add(new NotNullRule("SecondaryProduct", "POP", "SecondaryProduct is Required"));
            _validator.Add(new NotNullRule("UOM", "POP", "UOM is Required"));
        }

        public POPDO() { }

        public POPDO(POPDO obj) : this()
        {
            SetValues(obj);
        }

        public POPDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "POP_CN")]
        public Int64? POP_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _cutleave;
        [XmlElement]
        [Field(Name = "CutLeave")]
        public virtual String CutLeave
        {
            get
            {
                return _cutleave;
            }
            set
            {
                if (_cutleave == value) { return; }
                _cutleave = value;
                this.ValidateProperty(POP.CUTLEAVE, _cutleave);
                this.NotifyPropertyChanged(POP.CUTLEAVE);
            }
        }
        private String _stratum;
        [XmlElement]
        [Field(Name = "Stratum")]
        public virtual String Stratum
        {
            get
            {
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                this.ValidateProperty(POP.STRATUM, _stratum);
                this.NotifyPropertyChanged(POP.STRATUM);
            }
        }
        private String _samplegroup;
        [XmlElement]
        [Field(Name = "SampleGroup")]
        public virtual String SampleGroup
        {
            get
            {
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                this.ValidateProperty(POP.SAMPLEGROUP, _samplegroup);
                this.NotifyPropertyChanged(POP.SAMPLEGROUP);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(POP.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(POP.PRIMARYPRODUCT);
            }
        }
        private String _secondaryproduct;
        [XmlElement]
        [Field(Name = "SecondaryProduct")]
        public virtual String SecondaryProduct
        {
            get
            {
                return _secondaryproduct;
            }
            set
            {
                if (_secondaryproduct == value) { return; }
                _secondaryproduct = value;
                this.ValidateProperty(POP.SECONDARYPRODUCT, _secondaryproduct);
                this.NotifyPropertyChanged(POP.SECONDARYPRODUCT);
            }
        }
        private String _stm;
        [XmlElement]
        [Field(Name = "STM")]
        public virtual String STM
        {
            get
            {
                return _stm;
            }
            set
            {
                if (_stm == value) { return; }
                _stm = value;
                this.ValidateProperty(POP.STM, _stm);
                this.NotifyPropertyChanged(POP.STM);
            }
        }
        private String _uom;
        [XmlElement]
        [Field(Name = "UOM")]
        public virtual String UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                if (_uom == value) { return; }
                _uom = value;
                this.ValidateProperty(POP.UOM, _uom);
                this.NotifyPropertyChanged(POP.UOM);
            }
        }
        private Double _firststagetrees = 0.0;
        [XmlElement]
        [Field(Name = "FirstStageTrees")]
        public virtual Double FirstStageTrees
        {
            get
            {
                return _firststagetrees;
            }
            set
            {
                if (Math.Abs(_firststagetrees - value) < Double.Epsilon) { return; }
                _firststagetrees = value;
                this.ValidateProperty(POP.FIRSTSTAGETREES, _firststagetrees);
                this.NotifyPropertyChanged(POP.FIRSTSTAGETREES);
            }
        }
        private Double _measuredtrees = 0.0;
        [XmlElement]
        [Field(Name = "MeasuredTrees")]
        public virtual Double MeasuredTrees
        {
            get
            {
                return _measuredtrees;
            }
            set
            {
                if (Math.Abs(_measuredtrees - value) < Double.Epsilon) { return; }
                _measuredtrees = value;
                this.ValidateProperty(POP.MEASUREDTREES, _measuredtrees);
                this.NotifyPropertyChanged(POP.MEASUREDTREES);
            }
        }
        private Double _talliedtrees = 0.0;
        [XmlElement]
        [Field(Name = "TalliedTrees")]
        public virtual Double TalliedTrees
        {
            get
            {
                return _talliedtrees;
            }
            set
            {
                if (Math.Abs(_talliedtrees - value) < Double.Epsilon) { return; }
                _talliedtrees = value;
                this.ValidateProperty(POP.TALLIEDTREES, _talliedtrees);
                this.NotifyPropertyChanged(POP.TALLIEDTREES);
            }
        }
        private Double _sumkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumKPI")]
        public virtual Double SumKPI
        {
            get
            {
                return _sumkpi;
            }
            set
            {
                if (Math.Abs(_sumkpi - value) < Double.Epsilon) { return; }
                _sumkpi = value;
                this.ValidateProperty(POP.SUMKPI, _sumkpi);
                this.NotifyPropertyChanged(POP.SUMKPI);
            }
        }
        private Double _summeasuredkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumMeasuredKPI")]
        public virtual Double SumMeasuredKPI
        {
            get
            {
                return _summeasuredkpi;
            }
            set
            {
                if (Math.Abs(_summeasuredkpi - value) < Double.Epsilon) { return; }
                _summeasuredkpi = value;
                this.ValidateProperty(POP.SUMMEASUREDKPI, _summeasuredkpi);
                this.NotifyPropertyChanged(POP.SUMMEASUREDKPI);
            }
        }
        private Double _stageonesamples = 0.0;
        [XmlElement]
        [Field(Name = "StageOneSamples")]
        public virtual Double StageOneSamples
        {
            get
            {
                return _stageonesamples;
            }
            set
            {
                if (Math.Abs(_stageonesamples - value) < Double.Epsilon) { return; }
                _stageonesamples = value;
                this.ValidateProperty(POP.STAGEONESAMPLES, _stageonesamples);
                this.NotifyPropertyChanged(POP.STAGEONESAMPLES);
            }
        }
        private Double _stagetwosamples = 0.0;
        [XmlElement]
        [Field(Name = "StageTwoSamples")]
        public virtual Double StageTwoSamples
        {
            get
            {
                return _stagetwosamples;
            }
            set
            {
                if (Math.Abs(_stagetwosamples - value) < Double.Epsilon) { return; }
                _stagetwosamples = value;
                this.ValidateProperty(POP.STAGETWOSAMPLES, _stagetwosamples);
                this.NotifyPropertyChanged(POP.STAGETWOSAMPLES);
            }
        }
        private Double _stg1grossxpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXPP")]
        public virtual Double Stg1GrossXPP
        {
            get
            {
                return _stg1grossxpp;
            }
            set
            {
                if (Math.Abs(_stg1grossxpp - value) < Double.Epsilon) { return; }
                _stg1grossxpp = value;
                this.ValidateProperty(POP.STG1GROSSXPP, _stg1grossxpp);
                this.NotifyPropertyChanged(POP.STG1GROSSXPP);
            }
        }
        private Double _stg1grossxsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXsqrdPP")]
        public virtual Double Stg1GrossXsqrdPP
        {
            get
            {
                return _stg1grossxsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg1grossxsqrdpp - value) < Double.Epsilon) { return; }
                _stg1grossxsqrdpp = value;
                this.ValidateProperty(POP.STG1GROSSXSQRDPP, _stg1grossxsqrdpp);
                this.NotifyPropertyChanged(POP.STG1GROSSXSQRDPP);
            }
        }
        private Double _stg1netxpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXPP")]
        public virtual Double Stg1NetXPP
        {
            get
            {
                return _stg1netxpp;
            }
            set
            {
                if (Math.Abs(_stg1netxpp - value) < Double.Epsilon) { return; }
                _stg1netxpp = value;
                this.ValidateProperty(POP.STG1NETXPP, _stg1netxpp);
                this.NotifyPropertyChanged(POP.STG1NETXPP);
            }
        }
        private Double _stg1netxsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXsqrdPP")]
        public virtual Double Stg1NetXsqrdPP
        {
            get
            {
                return _stg1netxsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg1netxsqrdpp - value) < Double.Epsilon) { return; }
                _stg1netxsqrdpp = value;
                this.ValidateProperty(POP.STG1NETXSQRDPP, _stg1netxsqrdpp);
                this.NotifyPropertyChanged(POP.STG1NETXSQRDPP);
            }
        }
        private Double _stg1valuexpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXPP")]
        public virtual Double Stg1ValueXPP
        {
            get
            {
                return _stg1valuexpp;
            }
            set
            {
                if (Math.Abs(_stg1valuexpp - value) < Double.Epsilon) { return; }
                _stg1valuexpp = value;
                this.ValidateProperty(POP.STG1VALUEXPP, _stg1valuexpp);
                this.NotifyPropertyChanged(POP.STG1VALUEXPP);
            }
        }
        private Double _stg1valuexsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXsqrdPP")]
        public virtual Double Stg1ValueXsqrdPP
        {
            get
            {
                return _stg1valuexsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg1valuexsqrdpp - value) < Double.Epsilon) { return; }
                _stg1valuexsqrdpp = value;
                this.ValidateProperty(POP.STG1VALUEXSQRDPP, _stg1valuexsqrdpp);
                this.NotifyPropertyChanged(POP.STG1VALUEXSQRDPP);
            }
        }
        private Double _stg2grossxpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXPP")]
        public virtual Double Stg2GrossXPP
        {
            get
            {
                return _stg2grossxpp;
            }
            set
            {
                if (Math.Abs(_stg2grossxpp - value) < Double.Epsilon) { return; }
                _stg2grossxpp = value;
                this.ValidateProperty(POP.STG2GROSSXPP, _stg2grossxpp);
                this.NotifyPropertyChanged(POP.STG2GROSSXPP);
            }
        }
        private Double _stg2grossxsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXsqrdPP")]
        public virtual Double Stg2GrossXsqrdPP
        {
            get
            {
                return _stg2grossxsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg2grossxsqrdpp - value) < Double.Epsilon) { return; }
                _stg2grossxsqrdpp = value;
                this.ValidateProperty(POP.STG2GROSSXSQRDPP, _stg2grossxsqrdpp);
                this.NotifyPropertyChanged(POP.STG2GROSSXSQRDPP);
            }
        }
        private Double _stg2netxpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXPP")]
        public virtual Double Stg2NetXPP
        {
            get
            {
                return _stg2netxpp;
            }
            set
            {
                if (Math.Abs(_stg2netxpp - value) < Double.Epsilon) { return; }
                _stg2netxpp = value;
                this.ValidateProperty(POP.STG2NETXPP, _stg2netxpp);
                this.NotifyPropertyChanged(POP.STG2NETXPP);
            }
        }
        private Double _stg2netxsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXsqrdPP")]
        public virtual Double Stg2NetXsqrdPP
        {
            get
            {
                return _stg2netxsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg2netxsqrdpp - value) < Double.Epsilon) { return; }
                _stg2netxsqrdpp = value;
                this.ValidateProperty(POP.STG2NETXSQRDPP, _stg2netxsqrdpp);
                this.NotifyPropertyChanged(POP.STG2NETXSQRDPP);
            }
        }
        private Double _stg2valuexpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXPP")]
        public virtual Double Stg2ValueXPP
        {
            get
            {
                return _stg2valuexpp;
            }
            set
            {
                if (Math.Abs(_stg2valuexpp - value) < Double.Epsilon) { return; }
                _stg2valuexpp = value;
                this.ValidateProperty(POP.STG2VALUEXPP, _stg2valuexpp);
                this.NotifyPropertyChanged(POP.STG2VALUEXPP);
            }
        }
        private Double _stg2valuexsqrdpp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXsqrdPP")]
        public virtual Double Stg2ValueXsqrdPP
        {
            get
            {
                return _stg2valuexsqrdpp;
            }
            set
            {
                if (Math.Abs(_stg2valuexsqrdpp - value) < Double.Epsilon) { return; }
                _stg2valuexsqrdpp = value;
                this.ValidateProperty(POP.STG2VALUEXSQRDPP, _stg2valuexsqrdpp);
                this.NotifyPropertyChanged(POP.STG2VALUEXSQRDPP);
            }
        }
        private Double _stg1grossxsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXSP")]
        public virtual Double Stg1GrossXSP
        {
            get
            {
                return _stg1grossxsp;
            }
            set
            {
                if (Math.Abs(_stg1grossxsp - value) < Double.Epsilon) { return; }
                _stg1grossxsp = value;
                this.ValidateProperty(POP.STG1GROSSXSP, _stg1grossxsp);
                this.NotifyPropertyChanged(POP.STG1GROSSXSP);
            }
        }
        private Double _stg1grossxsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXsqrdSP")]
        public virtual Double Stg1GrossXsqrdSP
        {
            get
            {
                return _stg1grossxsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg1grossxsqrdsp - value) < Double.Epsilon) { return; }
                _stg1grossxsqrdsp = value;
                this.ValidateProperty(POP.STG1GROSSXSQRDSP, _stg1grossxsqrdsp);
                this.NotifyPropertyChanged(POP.STG1GROSSXSQRDSP);
            }
        }
        private Double _stg1netxsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXSP")]
        public virtual Double Stg1NetXSP
        {
            get
            {
                return _stg1netxsp;
            }
            set
            {
                if (Math.Abs(_stg1netxsp - value) < Double.Epsilon) { return; }
                _stg1netxsp = value;
                this.ValidateProperty(POP.STG1NETXSP, _stg1netxsp);
                this.NotifyPropertyChanged(POP.STG1NETXSP);
            }
        }
        private Double _stg1netxsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXsqrdSP")]
        public virtual Double Stg1NetXsqrdSP
        {
            get
            {
                return _stg1netxsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg1netxsqrdsp - value) < Double.Epsilon) { return; }
                _stg1netxsqrdsp = value;
                this.ValidateProperty(POP.STG1NETXSQRDSP, _stg1netxsqrdsp);
                this.NotifyPropertyChanged(POP.STG1NETXSQRDSP);
            }
        }
        private Double _stg1valuexsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXSP")]
        public virtual Double Stg1ValueXSP
        {
            get
            {
                return _stg1valuexsp;
            }
            set
            {
                if (Math.Abs(_stg1valuexsp - value) < Double.Epsilon) { return; }
                _stg1valuexsp = value;
                this.ValidateProperty(POP.STG1VALUEXSP, _stg1valuexsp);
                this.NotifyPropertyChanged(POP.STG1VALUEXSP);
            }
        }
        private Double _stg1valuexsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXsqrdSP")]
        public virtual Double Stg1ValueXsqrdSP
        {
            get
            {
                return _stg1valuexsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg1valuexsqrdsp - value) < Double.Epsilon) { return; }
                _stg1valuexsqrdsp = value;
                this.ValidateProperty(POP.STG1VALUEXSQRDSP, _stg1valuexsqrdsp);
                this.NotifyPropertyChanged(POP.STG1VALUEXSQRDSP);
            }
        }
        private Double _stg2grossxsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXSP")]
        public virtual Double Stg2GrossXSP
        {
            get
            {
                return _stg2grossxsp;
            }
            set
            {
                if (Math.Abs(_stg2grossxsp - value) < Double.Epsilon) { return; }
                _stg2grossxsp = value;
                this.ValidateProperty(POP.STG2GROSSXSP, _stg2grossxsp);
                this.NotifyPropertyChanged(POP.STG2GROSSXSP);
            }
        }
        private Double _stg2grossxsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXsqrdSP")]
        public virtual Double Stg2GrossXsqrdSP
        {
            get
            {
                return _stg2grossxsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg2grossxsqrdsp - value) < Double.Epsilon) { return; }
                _stg2grossxsqrdsp = value;
                this.ValidateProperty(POP.STG2GROSSXSQRDSP, _stg2grossxsqrdsp);
                this.NotifyPropertyChanged(POP.STG2GROSSXSQRDSP);
            }
        }
        private Double _stg2netxsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXSP")]
        public virtual Double Stg2NetXSP
        {
            get
            {
                return _stg2netxsp;
            }
            set
            {
                if (Math.Abs(_stg2netxsp - value) < Double.Epsilon) { return; }
                _stg2netxsp = value;
                this.ValidateProperty(POP.STG2NETXSP, _stg2netxsp);
                this.NotifyPropertyChanged(POP.STG2NETXSP);
            }
        }
        private Double _stg2netxsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXsqrdSP")]
        public virtual Double Stg2NetXsqrdSP
        {
            get
            {
                return _stg2netxsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg2netxsqrdsp - value) < Double.Epsilon) { return; }
                _stg2netxsqrdsp = value;
                this.ValidateProperty(POP.STG2NETXSQRDSP, _stg2netxsqrdsp);
                this.NotifyPropertyChanged(POP.STG2NETXSQRDSP);
            }
        }
        private Double _stg2valuexsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXSP")]
        public virtual Double Stg2ValueXSP
        {
            get
            {
                return _stg2valuexsp;
            }
            set
            {
                if (Math.Abs(_stg2valuexsp - value) < Double.Epsilon) { return; }
                _stg2valuexsp = value;
                this.ValidateProperty(POP.STG2VALUEXSP, _stg2valuexsp);
                this.NotifyPropertyChanged(POP.STG2VALUEXSP);
            }
        }
        private Double _stg2valuexsqrdsp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXsqrdSP")]
        public virtual Double Stg2ValueXsqrdSP
        {
            get
            {
                return _stg2valuexsqrdsp;
            }
            set
            {
                if (Math.Abs(_stg2valuexsqrdsp - value) < Double.Epsilon) { return; }
                _stg2valuexsqrdsp = value;
                this.ValidateProperty(POP.STG2VALUEXSQRDSP, _stg2valuexsqrdsp);
                this.NotifyPropertyChanged(POP.STG2VALUEXSQRDSP);
            }
        }
        private Double _stg1grossxrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXRP")]
        public virtual Double Stg1GrossXRP
        {
            get
            {
                return _stg1grossxrp;
            }
            set
            {
                if (Math.Abs(_stg1grossxrp - value) < Double.Epsilon) { return; }
                _stg1grossxrp = value;
                this.ValidateProperty(POP.STG1GROSSXRP, _stg1grossxrp);
                this.NotifyPropertyChanged(POP.STG1GROSSXRP);
            }
        }
        private Double _stg1grossxsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1GrossXsqrdRP")]
        public virtual Double Stg1GrossXsqrdRP
        {
            get
            {
                return _stg1grossxsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg1grossxsqrdrp - value) < Double.Epsilon) { return; }
                _stg1grossxsqrdrp = value;
                this.ValidateProperty(POP.STG1GROSSXSQRDRP, _stg1grossxsqrdrp);
                this.NotifyPropertyChanged(POP.STG1GROSSXSQRDRP);
            }
        }
        private Double _stg1netxrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXRP")]
        public virtual Double Stg1NetXRP
        {
            get
            {
                return _stg1netxrp;
            }
            set
            {
                if (Math.Abs(_stg1netxrp - value) < Double.Epsilon) { return; }
                _stg1netxrp = value;
                this.ValidateProperty(POP.STG1NETXRP, _stg1netxrp);
                this.NotifyPropertyChanged(POP.STG1NETXRP);
            }
        }
        private Double _stg1netxrsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1NetXRsqrdRP")]
        public virtual Double Stg1NetXRsqrdRP
        {
            get
            {
                return _stg1netxrsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg1netxrsqrdrp - value) < Double.Epsilon) { return; }
                _stg1netxrsqrdrp = value;
                this.ValidateProperty(POP.STG1NETXRSQRDRP, _stg1netxrsqrdrp);
                this.NotifyPropertyChanged(POP.STG1NETXRSQRDRP);
            }
        }
        private Double _stg1valuexrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXRP")]
        public virtual Double Stg1ValueXRP
        {
            get
            {
                return _stg1valuexrp;
            }
            set
            {
                if (Math.Abs(_stg1valuexrp - value) < Double.Epsilon) { return; }
                _stg1valuexrp = value;
                this.ValidateProperty(POP.STG1VALUEXRP, _stg1valuexrp);
                this.NotifyPropertyChanged(POP.STG1VALUEXRP);
            }
        }
        private Double _stg1valuexsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg1ValueXsqrdRP")]
        public virtual Double Stg1ValueXsqrdRP
        {
            get
            {
                return _stg1valuexsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg1valuexsqrdrp - value) < Double.Epsilon) { return; }
                _stg1valuexsqrdrp = value;
                this.ValidateProperty(POP.STG1VALUEXSQRDRP, _stg1valuexsqrdrp);
                this.NotifyPropertyChanged(POP.STG1VALUEXSQRDRP);
            }
        }
        private Double _stg2grossxrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXRP")]
        public virtual Double Stg2GrossXRP
        {
            get
            {
                return _stg2grossxrp;
            }
            set
            {
                if (Math.Abs(_stg2grossxrp - value) < Double.Epsilon) { return; }
                _stg2grossxrp = value;
                this.ValidateProperty(POP.STG2GROSSXRP, _stg2grossxrp);
                this.NotifyPropertyChanged(POP.STG2GROSSXRP);
            }
        }
        private Double _stg2grossxsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2GrossXsqrdRP")]
        public virtual Double Stg2GrossXsqrdRP
        {
            get
            {
                return _stg2grossxsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg2grossxsqrdrp - value) < Double.Epsilon) { return; }
                _stg2grossxsqrdrp = value;
                this.ValidateProperty(POP.STG2GROSSXSQRDRP, _stg2grossxsqrdrp);
                this.NotifyPropertyChanged(POP.STG2GROSSXSQRDRP);
            }
        }
        private Double _stg2netxrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXRP")]
        public virtual Double Stg2NetXRP
        {
            get
            {
                return _stg2netxrp;
            }
            set
            {
                if (Math.Abs(_stg2netxrp - value) < Double.Epsilon) { return; }
                _stg2netxrp = value;
                this.ValidateProperty(POP.STG2NETXRP, _stg2netxrp);
                this.NotifyPropertyChanged(POP.STG2NETXRP);
            }
        }
        private Double _stg2netxsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2NetXsqrdRP")]
        public virtual Double Stg2NetXsqrdRP
        {
            get
            {
                return _stg2netxsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg2netxsqrdrp - value) < Double.Epsilon) { return; }
                _stg2netxsqrdrp = value;
                this.ValidateProperty(POP.STG2NETXSQRDRP, _stg2netxsqrdrp);
                this.NotifyPropertyChanged(POP.STG2NETXSQRDRP);
            }
        }
        private Double _stg2valuexrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXRP")]
        public virtual Double Stg2ValueXRP
        {
            get
            {
                return _stg2valuexrp;
            }
            set
            {
                if (Math.Abs(_stg2valuexrp - value) < Double.Epsilon) { return; }
                _stg2valuexrp = value;
                this.ValidateProperty(POP.STG2VALUEXRP, _stg2valuexrp);
                this.NotifyPropertyChanged(POP.STG2VALUEXRP);
            }
        }
        private Double _stg2valuexsqrdrp = 0.0;
        [XmlElement]
        [Field(Name = "Stg2ValueXsqrdRP")]
        public virtual Double Stg2ValueXsqrdRP
        {
            get
            {
                return _stg2valuexsqrdrp;
            }
            set
            {
                if (Math.Abs(_stg2valuexsqrdrp - value) < Double.Epsilon) { return; }
                _stg2valuexsqrdrp = value;
                this.ValidateProperty(POP.STG2VALUEXSQRDRP, _stg2valuexsqrdrp);
                this.NotifyPropertyChanged(POP.STG2VALUEXSQRDRP);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("CutLeave", this.CutLeave) && isValid;
            isValid = ValidateProperty("Stratum", this.Stratum) && isValid;
            isValid = ValidateProperty("SampleGroup", this.SampleGroup) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("SecondaryProduct", this.SecondaryProduct) && isValid;
            isValid = ValidateProperty("STM", this.STM) && isValid;
            isValid = ValidateProperty("UOM", this.UOM) && isValid;
            isValid = ValidateProperty("FirstStageTrees", this.FirstStageTrees) && isValid;
            isValid = ValidateProperty("MeasuredTrees", this.MeasuredTrees) && isValid;
            isValid = ValidateProperty("TalliedTrees", this.TalliedTrees) && isValid;
            isValid = ValidateProperty("SumKPI", this.SumKPI) && isValid;
            isValid = ValidateProperty("SumMeasuredKPI", this.SumMeasuredKPI) && isValid;
            isValid = ValidateProperty("StageOneSamples", this.StageOneSamples) && isValid;
            isValid = ValidateProperty("StageTwoSamples", this.StageTwoSamples) && isValid;
            isValid = ValidateProperty("Stg1GrossXPP", this.Stg1GrossXPP) && isValid;
            isValid = ValidateProperty("Stg1GrossXsqrdPP", this.Stg1GrossXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg1NetXPP", this.Stg1NetXPP) && isValid;
            isValid = ValidateProperty("Stg1NetXsqrdPP", this.Stg1NetXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg1ValueXPP", this.Stg1ValueXPP) && isValid;
            isValid = ValidateProperty("Stg1ValueXsqrdPP", this.Stg1ValueXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg2GrossXPP", this.Stg2GrossXPP) && isValid;
            isValid = ValidateProperty("Stg2GrossXsqrdPP", this.Stg2GrossXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg2NetXPP", this.Stg2NetXPP) && isValid;
            isValid = ValidateProperty("Stg2NetXsqrdPP", this.Stg2NetXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg2ValueXPP", this.Stg2ValueXPP) && isValid;
            isValid = ValidateProperty("Stg2ValueXsqrdPP", this.Stg2ValueXsqrdPP) && isValid;
            isValid = ValidateProperty("Stg1GrossXSP", this.Stg1GrossXSP) && isValid;
            isValid = ValidateProperty("Stg1GrossXsqrdSP", this.Stg1GrossXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg1NetXSP", this.Stg1NetXSP) && isValid;
            isValid = ValidateProperty("Stg1NetXsqrdSP", this.Stg1NetXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg1ValueXSP", this.Stg1ValueXSP) && isValid;
            isValid = ValidateProperty("Stg1ValueXsqrdSP", this.Stg1ValueXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg2GrossXSP", this.Stg2GrossXSP) && isValid;
            isValid = ValidateProperty("Stg2GrossXsqrdSP", this.Stg2GrossXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg2NetXSP", this.Stg2NetXSP) && isValid;
            isValid = ValidateProperty("Stg2NetXsqrdSP", this.Stg2NetXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg2ValueXSP", this.Stg2ValueXSP) && isValid;
            isValid = ValidateProperty("Stg2ValueXsqrdSP", this.Stg2ValueXsqrdSP) && isValid;
            isValid = ValidateProperty("Stg1GrossXRP", this.Stg1GrossXRP) && isValid;
            isValid = ValidateProperty("Stg1GrossXsqrdRP", this.Stg1GrossXsqrdRP) && isValid;
            isValid = ValidateProperty("Stg1NetXRP", this.Stg1NetXRP) && isValid;
            isValid = ValidateProperty("Stg1NetXRsqrdRP", this.Stg1NetXRsqrdRP) && isValid;
            isValid = ValidateProperty("Stg1ValueXRP", this.Stg1ValueXRP) && isValid;
            isValid = ValidateProperty("Stg1ValueXsqrdRP", this.Stg1ValueXsqrdRP) && isValid;
            isValid = ValidateProperty("Stg2GrossXRP", this.Stg2GrossXRP) && isValid;
            isValid = ValidateProperty("Stg2GrossXsqrdRP", this.Stg2GrossXsqrdRP) && isValid;
            isValid = ValidateProperty("Stg2NetXRP", this.Stg2NetXRP) && isValid;
            isValid = ValidateProperty("Stg2NetXsqrdRP", this.Stg2NetXsqrdRP) && isValid;
            isValid = ValidateProperty("Stg2ValueXRP", this.Stg2ValueXRP) && isValid;
            isValid = ValidateProperty("Stg2ValueXsqrdRP", this.Stg2ValueXsqrdRP) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as POPDO);
        }

        public void SetValues(POPDO obj)
        {
            if (obj == null) { return; }
            CutLeave = obj.CutLeave;
            Stratum = obj.Stratum;
            SampleGroup = obj.SampleGroup;
            PrimaryProduct = obj.PrimaryProduct;
            SecondaryProduct = obj.SecondaryProduct;
            STM = obj.STM;
            UOM = obj.UOM;
            FirstStageTrees = obj.FirstStageTrees;
            MeasuredTrees = obj.MeasuredTrees;
            TalliedTrees = obj.TalliedTrees;
            SumKPI = obj.SumKPI;
            SumMeasuredKPI = obj.SumMeasuredKPI;
            StageOneSamples = obj.StageOneSamples;
            StageTwoSamples = obj.StageTwoSamples;
            Stg1GrossXPP = obj.Stg1GrossXPP;
            Stg1GrossXsqrdPP = obj.Stg1GrossXsqrdPP;
            Stg1NetXPP = obj.Stg1NetXPP;
            Stg1NetXsqrdPP = obj.Stg1NetXsqrdPP;
            Stg1ValueXPP = obj.Stg1ValueXPP;
            Stg1ValueXsqrdPP = obj.Stg1ValueXsqrdPP;
            Stg2GrossXPP = obj.Stg2GrossXPP;
            Stg2GrossXsqrdPP = obj.Stg2GrossXsqrdPP;
            Stg2NetXPP = obj.Stg2NetXPP;
            Stg2NetXsqrdPP = obj.Stg2NetXsqrdPP;
            Stg2ValueXPP = obj.Stg2ValueXPP;
            Stg2ValueXsqrdPP = obj.Stg2ValueXsqrdPP;
            Stg1GrossXSP = obj.Stg1GrossXSP;
            Stg1GrossXsqrdSP = obj.Stg1GrossXsqrdSP;
            Stg1NetXSP = obj.Stg1NetXSP;
            Stg1NetXsqrdSP = obj.Stg1NetXsqrdSP;
            Stg1ValueXSP = obj.Stg1ValueXSP;
            Stg1ValueXsqrdSP = obj.Stg1ValueXsqrdSP;
            Stg2GrossXSP = obj.Stg2GrossXSP;
            Stg2GrossXsqrdSP = obj.Stg2GrossXsqrdSP;
            Stg2NetXSP = obj.Stg2NetXSP;
            Stg2NetXsqrdSP = obj.Stg2NetXsqrdSP;
            Stg2ValueXSP = obj.Stg2ValueXSP;
            Stg2ValueXsqrdSP = obj.Stg2ValueXsqrdSP;
            Stg1GrossXRP = obj.Stg1GrossXRP;
            Stg1GrossXsqrdRP = obj.Stg1GrossXsqrdRP;
            Stg1NetXRP = obj.Stg1NetXRP;
            Stg1NetXRsqrdRP = obj.Stg1NetXRsqrdRP;
            Stg1ValueXRP = obj.Stg1ValueXRP;
            Stg1ValueXsqrdRP = obj.Stg1ValueXsqrdRP;
            Stg2GrossXRP = obj.Stg2GrossXRP;
            Stg2GrossXsqrdRP = obj.Stg2GrossXsqrdRP;
            Stg2NetXRP = obj.Stg2NetXRP;
            Stg2NetXsqrdRP = obj.Stg2NetXsqrdRP;
            Stg2ValueXRP = obj.Stg2ValueXRP;
            Stg2ValueXsqrdRP = obj.Stg2ValueXsqrdRP;
        }
    }
    [Table("PRO")]
    public partial class PRODO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static PRODO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("CutLeave", "PRO", "CutLeave is Required"));
            _validator.Add(new NotNullRule("Stratum", "PRO", "Stratum is Required"));
            _validator.Add(new NotNullRule("CuttingUnit", "PRO", "CuttingUnit is Required"));
            _validator.Add(new NotNullRule("SampleGroup", "PRO", "SampleGroup is Required"));
            _validator.Add(new NotNullRule("PrimaryProduct", "PRO", "PrimaryProduct is Required"));
            _validator.Add(new NotNullRule("SecondaryProduct", "PRO", "SecondaryProduct is Required"));
            _validator.Add(new NotNullRule("UOM", "PRO", "UOM is Required"));
        }

        public PRODO() { }

        public PRODO(PRODO obj) : this()
        {
            SetValues(obj);
        }

        public PRODO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "PRO_CN")]
        public Int64? PRO_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _cutleave;
        [XmlElement]
        [Field(Name = "CutLeave")]
        public virtual String CutLeave
        {
            get
            {
                return _cutleave;
            }
            set
            {
                if (_cutleave == value) { return; }
                _cutleave = value;
                this.ValidateProperty(PRO.CUTLEAVE, _cutleave);
                this.NotifyPropertyChanged(PRO.CUTLEAVE);
            }
        }
        private String _stratum;
        [XmlElement]
        [Field(Name = "Stratum")]
        public virtual String Stratum
        {
            get
            {
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                this.ValidateProperty(PRO.STRATUM, _stratum);
                this.NotifyPropertyChanged(PRO.STRATUM);
            }
        }
        private String _cuttingunit;
        [XmlElement]
        [Field(Name = "CuttingUnit")]
        public virtual String CuttingUnit
        {
            get
            {
                return _cuttingunit;
            }
            set
            {
                if (_cuttingunit == value) { return; }
                _cuttingunit = value;
                this.ValidateProperty(PRO.CUTTINGUNIT, _cuttingunit);
                this.NotifyPropertyChanged(PRO.CUTTINGUNIT);
            }
        }
        private String _samplegroup;
        [XmlElement]
        [Field(Name = "SampleGroup")]
        public virtual String SampleGroup
        {
            get
            {
                return _samplegroup;
            }
            set
            {
                if (_samplegroup == value) { return; }
                _samplegroup = value;
                this.ValidateProperty(PRO.SAMPLEGROUP, _samplegroup);
                this.NotifyPropertyChanged(PRO.SAMPLEGROUP);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(PRO.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(PRO.PRIMARYPRODUCT);
            }
        }
        private String _secondaryproduct;
        [XmlElement]
        [Field(Name = "SecondaryProduct")]
        public virtual String SecondaryProduct
        {
            get
            {
                return _secondaryproduct;
            }
            set
            {
                if (_secondaryproduct == value) { return; }
                _secondaryproduct = value;
                this.ValidateProperty(PRO.SECONDARYPRODUCT, _secondaryproduct);
                this.NotifyPropertyChanged(PRO.SECONDARYPRODUCT);
            }
        }
        private String _uom;
        [XmlElement]
        [Field(Name = "UOM")]
        public virtual String UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                if (_uom == value) { return; }
                _uom = value;
                this.ValidateProperty(PRO.UOM, _uom);
                this.NotifyPropertyChanged(PRO.UOM);
            }
        }
        private String _stm;
        [XmlElement]
        [Field(Name = "STM")]
        public virtual String STM
        {
            get
            {
                return _stm;
            }
            set
            {
                if (_stm == value) { return; }
                _stm = value;
                this.ValidateProperty(PRO.STM, _stm);
                this.NotifyPropertyChanged(PRO.STM);
            }
        }
        private Double _firststagetrees = 0.0;
        [XmlElement]
        [Field(Name = "FirstStageTrees")]
        public virtual Double FirstStageTrees
        {
            get
            {
                return _firststagetrees;
            }
            set
            {
                if (Math.Abs(_firststagetrees - value) < Double.Epsilon) { return; }
                _firststagetrees = value;
                this.ValidateProperty(PRO.FIRSTSTAGETREES, _firststagetrees);
                this.NotifyPropertyChanged(PRO.FIRSTSTAGETREES);
            }
        }
        private Double _measuredtrees = 0.0;
        [XmlElement]
        [Field(Name = "MeasuredTrees")]
        public virtual Double MeasuredTrees
        {
            get
            {
                return _measuredtrees;
            }
            set
            {
                if (Math.Abs(_measuredtrees - value) < Double.Epsilon) { return; }
                _measuredtrees = value;
                this.ValidateProperty(PRO.MEASUREDTREES, _measuredtrees);
                this.NotifyPropertyChanged(PRO.MEASUREDTREES);
            }
        }
        private Double _talliedtrees = 0.0;
        [XmlElement]
        [Field(Name = "TalliedTrees")]
        public virtual Double TalliedTrees
        {
            get
            {
                return _talliedtrees;
            }
            set
            {
                if (Math.Abs(_talliedtrees - value) < Double.Epsilon) { return; }
                _talliedtrees = value;
                this.ValidateProperty(PRO.TALLIEDTREES, _talliedtrees);
                this.NotifyPropertyChanged(PRO.TALLIEDTREES);
            }
        }
        private Double _sumkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumKPI")]
        public virtual Double SumKPI
        {
            get
            {
                return _sumkpi;
            }
            set
            {
                if (Math.Abs(_sumkpi - value) < Double.Epsilon) { return; }
                _sumkpi = value;
                this.ValidateProperty(PRO.SUMKPI, _sumkpi);
                this.NotifyPropertyChanged(PRO.SUMKPI);
            }
        }
        private Double _summeasuredkpi = 0.0;
        [XmlElement]
        [Field(Name = "SumMeasuredKPI")]
        public virtual Double SumMeasuredKPI
        {
            get
            {
                return _summeasuredkpi;
            }
            set
            {
                if (Math.Abs(_summeasuredkpi - value) < Double.Epsilon) { return; }
                _summeasuredkpi = value;
                this.ValidateProperty(PRO.SUMMEASUREDKPI, _summeasuredkpi);
                this.NotifyPropertyChanged(PRO.SUMMEASUREDKPI);
            }
        }
        private Double _prorationfactor = 0.0;
        [XmlElement]
        [Field(Name = "ProrationFactor")]
        public virtual Double ProrationFactor
        {
            get
            {
                return _prorationfactor;
            }
            set
            {
                if (Math.Abs(_prorationfactor - value) < Double.Epsilon) { return; }
                _prorationfactor = value;
                this.ValidateProperty(PRO.PRORATIONFACTOR, _prorationfactor);
                this.NotifyPropertyChanged(PRO.PRORATIONFACTOR);
            }
        }
        private Double _proratedestimatedtrees = 0.0;
        [XmlElement]
        [Field(Name = "ProratedEstimatedTrees")]
        public virtual Double ProratedEstimatedTrees
        {
            get
            {
                return _proratedestimatedtrees;
            }
            set
            {
                if (Math.Abs(_proratedestimatedtrees - value) < Double.Epsilon) { return; }
                _proratedestimatedtrees = value;
                this.ValidateProperty(PRO.PRORATEDESTIMATEDTREES, _proratedestimatedtrees);
                this.NotifyPropertyChanged(PRO.PRORATEDESTIMATEDTREES);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("CutLeave", this.CutLeave) && isValid;
            isValid = ValidateProperty("Stratum", this.Stratum) && isValid;
            isValid = ValidateProperty("CuttingUnit", this.CuttingUnit) && isValid;
            isValid = ValidateProperty("SampleGroup", this.SampleGroup) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("SecondaryProduct", this.SecondaryProduct) && isValid;
            isValid = ValidateProperty("UOM", this.UOM) && isValid;
            isValid = ValidateProperty("STM", this.STM) && isValid;
            isValid = ValidateProperty("FirstStageTrees", this.FirstStageTrees) && isValid;
            isValid = ValidateProperty("MeasuredTrees", this.MeasuredTrees) && isValid;
            isValid = ValidateProperty("TalliedTrees", this.TalliedTrees) && isValid;
            isValid = ValidateProperty("SumKPI", this.SumKPI) && isValid;
            isValid = ValidateProperty("SumMeasuredKPI", this.SumMeasuredKPI) && isValid;
            isValid = ValidateProperty("ProrationFactor", this.ProrationFactor) && isValid;
            isValid = ValidateProperty("ProratedEstimatedTrees", this.ProratedEstimatedTrees) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as PRODO);
        }

        public void SetValues(PRODO obj)
        {
            if (obj == null) { return; }
            CutLeave = obj.CutLeave;
            Stratum = obj.Stratum;
            CuttingUnit = obj.CuttingUnit;
            SampleGroup = obj.SampleGroup;
            PrimaryProduct = obj.PrimaryProduct;
            SecondaryProduct = obj.SecondaryProduct;
            UOM = obj.UOM;
            STM = obj.STM;
            FirstStageTrees = obj.FirstStageTrees;
            MeasuredTrees = obj.MeasuredTrees;
            TalliedTrees = obj.TalliedTrees;
            SumKPI = obj.SumKPI;
            SumMeasuredKPI = obj.SumMeasuredKPI;
            ProrationFactor = obj.ProrationFactor;
            ProratedEstimatedTrees = obj.ProratedEstimatedTrees;
        }
    }
    [Table("LogStock")]
    public partial class LogStockDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogStockDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Tree_CN", "LogStock", "Tree_CN is Required"));
            _validator.Add(new NotNullRule("LogNumber", "LogStock", "LogNumber is Required"));
        }

        public LogStockDO() { }

        public LogStockDO(LogStockDO obj) : this()
        {
            SetValues(obj);
        }

        public LogStockDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "LogStock_CN")]
        public Int64? LogStock_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _tree_cn;
        [XmlIgnore]
        [Field(Name = "Tree_CN")]
        public virtual long? Tree_CN
        {
            get
            {

                if (_tree != null)
                {
                    return _tree.Tree_CN;
                }
                return _tree_cn;
            }
            set
            {
                if (_tree_cn == value) { return; }
                if (value == null || value.Value == 0) { _tree = null; }
                _tree_cn = value;
                this.ValidateProperty(LOGSTOCK.TREE_CN, _tree_cn);
                this.NotifyPropertyChanged(LOGSTOCK.TREE_CN);
            }
        }
        public virtual TreeDO GetTree()
        {
            if (DAL == null) { return null; }
            if (this.Tree_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDO>(this.Tree_CN);
            //return DAL.ReadSingleRow<TreeDO>(TREE._NAME, this.Tree_CN);
        }

        private TreeDO _tree = null;
        [XmlIgnore]
        public TreeDO Tree
        {
            get
            {
                if (_tree == null)
                {
                    _tree = GetTree();
                }
                return _tree;
            }
            set
            {
                if (_tree == value) { return; }
                _tree = value;
                Tree_CN = (value != null) ? value.Tree_CN : null;
            }
        }
        private String _lognumber;
        [XmlElement]
        [Field(Name = "LogNumber")]
        public virtual String LogNumber
        {
            get
            {
                return _lognumber;
            }
            set
            {
                if (_lognumber == value) { return; }
                _lognumber = value;
                this.ValidateProperty(LOGSTOCK.LOGNUMBER, _lognumber);
                this.NotifyPropertyChanged(LOGSTOCK.LOGNUMBER);
            }
        }
        private String _grade;
        [XmlElement]
        [Field(Name = "Grade")]
        public virtual String Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (_grade == value) { return; }
                _grade = value;
                this.ValidateProperty(LOGSTOCK.GRADE, _grade);
                this.NotifyPropertyChanged(LOGSTOCK.GRADE);
            }
        }
        private float _seendefect = 0.0f;
        [XmlElement]
        [Field(Name = "SeenDefect")]
        public virtual float SeenDefect
        {
            get
            {
                return _seendefect;
            }
            set
            {
                if (Math.Abs(_seendefect - value) < float.Epsilon) { return; }
                _seendefect = value;
                this.ValidateProperty(LOGSTOCK.SEENDEFECT, _seendefect);
                this.NotifyPropertyChanged(LOGSTOCK.SEENDEFECT);
            }
        }
        private float _percentrecoverable = 0.0f;
        [XmlElement]
        [Field(Name = "PercentRecoverable")]
        public virtual float PercentRecoverable
        {
            get
            {
                return _percentrecoverable;
            }
            set
            {
                if (Math.Abs(_percentrecoverable - value) < float.Epsilon) { return; }
                _percentrecoverable = value;
                this.ValidateProperty(LOGSTOCK.PERCENTRECOVERABLE, _percentrecoverable);
                this.NotifyPropertyChanged(LOGSTOCK.PERCENTRECOVERABLE);
            }
        }
        private Int64 _length;
        [XmlElement]
        [Field(Name = "Length")]
        public virtual Int64 Length
        {
            get
            {
                return _length;
            }
            set
            {
                if (_length == value) { return; }
                _length = value;
                this.ValidateProperty(LOGSTOCK.LENGTH, _length);
                this.NotifyPropertyChanged(LOGSTOCK.LENGTH);
            }
        }
        private String _exportgrade;
        [XmlElement]
        [Field(Name = "ExportGrade")]
        public virtual String ExportGrade
        {
            get
            {
                return _exportgrade;
            }
            set
            {
                if (_exportgrade == value) { return; }
                _exportgrade = value;
                this.ValidateProperty(LOGSTOCK.EXPORTGRADE, _exportgrade);
                this.NotifyPropertyChanged(LOGSTOCK.EXPORTGRADE);
            }
        }
        private float _smallenddiameter = 0.0f;
        [XmlElement]
        [Field(Name = "SmallEndDiameter")]
        public virtual float SmallEndDiameter
        {
            get
            {
                return _smallenddiameter;
            }
            set
            {
                if (Math.Abs(_smallenddiameter - value) < float.Epsilon) { return; }
                _smallenddiameter = value;
                this.ValidateProperty(LOGSTOCK.SMALLENDDIAMETER, _smallenddiameter);
                this.NotifyPropertyChanged(LOGSTOCK.SMALLENDDIAMETER);
            }
        }
        private float _largeenddiameter = 0.0f;
        [XmlElement]
        [Field(Name = "LargeEndDiameter")]
        public virtual float LargeEndDiameter
        {
            get
            {
                return _largeenddiameter;
            }
            set
            {
                if (Math.Abs(_largeenddiameter - value) < float.Epsilon) { return; }
                _largeenddiameter = value;
                this.ValidateProperty(LOGSTOCK.LARGEENDDIAMETER, _largeenddiameter);
                this.NotifyPropertyChanged(LOGSTOCK.LARGEENDDIAMETER);
            }
        }
        private float _grossboardfoot = 0.0f;
        [XmlElement]
        [Field(Name = "GrossBoardFoot")]
        public virtual float GrossBoardFoot
        {
            get
            {
                return _grossboardfoot;
            }
            set
            {
                if (Math.Abs(_grossboardfoot - value) < float.Epsilon) { return; }
                _grossboardfoot = value;
                this.ValidateProperty(LOGSTOCK.GROSSBOARDFOOT, _grossboardfoot);
                this.NotifyPropertyChanged(LOGSTOCK.GROSSBOARDFOOT);
            }
        }
        private float _netboardfoot = 0.0f;
        [XmlElement]
        [Field(Name = "NetBoardFoot")]
        public virtual float NetBoardFoot
        {
            get
            {
                return _netboardfoot;
            }
            set
            {
                if (Math.Abs(_netboardfoot - value) < float.Epsilon) { return; }
                _netboardfoot = value;
                this.ValidateProperty(LOGSTOCK.NETBOARDFOOT, _netboardfoot);
                this.NotifyPropertyChanged(LOGSTOCK.NETBOARDFOOT);
            }
        }
        private float _grosscubicfoot = 0.0f;
        [XmlElement]
        [Field(Name = "GrossCubicFoot")]
        public virtual float GrossCubicFoot
        {
            get
            {
                return _grosscubicfoot;
            }
            set
            {
                if (Math.Abs(_grosscubicfoot - value) < float.Epsilon) { return; }
                _grosscubicfoot = value;
                this.ValidateProperty(LOGSTOCK.GROSSCUBICFOOT, _grosscubicfoot);
                this.NotifyPropertyChanged(LOGSTOCK.GROSSCUBICFOOT);
            }
        }
        private float _netcubicfoot = 0.0f;
        [XmlElement]
        [Field(Name = "NetCubicFoot")]
        public virtual float NetCubicFoot
        {
            get
            {
                return _netcubicfoot;
            }
            set
            {
                if (Math.Abs(_netcubicfoot - value) < float.Epsilon) { return; }
                _netcubicfoot = value;
                this.ValidateProperty(LOGSTOCK.NETCUBICFOOT, _netcubicfoot);
                this.NotifyPropertyChanged(LOGSTOCK.NETCUBICFOOT);
            }
        }
        private float _boardfootremoved = 0.0f;
        [XmlElement]
        [Field(Name = "BoardFootRemoved")]
        public virtual float BoardFootRemoved
        {
            get
            {
                return _boardfootremoved;
            }
            set
            {
                if (Math.Abs(_boardfootremoved - value) < float.Epsilon) { return; }
                _boardfootremoved = value;
                this.ValidateProperty(LOGSTOCK.BOARDFOOTREMOVED, _boardfootremoved);
                this.NotifyPropertyChanged(LOGSTOCK.BOARDFOOTREMOVED);
            }
        }
        private float _cubicfootremoved = 0.0f;
        [XmlElement]
        [Field(Name = "CubicFootRemoved")]
        public virtual float CubicFootRemoved
        {
            get
            {
                return _cubicfootremoved;
            }
            set
            {
                if (Math.Abs(_cubicfootremoved - value) < float.Epsilon) { return; }
                _cubicfootremoved = value;
                this.ValidateProperty(LOGSTOCK.CUBICFOOTREMOVED, _cubicfootremoved);
                this.NotifyPropertyChanged(LOGSTOCK.CUBICFOOTREMOVED);
            }
        }
        private float _dibclass = 0.0f;
        [XmlElement]
        [Field(Name = "DIBClass")]
        public virtual float DIBClass
        {
            get
            {
                return _dibclass;
            }
            set
            {
                if (Math.Abs(_dibclass - value) < float.Epsilon) { return; }
                _dibclass = value;
                this.ValidateProperty(LOGSTOCK.DIBCLASS, _dibclass);
                this.NotifyPropertyChanged(LOGSTOCK.DIBCLASS);
            }
        }
        private float _barkthickness = 0.0f;
        [XmlElement]
        [Field(Name = "BarkThickness")]
        public virtual float BarkThickness
        {
            get
            {
                return _barkthickness;
            }
            set
            {
                if (Math.Abs(_barkthickness - value) < float.Epsilon) { return; }
                _barkthickness = value;
                this.ValidateProperty(LOGSTOCK.BARKTHICKNESS, _barkthickness);
                this.NotifyPropertyChanged(LOGSTOCK.BARKTHICKNESS);
            }
        }
        private float _boardutil = 0.0f;
        [XmlElement]
        [Field(Name = "BoardUtil")]
        public virtual float BoardUtil
        {
            get
            {
                return _boardutil;
            }
            set
            {
                if (Math.Abs(_boardutil - value) < float.Epsilon) { return; }
                _boardutil = value;
                this.ValidateProperty(LOGSTOCK.BOARDUTIL, _boardutil);
                this.NotifyPropertyChanged(LOGSTOCK.BOARDUTIL);
            }
        }
        private float _cubicutil = 0.0f;
        [XmlElement]
        [Field(Name = "CubicUtil")]
        public virtual float CubicUtil
        {
            get
            {
                return _cubicutil;
            }
            set
            {
                if (Math.Abs(_cubicutil - value) < float.Epsilon) { return; }
                _cubicutil = value;
                this.ValidateProperty(LOGSTOCK.CUBICUTIL, _cubicutil);
                this.NotifyPropertyChanged(LOGSTOCK.CUBICUTIL);
            }
        }

        [XmlIgnore]
        [CreatedByField()]
        public string CreatedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "CreatedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [XmlIgnore]
        [Field(Name = "ModifiedDate",
        PersistanceFlags = PersistanceFlags.Never)]
        public string ModifiedDate { get; set; }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("LogNumber", this.LogNumber) && isValid;
            isValid = ValidateProperty("Grade", this.Grade) && isValid;
            isValid = ValidateProperty("SeenDefect", this.SeenDefect) && isValid;
            isValid = ValidateProperty("PercentRecoverable", this.PercentRecoverable) && isValid;
            isValid = ValidateProperty("Length", this.Length) && isValid;
            isValid = ValidateProperty("ExportGrade", this.ExportGrade) && isValid;
            isValid = ValidateProperty("SmallEndDiameter", this.SmallEndDiameter) && isValid;
            isValid = ValidateProperty("LargeEndDiameter", this.LargeEndDiameter) && isValid;
            isValid = ValidateProperty("GrossBoardFoot", this.GrossBoardFoot) && isValid;
            isValid = ValidateProperty("NetBoardFoot", this.NetBoardFoot) && isValid;
            isValid = ValidateProperty("GrossCubicFoot", this.GrossCubicFoot) && isValid;
            isValid = ValidateProperty("NetCubicFoot", this.NetCubicFoot) && isValid;
            isValid = ValidateProperty("BoardFootRemoved", this.BoardFootRemoved) && isValid;
            isValid = ValidateProperty("CubicFootRemoved", this.CubicFootRemoved) && isValid;
            isValid = ValidateProperty("DIBClass", this.DIBClass) && isValid;
            isValid = ValidateProperty("BarkThickness", this.BarkThickness) && isValid;
            isValid = ValidateProperty("BoardUtil", this.BoardUtil) && isValid;
            isValid = ValidateProperty("CubicUtil", this.CubicUtil) && isValid;
            isValid = ValidateProperty("Tree_CN", this.Tree_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogStockDO);
        }

        public void SetValues(LogStockDO obj)
        {
            if (obj == null) { return; }
            LogNumber = obj.LogNumber;
            Grade = obj.Grade;
            SeenDefect = obj.SeenDefect;
            PercentRecoverable = obj.PercentRecoverable;
            Length = obj.Length;
            ExportGrade = obj.ExportGrade;
            SmallEndDiameter = obj.SmallEndDiameter;
            LargeEndDiameter = obj.LargeEndDiameter;
            GrossBoardFoot = obj.GrossBoardFoot;
            NetBoardFoot = obj.NetBoardFoot;
            GrossCubicFoot = obj.GrossCubicFoot;
            NetCubicFoot = obj.NetCubicFoot;
            BoardFootRemoved = obj.BoardFootRemoved;
            CubicFootRemoved = obj.CubicFootRemoved;
            DIBClass = obj.DIBClass;
            BarkThickness = obj.BarkThickness;
            BoardUtil = obj.BoardUtil;
            CubicUtil = obj.CubicUtil;
        }
    }
    [Table("SampleGroupStats")]
    public partial class SampleGroupStatsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static SampleGroupStatsDO()
        {
            _validator = new RowValidator();
        }

        public SampleGroupStatsDO() { }

        public SampleGroupStatsDO(SampleGroupStatsDO obj) : this()
        {
            SetValues(obj);
        }

        public SampleGroupStatsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "SampleGroupStats_CN")]
        public Int64? SampleGroupStats_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratumstats_cn;
        [XmlIgnore]
        [Field(Name = "StratumStats_CN")]
        public virtual long? StratumStats_CN
        {
            get
            {

                if (_stratumstats != null)
                {
                    return _stratumstats.StratumStats_CN;
                }
                return _stratumstats_cn;
            }
            set
            {
                if (_stratumstats_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratumstats = null; }
                _stratumstats_cn = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.STRATUMSTATS_CN, _stratumstats_cn);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.STRATUMSTATS_CN);
            }
        }
        public virtual StratumStatsDO GetStratumStats()
        {
            if (DAL == null) { return null; }
            if (this.StratumStats_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumStatsDO>(this.StratumStats_CN);
            //return DAL.ReadSingleRow<StratumStatsDO>(STRATUMSTATS._NAME, this.StratumStats_CN);
        }

        private StratumStatsDO _stratumstats = null;
        [XmlIgnore]
        public StratumStatsDO StratumStats
        {
            get
            {
                if (_stratumstats == null)
                {
                    _stratumstats = GetStratumStats();
                }
                return _stratumstats;
            }
            set
            {
                if (_stratumstats == value) { return; }
                _stratumstats = value;
                StratumStats_CN = (value != null) ? value.StratumStats_CN : null;
            }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CODE, _code);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CODE);
            }
        }
        private Int64 _sgset;
        [XmlElement]
        [Field(Name = "SgSet")]
        public virtual Int64 SgSet
        {
            get
            {
                return _sgset;
            }
            set
            {
                if (_sgset == value) { return; }
                _sgset = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SGSET, _sgset);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SGSET);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.DESCRIPTION, _description);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.DESCRIPTION);
            }
        }
        private String _cutleave;
        [XmlElement]
        [Field(Name = "CutLeave")]
        public virtual String CutLeave
        {
            get
            {
                return _cutleave;
            }
            set
            {
                if (_cutleave == value) { return; }
                _cutleave = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CUTLEAVE, _cutleave);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CUTLEAVE);
            }
        }
        private String _uom;
        [XmlElement]
        [Field(Name = "UOM")]
        public virtual String UOM
        {
            get
            {
                return _uom;
            }
            set
            {
                if (_uom == value) { return; }
                _uom = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.UOM, _uom);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.UOM);
            }
        }
        private String _primaryproduct;
        [XmlElement]
        [Field(Name = "PrimaryProduct")]
        public virtual String PrimaryProduct
        {
            get
            {
                return _primaryproduct;
            }
            set
            {
                if (_primaryproduct == value) { return; }
                _primaryproduct = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.PRIMARYPRODUCT, _primaryproduct);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.PRIMARYPRODUCT);
            }
        }
        private String _secondaryproduct;
        [XmlElement]
        [Field(Name = "SecondaryProduct")]
        public virtual String SecondaryProduct
        {
            get
            {
                return _secondaryproduct;
            }
            set
            {
                if (_secondaryproduct == value) { return; }
                _secondaryproduct = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SECONDARYPRODUCT, _secondaryproduct);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SECONDARYPRODUCT);
            }
        }
        private String _defaultlivedead;
        [XmlElement]
        [Field(Name = "DefaultLiveDead")]
        public virtual String DefaultLiveDead
        {
            get
            {
                return _defaultlivedead;
            }
            set
            {
                if (_defaultlivedead == value) { return; }
                _defaultlivedead = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.DEFAULTLIVEDEAD, _defaultlivedead);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.DEFAULTLIVEDEAD);
            }
        }
        private float _sgerror = 0.0f;
        [XmlElement]
        [Field(Name = "SgError")]
        public virtual float SgError
        {
            get
            {
                return _sgerror;
            }
            set
            {
                if (Math.Abs(_sgerror - value) < float.Epsilon) { return; }
                _sgerror = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SGERROR, _sgerror);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SGERROR);
            }
        }
        private Int64 _samplesize1;
        [XmlElement]
        [Field(Name = "SampleSize1")]
        public virtual Int64 SampleSize1
        {
            get
            {
                return _samplesize1;
            }
            set
            {
                if (_samplesize1 == value) { return; }
                _samplesize1 = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SAMPLESIZE1, _samplesize1);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SAMPLESIZE1);
            }
        }
        private Int64 _samplesize2;
        [XmlElement]
        [Field(Name = "SampleSize2")]
        public virtual Int64 SampleSize2
        {
            get
            {
                return _samplesize2;
            }
            set
            {
                if (_samplesize2 == value) { return; }
                _samplesize2 = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SAMPLESIZE2, _samplesize2);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SAMPLESIZE2);
            }
        }
        private float _cv1 = 0.0f;
        [XmlElement]
        [Field(Name = "CV1")]
        public virtual float CV1
        {
            get
            {
                return _cv1;
            }
            set
            {
                if (Math.Abs(_cv1 - value) < float.Epsilon) { return; }
                _cv1 = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CV1, _cv1);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CV1);
            }
        }
        private float _cv2 = 0.0f;
        [XmlElement]
        [Field(Name = "CV2")]
        public virtual float CV2
        {
            get
            {
                return _cv2;
            }
            set
            {
                if (Math.Abs(_cv2 - value) < float.Epsilon) { return; }
                _cv2 = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CV2, _cv2);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CV2);
            }
        }
        private float _treesperacre = 0.0f;
        [XmlElement]
        [Field(Name = "TreesPerAcre")]
        public virtual float TreesPerAcre
        {
            get
            {
                return _treesperacre;
            }
            set
            {
                if (Math.Abs(_treesperacre - value) < float.Epsilon) { return; }
                _treesperacre = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.TREESPERACRE, _treesperacre);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.TREESPERACRE);
            }
        }
        private float _volumeperacre = 0.0f;
        [XmlElement]
        [Field(Name = "VolumePerAcre")]
        public virtual float VolumePerAcre
        {
            get
            {
                return _volumeperacre;
            }
            set
            {
                if (Math.Abs(_volumeperacre - value) < float.Epsilon) { return; }
                _volumeperacre = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.VOLUMEPERACRE, _volumeperacre);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.VOLUMEPERACRE);
            }
        }
        private float _treesperplot = 0.0f;
        [XmlElement]
        [Field(Name = "TreesPerPlot")]
        public virtual float TreesPerPlot
        {
            get
            {
                return _treesperplot;
            }
            set
            {
                if (Math.Abs(_treesperplot - value) < float.Epsilon) { return; }
                _treesperplot = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.TREESPERPLOT, _treesperplot);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.TREESPERPLOT);
            }
        }
        private float _averageheight = 0.0f;
        [XmlElement]
        [Field(Name = "AverageHeight")]
        public virtual float AverageHeight
        {
            get
            {
                return _averageheight;
            }
            set
            {
                if (Math.Abs(_averageheight - value) < float.Epsilon) { return; }
                _averageheight = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.AVERAGEHEIGHT, _averageheight);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.AVERAGEHEIGHT);
            }
        }
        private Int64 _samplingfrequency;
        [XmlElement]
        [Field(Name = "SamplingFrequency")]
        public virtual Int64 SamplingFrequency
        {
            get
            {
                return _samplingfrequency;
            }
            set
            {
                if (_samplingfrequency == value) { return; }
                _samplingfrequency = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.SAMPLINGFREQUENCY, _samplingfrequency);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.SAMPLINGFREQUENCY);
            }
        }
        private Int64 _insurancefrequency;
        [XmlElement]
        [Field(Name = "InsuranceFrequency")]
        public virtual Int64 InsuranceFrequency
        {
            get
            {
                return _insurancefrequency;
            }
            set
            {
                if (_insurancefrequency == value) { return; }
                _insurancefrequency = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.INSURANCEFREQUENCY, _insurancefrequency);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.INSURANCEFREQUENCY);
            }
        }
        private Int64 _kz;
        [XmlElement]
        [Field(Name = "KZ")]
        public virtual Int64 KZ
        {
            get
            {
                return _kz;
            }
            set
            {
                if (_kz == value) { return; }
                _kz = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.KZ, _kz);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.KZ);
            }
        }
        private float _bigbaf = 0.0f;
        [XmlElement]
        [Field(Name = "BigBAF")]
        public virtual float BigBAF
        {
            get
            {
                return _bigbaf;
            }
            set
            {
                if (Math.Abs(_bigbaf - value) < float.Epsilon) { return; }
                _bigbaf = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.BIGBAF, _bigbaf);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.BIGBAF);
            }
        }
        private Int64 _bigfix;
        [XmlElement]
        [Field(Name = "BigFIX")]
        public virtual Int64 BigFIX
        {
            get
            {
                return _bigfix;
            }
            set
            {
                if (_bigfix == value) { return; }
                _bigfix = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.BIGFIX, _bigfix);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.BIGFIX);
            }
        }
        private float _mindbh = 0.0f;
        [XmlElement]
        [Field(Name = "MinDbh")]
        public virtual float MinDbh
        {
            get
            {
                return _mindbh;
            }
            set
            {
                if (Math.Abs(_mindbh - value) < float.Epsilon) { return; }
                _mindbh = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.MINDBH, _mindbh);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.MINDBH);
            }
        }
        private float _maxdbh = 0.0f;
        [XmlElement]
        [Field(Name = "MaxDbh")]
        public virtual float MaxDbh
        {
            get
            {
                return _maxdbh;
            }
            set
            {
                if (Math.Abs(_maxdbh - value) < float.Epsilon) { return; }
                _maxdbh = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.MAXDBH, _maxdbh);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.MAXDBH);
            }
        }
        private Int64 _cv_def;
        [XmlElement]
        [Field(Name = "CV_Def")]
        public virtual Int64 CV_Def
        {
            get
            {
                return _cv_def;
            }
            set
            {
                if (_cv_def == value) { return; }
                _cv_def = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CV_DEF, _cv_def);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CV_DEF);
            }
        }
        private Int64 _cv2_def;
        [XmlElement]
        [Field(Name = "CV2_Def")]
        public virtual Int64 CV2_Def
        {
            get
            {
                return _cv2_def;
            }
            set
            {
                if (_cv2_def == value) { return; }
                _cv2_def = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.CV2_DEF, _cv2_def);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.CV2_DEF);
            }
        }
        private Int64 _tpa_def;
        [XmlElement]
        [Field(Name = "TPA_Def")]
        public virtual Int64 TPA_Def
        {
            get
            {
                return _tpa_def;
            }
            set
            {
                if (_tpa_def == value) { return; }
                _tpa_def = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.TPA_DEF, _tpa_def);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.TPA_DEF);
            }
        }
        private Int64 _vpa_def;
        [XmlElement]
        [Field(Name = "VPA_Def")]
        public virtual Int64 VPA_Def
        {
            get
            {
                return _vpa_def;
            }
            set
            {
                if (_vpa_def == value) { return; }
                _vpa_def = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.VPA_DEF, _vpa_def);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.VPA_DEF);
            }
        }
        private Int64 _reconplots;
        [XmlElement]
        [Field(Name = "ReconPlots")]
        public virtual Int64 ReconPlots
        {
            get
            {
                return _reconplots;
            }
            set
            {
                if (_reconplots == value) { return; }
                _reconplots = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.RECONPLOTS, _reconplots);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.RECONPLOTS);
            }
        }
        private Int64 _recontrees;
        [XmlElement]
        [Field(Name = "ReconTrees")]
        public virtual Int64 ReconTrees
        {
            get
            {
                return _recontrees;
            }
            set
            {
                if (_recontrees == value) { return; }
                _recontrees = value;
                this.ValidateProperty(SAMPLEGROUPSTATS.RECONTREES, _recontrees);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATS.RECONTREES);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("SgSet", this.SgSet) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("CutLeave", this.CutLeave) && isValid;
            isValid = ValidateProperty("UOM", this.UOM) && isValid;
            isValid = ValidateProperty("PrimaryProduct", this.PrimaryProduct) && isValid;
            isValid = ValidateProperty("SecondaryProduct", this.SecondaryProduct) && isValid;
            isValid = ValidateProperty("DefaultLiveDead", this.DefaultLiveDead) && isValid;
            isValid = ValidateProperty("SgError", this.SgError) && isValid;
            isValid = ValidateProperty("SampleSize1", this.SampleSize1) && isValid;
            isValid = ValidateProperty("SampleSize2", this.SampleSize2) && isValid;
            isValid = ValidateProperty("CV1", this.CV1) && isValid;
            isValid = ValidateProperty("CV2", this.CV2) && isValid;
            isValid = ValidateProperty("TreesPerAcre", this.TreesPerAcre) && isValid;
            isValid = ValidateProperty("VolumePerAcre", this.VolumePerAcre) && isValid;
            isValid = ValidateProperty("TreesPerPlot", this.TreesPerPlot) && isValid;
            isValid = ValidateProperty("AverageHeight", this.AverageHeight) && isValid;
            isValid = ValidateProperty("SamplingFrequency", this.SamplingFrequency) && isValid;
            isValid = ValidateProperty("InsuranceFrequency", this.InsuranceFrequency) && isValid;
            isValid = ValidateProperty("KZ", this.KZ) && isValid;
            isValid = ValidateProperty("BigBAF", this.BigBAF) && isValid;
            isValid = ValidateProperty("BigFIX", this.BigFIX) && isValid;
            isValid = ValidateProperty("MinDbh", this.MinDbh) && isValid;
            isValid = ValidateProperty("MaxDbh", this.MaxDbh) && isValid;
            isValid = ValidateProperty("CV_Def", this.CV_Def) && isValid;
            isValid = ValidateProperty("CV2_Def", this.CV2_Def) && isValid;
            isValid = ValidateProperty("TPA_Def", this.TPA_Def) && isValid;
            isValid = ValidateProperty("VPA_Def", this.VPA_Def) && isValid;
            isValid = ValidateProperty("ReconPlots", this.ReconPlots) && isValid;
            isValid = ValidateProperty("ReconTrees", this.ReconTrees) && isValid;
            isValid = ValidateProperty("StratumStats_CN", this.StratumStats_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as SampleGroupStatsDO);
        }

        public void SetValues(SampleGroupStatsDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            SgSet = obj.SgSet;
            Description = obj.Description;
            CutLeave = obj.CutLeave;
            UOM = obj.UOM;
            PrimaryProduct = obj.PrimaryProduct;
            SecondaryProduct = obj.SecondaryProduct;
            DefaultLiveDead = obj.DefaultLiveDead;
            SgError = obj.SgError;
            SampleSize1 = obj.SampleSize1;
            SampleSize2 = obj.SampleSize2;
            CV1 = obj.CV1;
            CV2 = obj.CV2;
            TreesPerAcre = obj.TreesPerAcre;
            VolumePerAcre = obj.VolumePerAcre;
            TreesPerPlot = obj.TreesPerPlot;
            AverageHeight = obj.AverageHeight;
            SamplingFrequency = obj.SamplingFrequency;
            InsuranceFrequency = obj.InsuranceFrequency;
            KZ = obj.KZ;
            BigBAF = obj.BigBAF;
            BigFIX = obj.BigFIX;
            MinDbh = obj.MinDbh;
            MaxDbh = obj.MaxDbh;
            CV_Def = obj.CV_Def;
            CV2_Def = obj.CV2_Def;
            TPA_Def = obj.TPA_Def;
            VPA_Def = obj.VPA_Def;
            ReconPlots = obj.ReconPlots;
            ReconTrees = obj.ReconTrees;
        }
    }
    [Table("SampleGroupStatsTreeDefaultValue")]
    public partial class SampleGroupStatsTreeDefaultValueDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static SampleGroupStatsTreeDefaultValueDO()
        {
            _validator = new RowValidator();
        }

        public SampleGroupStatsTreeDefaultValueDO() { }

        public SampleGroupStatsTreeDefaultValueDO(SampleGroupStatsTreeDefaultValueDO obj) : this()
        {
            SetValues(obj);
        }

        public SampleGroupStatsTreeDefaultValueDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(SAMPLEGROUPSTATSTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATSTREEDEFAULTVALUE.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }
        private long? _samplegroupstats_cn;
        [XmlIgnore]
        [Field(Name = "SampleGroupStats_CN")]
        public virtual long? SampleGroupStats_CN
        {
            get
            {

                if (_samplegroupstats != null)
                {
                    return _samplegroupstats.SampleGroupStats_CN;
                }
                return _samplegroupstats_cn;
            }
            set
            {
                if (_samplegroupstats_cn == value) { return; }
                if (value == null || value.Value == 0) { _samplegroupstats = null; }
                _samplegroupstats_cn = value;
                this.ValidateProperty(SAMPLEGROUPSTATSTREEDEFAULTVALUE.SAMPLEGROUPSTATS_CN, _samplegroupstats_cn);
                this.NotifyPropertyChanged(SAMPLEGROUPSTATSTREEDEFAULTVALUE.SAMPLEGROUPSTATS_CN);
            }
        }
        public virtual SampleGroupStatsDO GetSampleGroupStats()
        {
            if (DAL == null) { return null; }
            if (this.SampleGroupStats_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<SampleGroupStatsDO>(this.SampleGroupStats_CN);
            //return DAL.ReadSingleRow<SampleGroupStatsDO>(SAMPLEGROUPSTATS._NAME, this.SampleGroupStats_CN);
        }

        private SampleGroupStatsDO _samplegroupstats = null;
        [XmlIgnore]
        public SampleGroupStatsDO SampleGroupStats
        {
            get
            {
                if (_samplegroupstats == null)
                {
                    _samplegroupstats = GetSampleGroupStats();
                }
                return _samplegroupstats;
            }
            set
            {
                if (_samplegroupstats == value) { return; }
                _samplegroupstats = value;
                SampleGroupStats_CN = (value != null) ? value.SampleGroupStats_CN : null;
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            isValid = ValidateProperty("SampleGroupStats_CN", this.SampleGroupStats_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as SampleGroupStatsTreeDefaultValueDO);
        }

        public void SetValues(SampleGroupStatsTreeDefaultValueDO obj)
        {
            if (obj == null) { return; }
        }
    }
    [Table("StratumStats")]
    public partial class StratumStatsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static StratumStatsDO()
        {
            _validator = new RowValidator();
        }

        public StratumStatsDO() { }

        public StratumStatsDO(StratumStatsDO obj) : this()
        {
            SetValues(obj);
        }

        public StratumStatsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "StratumStats_CN")]
        public Int64? StratumStats_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(STRATUMSTATS.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(STRATUMSTATS.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(STRATUMSTATS.CODE, _code);
                this.NotifyPropertyChanged(STRATUMSTATS.CODE);
            }
        }
        private String _description;
        [XmlElement]
        [Field(Name = "Description")]
        public virtual String Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description == value) { return; }
                _description = value;
                this.ValidateProperty(STRATUMSTATS.DESCRIPTION, _description);
                this.NotifyPropertyChanged(STRATUMSTATS.DESCRIPTION);
            }
        }
        private String _method;
        [XmlElement]
        [Field(Name = "Method")]
        public virtual String Method
        {
            get
            {
                return _method;
            }
            set
            {
                if (_method == value) { return; }
                _method = value;
                this.ValidateProperty(STRATUMSTATS.METHOD, _method);
                this.NotifyPropertyChanged(STRATUMSTATS.METHOD);
            }
        }
        private Int64 _sgset;
        [XmlElement]
        [Field(Name = "SgSet")]
        public virtual Int64 SgSet
        {
            get
            {
                return _sgset;
            }
            set
            {
                if (_sgset == value) { return; }
                _sgset = value;
                this.ValidateProperty(STRATUMSTATS.SGSET, _sgset);
                this.NotifyPropertyChanged(STRATUMSTATS.SGSET);
            }
        }
        private String _sgsetdescription;
        [XmlElement]
        [Field(Name = "SgSetDescription")]
        public virtual String SgSetDescription
        {
            get
            {
                return _sgsetdescription;
            }
            set
            {
                if (_sgsetdescription == value) { return; }
                _sgsetdescription = value;
                this.ValidateProperty(STRATUMSTATS.SGSETDESCRIPTION, _sgsetdescription);
                this.NotifyPropertyChanged(STRATUMSTATS.SGSETDESCRIPTION);
            }
        }
        private float _basalareafactor = 0.0f;
        [XmlElement]
        [Field(Name = "BasalAreaFactor")]
        public virtual float BasalAreaFactor
        {
            get
            {
                return _basalareafactor;
            }
            set
            {
                if (Math.Abs(_basalareafactor - value) < float.Epsilon) { return; }
                _basalareafactor = value;
                this.ValidateProperty(STRATUMSTATS.BASALAREAFACTOR, _basalareafactor);
                this.NotifyPropertyChanged(STRATUMSTATS.BASALAREAFACTOR);
            }
        }
        private float _fixedplotsize = 0.0f;
        [XmlElement]
        [Field(Name = "FixedPlotSize")]
        public virtual float FixedPlotSize
        {
            get
            {
                return _fixedplotsize;
            }
            set
            {
                if (Math.Abs(_fixedplotsize - value) < float.Epsilon) { return; }
                _fixedplotsize = value;
                this.ValidateProperty(STRATUMSTATS.FIXEDPLOTSIZE, _fixedplotsize);
                this.NotifyPropertyChanged(STRATUMSTATS.FIXEDPLOTSIZE);
            }
        }
        private float _strerror = 0.0f;
        [XmlElement]
        [Field(Name = "StrError")]
        public virtual float StrError
        {
            get
            {
                return _strerror;
            }
            set
            {
                if (Math.Abs(_strerror - value) < float.Epsilon) { return; }
                _strerror = value;
                this.ValidateProperty(STRATUMSTATS.STRERROR, _strerror);
                this.NotifyPropertyChanged(STRATUMSTATS.STRERROR);
            }
        }
        private Int64 _samplesize1;
        [XmlElement]
        [Field(Name = "SampleSize1")]
        public virtual Int64 SampleSize1
        {
            get
            {
                return _samplesize1;
            }
            set
            {
                if (_samplesize1 == value) { return; }
                _samplesize1 = value;
                this.ValidateProperty(STRATUMSTATS.SAMPLESIZE1, _samplesize1);
                this.NotifyPropertyChanged(STRATUMSTATS.SAMPLESIZE1);
            }
        }
        private Int64 _samplesize2;
        [XmlElement]
        [Field(Name = "SampleSize2")]
        public virtual Int64 SampleSize2
        {
            get
            {
                return _samplesize2;
            }
            set
            {
                if (_samplesize2 == value) { return; }
                _samplesize2 = value;
                this.ValidateProperty(STRATUMSTATS.SAMPLESIZE2, _samplesize2);
                this.NotifyPropertyChanged(STRATUMSTATS.SAMPLESIZE2);
            }
        }
        private float _weightedcv1 = 0.0f;
        [XmlElement]
        [Field(Name = "WeightedCV1")]
        public virtual float WeightedCV1
        {
            get
            {
                return _weightedcv1;
            }
            set
            {
                if (Math.Abs(_weightedcv1 - value) < float.Epsilon) { return; }
                _weightedcv1 = value;
                this.ValidateProperty(STRATUMSTATS.WEIGHTEDCV1, _weightedcv1);
                this.NotifyPropertyChanged(STRATUMSTATS.WEIGHTEDCV1);
            }
        }
        private float _weightedcv2 = 0.0f;
        [XmlElement]
        [Field(Name = "WeightedCV2")]
        public virtual float WeightedCV2
        {
            get
            {
                return _weightedcv2;
            }
            set
            {
                if (Math.Abs(_weightedcv2 - value) < float.Epsilon) { return; }
                _weightedcv2 = value;
                this.ValidateProperty(STRATUMSTATS.WEIGHTEDCV2, _weightedcv2);
                this.NotifyPropertyChanged(STRATUMSTATS.WEIGHTEDCV2);
            }
        }
        private float _treesperacre = 0.0f;
        [XmlElement]
        [Field(Name = "TreesPerAcre")]
        public virtual float TreesPerAcre
        {
            get
            {
                return _treesperacre;
            }
            set
            {
                if (Math.Abs(_treesperacre - value) < float.Epsilon) { return; }
                _treesperacre = value;
                this.ValidateProperty(STRATUMSTATS.TREESPERACRE, _treesperacre);
                this.NotifyPropertyChanged(STRATUMSTATS.TREESPERACRE);
            }
        }
        private float _volumeperacre = 0.0f;
        [XmlElement]
        [Field(Name = "VolumePerAcre")]
        public virtual float VolumePerAcre
        {
            get
            {
                return _volumeperacre;
            }
            set
            {
                if (Math.Abs(_volumeperacre - value) < float.Epsilon) { return; }
                _volumeperacre = value;
                this.ValidateProperty(STRATUMSTATS.VOLUMEPERACRE, _volumeperacre);
                this.NotifyPropertyChanged(STRATUMSTATS.VOLUMEPERACRE);
            }
        }
        private float _totalvolume = 0.0f;
        [XmlElement]
        [Field(Name = "TotalVolume")]
        public virtual float TotalVolume
        {
            get
            {
                return _totalvolume;
            }
            set
            {
                if (Math.Abs(_totalvolume - value) < float.Epsilon) { return; }
                _totalvolume = value;
                this.ValidateProperty(STRATUMSTATS.TOTALVOLUME, _totalvolume);
                this.NotifyPropertyChanged(STRATUMSTATS.TOTALVOLUME);
            }
        }
        private float _totalacres = 0.0f;
        [XmlElement]
        [Field(Name = "TotalAcres")]
        public virtual float TotalAcres
        {
            get
            {
                return _totalacres;
            }
            set
            {
                if (Math.Abs(_totalacres - value) < float.Epsilon) { return; }
                _totalacres = value;
                this.ValidateProperty(STRATUMSTATS.TOTALACRES, _totalacres);
                this.NotifyPropertyChanged(STRATUMSTATS.TOTALACRES);
            }
        }
        private Int64 _plotspacing;
        [XmlElement]
        [Field(Name = "PlotSpacing")]
        public virtual Int64 PlotSpacing
        {
            get
            {
                return _plotspacing;
            }
            set
            {
                if (_plotspacing == value) { return; }
                _plotspacing = value;
                this.ValidateProperty(STRATUMSTATS.PLOTSPACING, _plotspacing);
                this.NotifyPropertyChanged(STRATUMSTATS.PLOTSPACING);
            }
        }
        private Int64 _used;
        [XmlElement]
        [Field(Name = "Used")]
        public virtual Int64 Used
        {
            get
            {
                return _used;
            }
            set
            {
                if (_used == value) { return; }
                _used = value;
                this.ValidateProperty(STRATUMSTATS.USED, _used);
                this.NotifyPropertyChanged(STRATUMSTATS.USED);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("Description", this.Description) && isValid;
            isValid = ValidateProperty("Method", this.Method) && isValid;
            isValid = ValidateProperty("SgSet", this.SgSet) && isValid;
            isValid = ValidateProperty("SgSetDescription", this.SgSetDescription) && isValid;
            isValid = ValidateProperty("BasalAreaFactor", this.BasalAreaFactor) && isValid;
            isValid = ValidateProperty("FixedPlotSize", this.FixedPlotSize) && isValid;
            isValid = ValidateProperty("StrError", this.StrError) && isValid;
            isValid = ValidateProperty("SampleSize1", this.SampleSize1) && isValid;
            isValid = ValidateProperty("SampleSize2", this.SampleSize2) && isValid;
            isValid = ValidateProperty("WeightedCV1", this.WeightedCV1) && isValid;
            isValid = ValidateProperty("WeightedCV2", this.WeightedCV2) && isValid;
            isValid = ValidateProperty("TreesPerAcre", this.TreesPerAcre) && isValid;
            isValid = ValidateProperty("VolumePerAcre", this.VolumePerAcre) && isValid;
            isValid = ValidateProperty("TotalVolume", this.TotalVolume) && isValid;
            isValid = ValidateProperty("TotalAcres", this.TotalAcres) && isValid;
            isValid = ValidateProperty("PlotSpacing", this.PlotSpacing) && isValid;
            isValid = ValidateProperty("Used", this.Used) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as StratumStatsDO);
        }

        public void SetValues(StratumStatsDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            Description = obj.Description;
            Method = obj.Method;
            SgSet = obj.SgSet;
            SgSetDescription = obj.SgSetDescription;
            BasalAreaFactor = obj.BasalAreaFactor;
            FixedPlotSize = obj.FixedPlotSize;
            StrError = obj.StrError;
            SampleSize1 = obj.SampleSize1;
            SampleSize2 = obj.SampleSize2;
            WeightedCV1 = obj.WeightedCV1;
            WeightedCV2 = obj.WeightedCV2;
            TreesPerAcre = obj.TreesPerAcre;
            VolumePerAcre = obj.VolumePerAcre;
            TotalVolume = obj.TotalVolume;
            TotalAcres = obj.TotalAcres;
            PlotSpacing = obj.PlotSpacing;
            Used = obj.Used;
        }
    }
    [Table("Regression")]
    public partial class RegressionDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static RegressionDO()
        {
            _validator = new RowValidator();
        }

        public RegressionDO() { }

        public RegressionDO(RegressionDO obj) : this()
        {
            SetValues(obj);
        }

        public RegressionDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Regression_CN")]
        public Int64? Regression_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _rvolume;
        [XmlElement]
        [Field(Name = "rVolume")]
        public virtual String rVolume
        {
            get
            {
                return _rvolume;
            }
            set
            {
                if (_rvolume == value) { return; }
                _rvolume = value;
                this.ValidateProperty(REGRESSION.RVOLUME, _rvolume);
                this.NotifyPropertyChanged(REGRESSION.RVOLUME);
            }
        }
        private String _rvoltype;
        [XmlElement]
        [Field(Name = "rVolType")]
        public virtual String rVolType
        {
            get
            {
                return _rvoltype;
            }
            set
            {
                if (_rvoltype == value) { return; }
                _rvoltype = value;
                this.ValidateProperty(REGRESSION.RVOLTYPE, _rvoltype);
                this.NotifyPropertyChanged(REGRESSION.RVOLTYPE);
            }
        }
        private String _rspeices;
        [XmlElement]
        [Field(Name = "rSpeices")]
        public virtual String rSpeices
        {
            get
            {
                return _rspeices;
            }
            set
            {
                if (_rspeices == value) { return; }
                _rspeices = value;
                this.ValidateProperty(REGRESSION.RSPEICES, _rspeices);
                this.NotifyPropertyChanged(REGRESSION.RSPEICES);
            }
        }
        private String _rproduct;
        [XmlElement]
        [Field(Name = "rProduct")]
        public virtual String rProduct
        {
            get
            {
                return _rproduct;
            }
            set
            {
                if (_rproduct == value) { return; }
                _rproduct = value;
                this.ValidateProperty(REGRESSION.RPRODUCT, _rproduct);
                this.NotifyPropertyChanged(REGRESSION.RPRODUCT);
            }
        }
        private String _rlivedead;
        [XmlElement]
        [Field(Name = "rLiveDead")]
        public virtual String rLiveDead
        {
            get
            {
                return _rlivedead;
            }
            set
            {
                if (_rlivedead == value) { return; }
                _rlivedead = value;
                this.ValidateProperty(REGRESSION.RLIVEDEAD, _rlivedead);
                this.NotifyPropertyChanged(REGRESSION.RLIVEDEAD);
            }
        }
        private float _coefficienta = 0.0f;
        [XmlElement]
        [Field(Name = "CoefficientA")]
        public virtual float CoefficientA
        {
            get
            {
                return _coefficienta;
            }
            set
            {
                if (Math.Abs(_coefficienta - value) < float.Epsilon) { return; }
                _coefficienta = value;
                this.ValidateProperty(REGRESSION.COEFFICIENTA, _coefficienta);
                this.NotifyPropertyChanged(REGRESSION.COEFFICIENTA);
            }
        }
        private float _coefficientb = 0.0f;
        [XmlElement]
        [Field(Name = "CoefficientB")]
        public virtual float CoefficientB
        {
            get
            {
                return _coefficientb;
            }
            set
            {
                if (Math.Abs(_coefficientb - value) < float.Epsilon) { return; }
                _coefficientb = value;
                this.ValidateProperty(REGRESSION.COEFFICIENTB, _coefficientb);
                this.NotifyPropertyChanged(REGRESSION.COEFFICIENTB);
            }
        }
        private float _coefficientc = 0.0f;
        [XmlElement]
        [Field(Name = "CoefficientC")]
        public virtual float CoefficientC
        {
            get
            {
                return _coefficientc;
            }
            set
            {
                if (Math.Abs(_coefficientc - value) < float.Epsilon) { return; }
                _coefficientc = value;
                this.ValidateProperty(REGRESSION.COEFFICIENTC, _coefficientc);
                this.NotifyPropertyChanged(REGRESSION.COEFFICIENTC);
            }
        }
        private Int64 _totaltrees;
        [XmlElement]
        [Field(Name = "TotalTrees")]
        public virtual Int64 TotalTrees
        {
            get
            {
                return _totaltrees;
            }
            set
            {
                if (_totaltrees == value) { return; }
                _totaltrees = value;
                this.ValidateProperty(REGRESSION.TOTALTREES, _totaltrees);
                this.NotifyPropertyChanged(REGRESSION.TOTALTREES);
            }
        }
        private float _meanse = 0.0f;
        [XmlElement]
        [Field(Name = "MeanSE")]
        public virtual float MeanSE
        {
            get
            {
                return _meanse;
            }
            set
            {
                if (Math.Abs(_meanse - value) < float.Epsilon) { return; }
                _meanse = value;
                this.ValidateProperty(REGRESSION.MEANSE, _meanse);
                this.NotifyPropertyChanged(REGRESSION.MEANSE);
            }
        }
        private float _rsquared = 0.0f;
        [XmlElement]
        [Field(Name = "Rsquared")]
        public virtual float Rsquared
        {
            get
            {
                return _rsquared;
            }
            set
            {
                if (Math.Abs(_rsquared - value) < float.Epsilon) { return; }
                _rsquared = value;
                this.ValidateProperty(REGRESSION.RSQUARED, _rsquared);
                this.NotifyPropertyChanged(REGRESSION.RSQUARED);
            }
        }
        private String _regressmodel;
        [XmlElement]
        [Field(Name = "RegressModel")]
        public virtual String RegressModel
        {
            get
            {
                return _regressmodel;
            }
            set
            {
                if (_regressmodel == value) { return; }
                _regressmodel = value;
                this.ValidateProperty(REGRESSION.REGRESSMODEL, _regressmodel);
                this.NotifyPropertyChanged(REGRESSION.REGRESSMODEL);
            }
        }
        private float _rmindbh = 0.0f;
        [XmlElement]
        [Field(Name = "rMinDbh")]
        public virtual float rMinDbh
        {
            get
            {
                return _rmindbh;
            }
            set
            {
                if (Math.Abs(_rmindbh - value) < float.Epsilon) { return; }
                _rmindbh = value;
                this.ValidateProperty(REGRESSION.RMINDBH, _rmindbh);
                this.NotifyPropertyChanged(REGRESSION.RMINDBH);
            }
        }
        private float _rmaxdbh = 0.0f;
        [XmlElement]
        [Field(Name = "rMaxDbh")]
        public virtual float rMaxDbh
        {
            get
            {
                return _rmaxdbh;
            }
            set
            {
                if (Math.Abs(_rmaxdbh - value) < float.Epsilon) { return; }
                _rmaxdbh = value;
                this.ValidateProperty(REGRESSION.RMAXDBH, _rmaxdbh);
                this.NotifyPropertyChanged(REGRESSION.RMAXDBH);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("rVolume", this.rVolume) && isValid;
            isValid = ValidateProperty("rVolType", this.rVolType) && isValid;
            isValid = ValidateProperty("rSpeices", this.rSpeices) && isValid;
            isValid = ValidateProperty("rProduct", this.rProduct) && isValid;
            isValid = ValidateProperty("rLiveDead", this.rLiveDead) && isValid;
            isValid = ValidateProperty("CoefficientA", this.CoefficientA) && isValid;
            isValid = ValidateProperty("CoefficientB", this.CoefficientB) && isValid;
            isValid = ValidateProperty("CoefficientC", this.CoefficientC) && isValid;
            isValid = ValidateProperty("TotalTrees", this.TotalTrees) && isValid;
            isValid = ValidateProperty("MeanSE", this.MeanSE) && isValid;
            isValid = ValidateProperty("Rsquared", this.Rsquared) && isValid;
            isValid = ValidateProperty("RegressModel", this.RegressModel) && isValid;
            isValid = ValidateProperty("rMinDbh", this.rMinDbh) && isValid;
            isValid = ValidateProperty("rMaxDbh", this.rMaxDbh) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as RegressionDO);
        }

        public void SetValues(RegressionDO obj)
        {
            if (obj == null) { return; }
            rVolume = obj.rVolume;
            rVolType = obj.rVolType;
            rSpeices = obj.rSpeices;
            rProduct = obj.rProduct;
            rLiveDead = obj.rLiveDead;
            CoefficientA = obj.CoefficientA;
            CoefficientB = obj.CoefficientB;
            CoefficientC = obj.CoefficientC;
            TotalTrees = obj.TotalTrees;
            MeanSE = obj.MeanSE;
            Rsquared = obj.Rsquared;
            RegressModel = obj.RegressModel;
            rMinDbh = obj.rMinDbh;
            rMaxDbh = obj.rMaxDbh;
        }
    }
    [Table("LogMatrix")]
    public partial class LogMatrixDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogMatrixDO()
        {
            _validator = new RowValidator();
        }

        public LogMatrixDO() { }

        public LogMatrixDO(LogMatrixDO obj) : this()
        {
            SetValues(obj);
        }

        public LogMatrixDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _reportnumber;
        [XmlElement]
        [Field(Name = "ReportNumber")]
        public virtual String ReportNumber
        {
            get
            {
                return _reportnumber;
            }
            set
            {
                if (_reportnumber == value) { return; }
                _reportnumber = value;
                this.ValidateProperty(LOGMATRIX.REPORTNUMBER, _reportnumber);
                this.NotifyPropertyChanged(LOGMATRIX.REPORTNUMBER);
            }
        }
        private String _gradedescription;
        [XmlElement]
        [Field(Name = "GradeDescription")]
        public virtual String GradeDescription
        {
            get
            {
                return _gradedescription;
            }
            set
            {
                if (_gradedescription == value) { return; }
                _gradedescription = value;
                this.ValidateProperty(LOGMATRIX.GRADEDESCRIPTION, _gradedescription);
                this.NotifyPropertyChanged(LOGMATRIX.GRADEDESCRIPTION);
            }
        }
        private String _logsortdescription;
        [XmlElement]
        [Field(Name = "LogSortDescription")]
        public virtual String LogSortDescription
        {
            get
            {
                return _logsortdescription;
            }
            set
            {
                if (_logsortdescription == value) { return; }
                _logsortdescription = value;
                this.ValidateProperty(LOGMATRIX.LOGSORTDESCRIPTION, _logsortdescription);
                this.NotifyPropertyChanged(LOGMATRIX.LOGSORTDESCRIPTION);
            }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(LOGMATRIX.SPECIES, _species);
                this.NotifyPropertyChanged(LOGMATRIX.SPECIES);
            }
        }
        private String _loggrade1;
        [XmlElement]
        [Field(Name = "LogGrade1")]
        public virtual String LogGrade1
        {
            get
            {
                return _loggrade1;
            }
            set
            {
                if (_loggrade1 == value) { return; }
                _loggrade1 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE1, _loggrade1);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE1);
            }
        }
        private String _loggrade2;
        [XmlElement]
        [Field(Name = "LogGrade2")]
        public virtual String LogGrade2
        {
            get
            {
                return _loggrade2;
            }
            set
            {
                if (_loggrade2 == value) { return; }
                _loggrade2 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE2, _loggrade2);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE2);
            }
        }
        private String _loggrade3;
        [XmlElement]
        [Field(Name = "LogGrade3")]
        public virtual String LogGrade3
        {
            get
            {
                return _loggrade3;
            }
            set
            {
                if (_loggrade3 == value) { return; }
                _loggrade3 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE3, _loggrade3);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE3);
            }
        }
        private String _loggrade4;
        [XmlElement]
        [Field(Name = "LogGrade4")]
        public virtual String LogGrade4
        {
            get
            {
                return _loggrade4;
            }
            set
            {
                if (_loggrade4 == value) { return; }
                _loggrade4 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE4, _loggrade4);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE4);
            }
        }
        private String _loggrade5;
        [XmlElement]
        [Field(Name = "LogGrade5")]
        public virtual String LogGrade5
        {
            get
            {
                return _loggrade5;
            }
            set
            {
                if (_loggrade5 == value) { return; }
                _loggrade5 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE5, _loggrade5);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE5);
            }
        }
        private String _loggrade6;
        [XmlElement]
        [Field(Name = "LogGrade6")]
        public virtual String LogGrade6
        {
            get
            {
                return _loggrade6;
            }
            set
            {
                if (_loggrade6 == value) { return; }
                _loggrade6 = value;
                this.ValidateProperty(LOGMATRIX.LOGGRADE6, _loggrade6);
                this.NotifyPropertyChanged(LOGMATRIX.LOGGRADE6);
            }
        }
        private String _sedlimit;
        [XmlElement]
        [Field(Name = "SEDlimit")]
        public virtual String SEDlimit
        {
            get
            {
                return _sedlimit;
            }
            set
            {
                if (_sedlimit == value) { return; }
                _sedlimit = value;
                this.ValidateProperty(LOGMATRIX.SEDLIMIT, _sedlimit);
                this.NotifyPropertyChanged(LOGMATRIX.SEDLIMIT);
            }
        }
        private Double _sedminimum = 0.0;
        [XmlElement]
        [Field(Name = "SEDminimum")]
        public virtual Double SEDminimum
        {
            get
            {
                return _sedminimum;
            }
            set
            {
                if (Math.Abs(_sedminimum - value) < Double.Epsilon) { return; }
                _sedminimum = value;
                this.ValidateProperty(LOGMATRIX.SEDMINIMUM, _sedminimum);
                this.NotifyPropertyChanged(LOGMATRIX.SEDMINIMUM);
            }
        }
        private Double _sedmaximum = 0.0;
        [XmlElement]
        [Field(Name = "SEDmaximum")]
        public virtual Double SEDmaximum
        {
            get
            {
                return _sedmaximum;
            }
            set
            {
                if (Math.Abs(_sedmaximum - value) < Double.Epsilon) { return; }
                _sedmaximum = value;
                this.ValidateProperty(LOGMATRIX.SEDMAXIMUM, _sedmaximum);
                this.NotifyPropertyChanged(LOGMATRIX.SEDMAXIMUM);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("ReportNumber", this.ReportNumber) && isValid;
            isValid = ValidateProperty("GradeDescription", this.GradeDescription) && isValid;
            isValid = ValidateProperty("LogSortDescription", this.LogSortDescription) && isValid;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("LogGrade1", this.LogGrade1) && isValid;
            isValid = ValidateProperty("LogGrade2", this.LogGrade2) && isValid;
            isValid = ValidateProperty("LogGrade3", this.LogGrade3) && isValid;
            isValid = ValidateProperty("LogGrade4", this.LogGrade4) && isValid;
            isValid = ValidateProperty("LogGrade5", this.LogGrade5) && isValid;
            isValid = ValidateProperty("LogGrade6", this.LogGrade6) && isValid;
            isValid = ValidateProperty("SEDlimit", this.SEDlimit) && isValid;
            isValid = ValidateProperty("SEDminimum", this.SEDminimum) && isValid;
            isValid = ValidateProperty("SEDmaximum", this.SEDmaximum) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogMatrixDO);
        }

        public void SetValues(LogMatrixDO obj)
        {
            if (obj == null) { return; }
            ReportNumber = obj.ReportNumber;
            GradeDescription = obj.GradeDescription;
            LogSortDescription = obj.LogSortDescription;
            Species = obj.Species;
            LogGrade1 = obj.LogGrade1;
            LogGrade2 = obj.LogGrade2;
            LogGrade3 = obj.LogGrade3;
            LogGrade4 = obj.LogGrade4;
            LogGrade5 = obj.LogGrade5;
            LogGrade6 = obj.LogGrade6;
            SEDlimit = obj.SEDlimit;
            SEDminimum = obj.SEDminimum;
            SEDmaximum = obj.SEDmaximum;
        }
    }
    #endregion
    #region Settings Tables
    [Table("TreeDefaultValueTreeAuditValue")]
    public partial class TreeDefaultValueTreeAuditValueDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeDefaultValueTreeAuditValueDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("TreeAuditValue_CN", "TreeDefaultValueTreeAuditValue", "TreeAuditValue_CN is Required"));
            _validator.Add(new NotNullRule("TreeDefaultValue_CN", "TreeDefaultValueTreeAuditValue", "TreeDefaultValue_CN is Required"));
        }

        public TreeDefaultValueTreeAuditValueDO() { }

        public TreeDefaultValueTreeAuditValueDO(TreeDefaultValueTreeAuditValueDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeDefaultValueTreeAuditValueDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _treeauditvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeAuditValue_CN")]
        public virtual long? TreeAuditValue_CN
        {
            get
            {

                if (_treeauditvalue != null)
                {
                    return _treeauditvalue.TreeAuditValue_CN;
                }
                return _treeauditvalue_cn;
            }
            set
            {
                if (_treeauditvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treeauditvalue = null; }
                _treeauditvalue_cn = value;
                this.ValidateProperty(TREEDEFAULTVALUETREEAUDITVALUE.TREEAUDITVALUE_CN, _treeauditvalue_cn);
                this.NotifyPropertyChanged(TREEDEFAULTVALUETREEAUDITVALUE.TREEAUDITVALUE_CN);
            }
        }
        public virtual TreeAuditValueDO GetTreeAuditValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeAuditValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeAuditValueDO>(this.TreeAuditValue_CN);
            //return DAL.ReadSingleRow<TreeAuditValueDO>(TREEAUDITVALUE._NAME, this.TreeAuditValue_CN);
        }

        private TreeAuditValueDO _treeauditvalue = null;
        [XmlIgnore]
        public TreeAuditValueDO TreeAuditValue
        {
            get
            {
                if (_treeauditvalue == null)
                {
                    _treeauditvalue = GetTreeAuditValue();
                }
                return _treeauditvalue;
            }
            set
            {
                if (_treeauditvalue == value) { return; }
                _treeauditvalue = value;
                TreeAuditValue_CN = (value != null) ? value.TreeAuditValue_CN : null;
            }
        }
        private long? _treedefaultvalue_cn;
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN
        {
            get
            {

                if (_treedefaultvalue != null)
                {
                    return _treedefaultvalue.TreeDefaultValue_CN;
                }
                return _treedefaultvalue_cn;
            }
            set
            {
                if (_treedefaultvalue_cn == value) { return; }
                if (value == null || value.Value == 0) { _treedefaultvalue = null; }
                _treedefaultvalue_cn = value;
                this.ValidateProperty(TREEDEFAULTVALUETREEAUDITVALUE.TREEDEFAULTVALUE_CN, _treedefaultvalue_cn);
                this.NotifyPropertyChanged(TREEDEFAULTVALUETREEAUDITVALUE.TREEDEFAULTVALUE_CN);
            }
        }
        public virtual TreeDefaultValueDO GetTreeDefaultValue()
        {
            if (DAL == null) { return null; }
            if (this.TreeDefaultValue_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<TreeDefaultValueDO>(this.TreeDefaultValue_CN);
            //return DAL.ReadSingleRow<TreeDefaultValueDO>(TREEDEFAULTVALUE._NAME, this.TreeDefaultValue_CN);
        }

        private TreeDefaultValueDO _treedefaultvalue = null;
        [XmlIgnore]
        public TreeDefaultValueDO TreeDefaultValue
        {
            get
            {
                if (_treedefaultvalue == null)
                {
                    _treedefaultvalue = GetTreeDefaultValue();
                }
                return _treedefaultvalue;
            }
            set
            {
                if (_treedefaultvalue == value) { return; }
                _treedefaultvalue = value;
                TreeDefaultValue_CN = (value != null) ? value.TreeDefaultValue_CN : null;
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TreeAuditValue_CN", this.TreeAuditValue_CN) && isValid;
            isValid = ValidateProperty("TreeDefaultValue_CN", this.TreeDefaultValue_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeDefaultValueTreeAuditValueDO);
        }

        public void SetValues(TreeDefaultValueTreeAuditValueDO obj)
        {
            if (obj == null) { return; }
        }
    }
    [Table("TreeAuditValue")]
    public partial class TreeAuditValueDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeAuditValueDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Field", "TreeAuditValue", "Field is Required"));
        }

        public TreeAuditValueDO() { }

        public TreeAuditValueDO(TreeAuditValueDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeAuditValueDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "TreeAuditValue_CN")]
        public Int64? TreeAuditValue_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _field;
        [XmlElement]
        [Field(Name = "Field")]
        public virtual String Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value) { return; }
                _field = value;
                this.ValidateProperty(TREEAUDITVALUE.FIELD, _field);
                this.NotifyPropertyChanged(TREEAUDITVALUE.FIELD);
            }
        }
        private float _min = 0.0f;
        [XmlElement]
        [Field(Name = "Min")]
        public virtual float Min
        {
            get
            {
                return _min;
            }
            set
            {
                if (Math.Abs(_min - value) < float.Epsilon) { return; }
                _min = value;
                this.ValidateProperty(TREEAUDITVALUE.MIN, _min);
                this.NotifyPropertyChanged(TREEAUDITVALUE.MIN);
            }
        }
        private float _max = 0.0f;
        [XmlElement]
        [Field(Name = "Max")]
        public virtual float Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (Math.Abs(_max - value) < float.Epsilon) { return; }
                _max = value;
                this.ValidateProperty(TREEAUDITVALUE.MAX, _max);
                this.NotifyPropertyChanged(TREEAUDITVALUE.MAX);
            }
        }
        private String _valueset;
        [XmlElement]
        [Field(Name = "ValueSet")]
        public virtual String ValueSet
        {
            get
            {
                return _valueset;
            }
            set
            {
                if (_valueset == value) { return; }
                _valueset = value;
                this.ValidateProperty(TREEAUDITVALUE.VALUESET, _valueset);
                this.NotifyPropertyChanged(TREEAUDITVALUE.VALUESET);
            }
        }
        private bool _required = false;
        [XmlElement]
        [Field(Name = "Required")]
        public virtual bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                if (_required == value) { return; }
                _required = value;
                this.ValidateProperty(TREEAUDITVALUE.REQUIRED, _required);
                this.NotifyPropertyChanged(TREEAUDITVALUE.REQUIRED);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Field", this.Field) && isValid;
            isValid = ValidateProperty("Min", this.Min) && isValid;
            isValid = ValidateProperty("Max", this.Max) && isValid;
            isValid = ValidateProperty("ValueSet", this.ValueSet) && isValid;
            isValid = ValidateProperty("Required", this.Required) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeAuditValueDO);
        }

        public void SetValues(TreeAuditValueDO obj)
        {
            if (obj == null) { return; }
            Field = obj.Field;
            Min = obj.Min;
            Max = obj.Max;
            ValueSet = obj.ValueSet;
            Required = obj.Required;
        }
    }
    [Table("LogGradeAuditRule")]
    public partial class LogGradeAuditRuleDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogGradeAuditRuleDO()
        {
            _validator = new RowValidator();
        }

        public LogGradeAuditRuleDO() { }

        public LogGradeAuditRuleDO(LogGradeAuditRuleDO obj) : this()
        {
            SetValues(obj);
        }

        public LogGradeAuditRuleDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _species;
        [XmlElement]
        [Field(Name = "Species")]
        public virtual String Species
        {
            get
            {
                return _species;
            }
            set
            {
                if (_species == value) { return; }
                _species = value;
                this.ValidateProperty(LOGGRADEAUDITRULE.SPECIES, _species);
                this.NotifyPropertyChanged(LOGGRADEAUDITRULE.SPECIES);
            }
        }
        private float _defectmax = 0.0f;
        [XmlElement]
        [Field(Name = "DefectMax")]
        public virtual float DefectMax
        {
            get
            {
                return _defectmax;
            }
            set
            {
                if (Math.Abs(_defectmax - value) < float.Epsilon) { return; }
                _defectmax = value;
                this.ValidateProperty(LOGGRADEAUDITRULE.DEFECTMAX, _defectmax);
                this.NotifyPropertyChanged(LOGGRADEAUDITRULE.DEFECTMAX);
            }
        }
        private String _validgrades;
        [XmlElement]
        [Field(Name = "ValidGrades")]
        public virtual String ValidGrades
        {
            get
            {
                return _validgrades;
            }
            set
            {
                if (_validgrades == value) { return; }
                _validgrades = value;
                this.ValidateProperty(LOGGRADEAUDITRULE.VALIDGRADES, _validgrades);
                this.NotifyPropertyChanged(LOGGRADEAUDITRULE.VALIDGRADES);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Species", this.Species) && isValid;
            isValid = ValidateProperty("DefectMax", this.DefectMax) && isValid;
            isValid = ValidateProperty("ValidGrades", this.ValidGrades) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogGradeAuditRuleDO);
        }

        public void SetValues(LogGradeAuditRuleDO obj)
        {
            if (obj == null) { return; }
            Species = obj.Species;
            DefectMax = obj.DefectMax;
            ValidGrades = obj.ValidGrades;
        }
    }
    [Table("LogFieldSetup")]
    public partial class LogFieldSetupDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogFieldSetupDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "LogFieldSetup", "Stratum_CN is Required"));
            _validator.Add(new NotNullRule("Field", "LogFieldSetup", "Field is Required"));
        }

        public LogFieldSetupDO() { }

        public LogFieldSetupDO(LogFieldSetupDO obj) : this()
        {
            SetValues(obj);
        }

        public LogFieldSetupDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(LOGFIELDSETUP.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(LOGFIELDSETUP.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private String _field;
        [XmlElement]
        [Field(Name = "Field")]
        public virtual String Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value) { return; }
                _field = value;
                this.ValidateProperty(LOGFIELDSETUP.FIELD, _field);
                this.NotifyPropertyChanged(LOGFIELDSETUP.FIELD);
            }
        }
        private Int64 _fieldorder;
        [XmlElement]
        [Field(Name = "FieldOrder")]
        public virtual Int64 FieldOrder
        {
            get
            {
                return _fieldorder;
            }
            set
            {
                if (_fieldorder == value) { return; }
                _fieldorder = value;
                this.ValidateProperty(LOGFIELDSETUP.FIELDORDER, _fieldorder);
                this.NotifyPropertyChanged(LOGFIELDSETUP.FIELDORDER);
            }
        }
        private String _columntype;
        [XmlElement]
        [Field(Name = "ColumnType")]
        public virtual String ColumnType
        {
            get
            {
                return _columntype;
            }
            set
            {
                if (_columntype == value) { return; }
                _columntype = value;
                this.ValidateProperty(LOGFIELDSETUP.COLUMNTYPE, _columntype);
                this.NotifyPropertyChanged(LOGFIELDSETUP.COLUMNTYPE);
            }
        }
        private String _heading;
        [XmlElement]
        [Field(Name = "Heading")]
        public virtual String Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                if (_heading == value) { return; }
                _heading = value;
                this.ValidateProperty(LOGFIELDSETUP.HEADING, _heading);
                this.NotifyPropertyChanged(LOGFIELDSETUP.HEADING);
            }
        }
        private float _width = 0.0f;
        [XmlElement]
        [Field(Name = "Width")]
        public virtual float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (Math.Abs(_width - value) < float.Epsilon) { return; }
                _width = value;
                this.ValidateProperty(LOGFIELDSETUP.WIDTH, _width);
                this.NotifyPropertyChanged(LOGFIELDSETUP.WIDTH);
            }
        }
        private String _format;
        [XmlElement]
        [Field(Name = "Format")]
        public virtual String Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format == value) { return; }
                _format = value;
                this.ValidateProperty(LOGFIELDSETUP.FORMAT, _format);
                this.NotifyPropertyChanged(LOGFIELDSETUP.FORMAT);
            }
        }
        private String _behavior;
        [XmlElement]
        [Field(Name = "Behavior")]
        public virtual String Behavior
        {
            get
            {
                return _behavior;
            }
            set
            {
                if (_behavior == value) { return; }
                _behavior = value;
                this.ValidateProperty(LOGFIELDSETUP.BEHAVIOR, _behavior);
                this.NotifyPropertyChanged(LOGFIELDSETUP.BEHAVIOR);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Field", this.Field) && isValid;
            isValid = ValidateProperty("FieldOrder", this.FieldOrder) && isValid;
            isValid = ValidateProperty("ColumnType", this.ColumnType) && isValid;
            isValid = ValidateProperty("Heading", this.Heading) && isValid;
            isValid = ValidateProperty("Width", this.Width) && isValid;
            isValid = ValidateProperty("Format", this.Format) && isValid;
            isValid = ValidateProperty("Behavior", this.Behavior) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogFieldSetupDO);
        }

        public void SetValues(LogFieldSetupDO obj)
        {
            if (obj == null) { return; }
            Field = obj.Field;
            FieldOrder = obj.FieldOrder;
            ColumnType = obj.ColumnType;
            Heading = obj.Heading;
            Width = obj.Width;
            Format = obj.Format;
            Behavior = obj.Behavior;
        }
    }
    [Table("TreeFieldSetup")]
    public partial class TreeFieldSetupDO : DataObject
    {
        private static RowValidator _validator;

        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeFieldSetupDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Stratum_CN", "TreeFieldSetup", "Stratum_CN is Required"));
            _validator.Add(new NotNullRule("Field", "TreeFieldSetup", "Field is Required"));
        }

        public TreeFieldSetupDO() { }

        public TreeFieldSetupDO(TreeFieldSetupDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeFieldSetupDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _stratum_cn;
        [XmlIgnore]
        [Field(Name = "Stratum_CN")]
        public virtual long? Stratum_CN
        {
            get
            {

                if (_stratum != null)
                {
                    return _stratum.Stratum_CN;
                }
                return _stratum_cn;
            }
            set
            {
                if (_stratum_cn == value) { return; }
                if (value == null || value.Value == 0) { _stratum = null; }
                _stratum_cn = value;
                this.ValidateProperty(TREEFIELDSETUP.STRATUM_CN, _stratum_cn);
                this.NotifyPropertyChanged(TREEFIELDSETUP.STRATUM_CN);
            }
        }
        public virtual StratumDO GetStratum()
        {
            if (DAL == null) { return null; }
            if (this.Stratum_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<StratumDO>(this.Stratum_CN);
            //return DAL.ReadSingleRow<StratumDO>(STRATUM._NAME, this.Stratum_CN);
        }

        private StratumDO _stratum = null;
        [XmlIgnore]
        public StratumDO Stratum
        {
            get
            {
                if (_stratum == null)
                {
                    _stratum = GetStratum();
                }
                return _stratum;
            }
            set
            {
                if (_stratum == value) { return; }
                _stratum = value;
                Stratum_CN = (value != null) ? value.Stratum_CN : null;
            }
        }
        private String _field;
        [XmlElement]
        [Field(Name = "Field")]
        public virtual String Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value) { return; }
                _field = value;
                this.ValidateProperty(TREEFIELDSETUP.FIELD, _field);
                this.NotifyPropertyChanged(TREEFIELDSETUP.FIELD);
            }
        }
        private Int64 _fieldorder;
        [XmlElement]
        [Field(Name = "FieldOrder")]
        public virtual Int64 FieldOrder
        {
            get
            {
                return _fieldorder;
            }
            set
            {
                if (_fieldorder == value) { return; }
                _fieldorder = value;
                this.ValidateProperty(TREEFIELDSETUP.FIELDORDER, _fieldorder);
                this.NotifyPropertyChanged(TREEFIELDSETUP.FIELDORDER);
            }
        }
        private String _columntype;
        [XmlElement]
        [Field(Name = "ColumnType")]
        public virtual String ColumnType
        {
            get
            {
                return _columntype;
            }
            set
            {
                if (_columntype == value) { return; }
                _columntype = value;
                this.ValidateProperty(TREEFIELDSETUP.COLUMNTYPE, _columntype);
                this.NotifyPropertyChanged(TREEFIELDSETUP.COLUMNTYPE);
            }
        }
        private String _heading;
        [XmlElement]
        [Field(Name = "Heading")]
        public virtual String Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                if (_heading == value) { return; }
                _heading = value;
                this.ValidateProperty(TREEFIELDSETUP.HEADING, _heading);
                this.NotifyPropertyChanged(TREEFIELDSETUP.HEADING);
            }
        }
        private float _width = 0.0f;
        [XmlElement]
        [Field(Name = "Width")]
        public virtual float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (Math.Abs(_width - value) < float.Epsilon) { return; }
                _width = value;
                this.ValidateProperty(TREEFIELDSETUP.WIDTH, _width);
                this.NotifyPropertyChanged(TREEFIELDSETUP.WIDTH);
            }
        }
        private String _format;
        [XmlElement]
        [Field(Name = "Format")]
        public virtual String Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format == value) { return; }
                _format = value;
                this.ValidateProperty(TREEFIELDSETUP.FORMAT, _format);
                this.NotifyPropertyChanged(TREEFIELDSETUP.FORMAT);
            }
        }
        private String _behavior;
        [XmlElement]
        [Field(Name = "Behavior")]
        public virtual String Behavior
        {
            get
            {
                return _behavior;
            }
            set
            {
                if (_behavior == value) { return; }
                _behavior = value;
                this.ValidateProperty(TREEFIELDSETUP.BEHAVIOR, _behavior);
                this.NotifyPropertyChanged(TREEFIELDSETUP.BEHAVIOR);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Field", this.Field) && isValid;
            isValid = ValidateProperty("FieldOrder", this.FieldOrder) && isValid;
            isValid = ValidateProperty("ColumnType", this.ColumnType) && isValid;
            isValid = ValidateProperty("Heading", this.Heading) && isValid;
            isValid = ValidateProperty("Width", this.Width) && isValid;
            isValid = ValidateProperty("Format", this.Format) && isValid;
            isValid = ValidateProperty("Behavior", this.Behavior) && isValid;
            isValid = ValidateProperty("Stratum_CN", this.Stratum_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeFieldSetupDO);
        }

        public void SetValues(TreeFieldSetupDO obj)
        {
            if (obj == null) { return; }
            Field = obj.Field;
            FieldOrder = obj.FieldOrder;
            ColumnType = obj.ColumnType;
            Heading = obj.Heading;
            Width = obj.Width;
            Format = obj.Format;
            Behavior = obj.Behavior;
        }
    }
    [Table("LogFieldSetupDefault")]
    public partial class LogFieldSetupDefaultDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LogFieldSetupDefaultDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Field", "LogFieldSetupDefault", "Field is Required"));
        }

        public LogFieldSetupDefaultDO() { }

        public LogFieldSetupDefaultDO(LogFieldSetupDefaultDO obj) : this()
        {
            SetValues(obj);
        }

        public LogFieldSetupDefaultDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "LogFieldSetupDefault_CN")]
        public Int64? LogFieldSetupDefault_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _field;
        [XmlElement]
        [Field(Name = "Field")]
        public virtual String Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value) { return; }
                _field = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.FIELD, _field);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.FIELD);
            }
        }
        private String _fieldname;
        [XmlElement]
        [Field(Name = "FieldName")]
        public virtual String FieldName
        {
            get
            {
                return _fieldname;
            }
            set
            {
                if (_fieldname == value) { return; }
                _fieldname = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.FIELDNAME, _fieldname);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.FIELDNAME);
            }
        }
        private Int64 _fieldorder;
        [XmlElement]
        [Field(Name = "FieldOrder")]
        public virtual Int64 FieldOrder
        {
            get
            {
                return _fieldorder;
            }
            set
            {
                if (_fieldorder == value) { return; }
                _fieldorder = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.FIELDORDER, _fieldorder);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.FIELDORDER);
            }
        }
        private String _columntype;
        [XmlElement]
        [Field(Name = "ColumnType")]
        public virtual String ColumnType
        {
            get
            {
                return _columntype;
            }
            set
            {
                if (_columntype == value) { return; }
                _columntype = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.COLUMNTYPE, _columntype);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.COLUMNTYPE);
            }
        }
        private String _heading;
        [XmlElement]
        [Field(Name = "Heading")]
        public virtual String Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                if (_heading == value) { return; }
                _heading = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.HEADING, _heading);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.HEADING);
            }
        }
        private float _width = 0.0f;
        [XmlElement]
        [Field(Name = "Width")]
        public virtual float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (Math.Abs(_width - value) < float.Epsilon) { return; }
                _width = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.WIDTH, _width);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.WIDTH);
            }
        }
        private String _format;
        [XmlElement]
        [Field(Name = "Format")]
        public virtual String Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format == value) { return; }
                _format = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.FORMAT, _format);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.FORMAT);
            }
        }
        private String _behavior;
        [XmlElement]
        [Field(Name = "Behavior")]
        public virtual String Behavior
        {
            get
            {
                return _behavior;
            }
            set
            {
                if (_behavior == value) { return; }
                _behavior = value;
                this.ValidateProperty(LOGFIELDSETUPDEFAULT.BEHAVIOR, _behavior);
                this.NotifyPropertyChanged(LOGFIELDSETUPDEFAULT.BEHAVIOR);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Field", this.Field) && isValid;
            isValid = ValidateProperty("FieldName", this.FieldName) && isValid;
            isValid = ValidateProperty("FieldOrder", this.FieldOrder) && isValid;
            isValid = ValidateProperty("ColumnType", this.ColumnType) && isValid;
            isValid = ValidateProperty("Heading", this.Heading) && isValid;
            isValid = ValidateProperty("Width", this.Width) && isValid;
            isValid = ValidateProperty("Format", this.Format) && isValid;
            isValid = ValidateProperty("Behavior", this.Behavior) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LogFieldSetupDefaultDO);
        }

        public void SetValues(LogFieldSetupDefaultDO obj)
        {
            if (obj == null) { return; }
            Field = obj.Field;
            FieldName = obj.FieldName;
            FieldOrder = obj.FieldOrder;
            ColumnType = obj.ColumnType;
            Heading = obj.Heading;
            Width = obj.Width;
            Format = obj.Format;
            Behavior = obj.Behavior;
        }
    }
    [Table("TreeFieldSetupDefault")]
    public partial class TreeFieldSetupDefaultDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static TreeFieldSetupDefaultDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("Method", "TreeFieldSetupDefault", "Method is Required"));
            _validator.Add(new NotNullRule("Field", "TreeFieldSetupDefault", "Field is Required"));
        }

        public TreeFieldSetupDefaultDO() { }

        public TreeFieldSetupDefaultDO(TreeFieldSetupDefaultDO obj) : this()
        {
            SetValues(obj);
        }

        public TreeFieldSetupDefaultDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "TreeFieldSetupDefault_CN")]
        public Int64? TreeFieldSetupDefault_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _method;
        [XmlElement]
        [Field(Name = "Method")]
        public virtual String Method
        {
            get
            {
                return _method;
            }
            set
            {
                if (_method == value) { return; }
                _method = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.METHOD, _method);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.METHOD);
            }
        }
        private String _field;
        [XmlElement]
        [Field(Name = "Field")]
        public virtual String Field
        {
            get
            {
                return _field;
            }
            set
            {
                if (_field == value) { return; }
                _field = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.FIELD, _field);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.FIELD);
            }
        }
        private String _fieldname;
        [XmlElement]
        [Field(Name = "FieldName")]
        public virtual String FieldName
        {
            get
            {
                return _fieldname;
            }
            set
            {
                if (_fieldname == value) { return; }
                _fieldname = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.FIELDNAME, _fieldname);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.FIELDNAME);
            }
        }
        private Int64 _fieldorder;
        [XmlElement]
        [Field(Name = "FieldOrder")]
        public virtual Int64 FieldOrder
        {
            get
            {
                return _fieldorder;
            }
            set
            {
                if (_fieldorder == value) { return; }
                _fieldorder = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.FIELDORDER, _fieldorder);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.FIELDORDER);
            }
        }
        private String _columntype;
        [XmlElement]
        [Field(Name = "ColumnType")]
        public virtual String ColumnType
        {
            get
            {
                return _columntype;
            }
            set
            {
                if (_columntype == value) { return; }
                _columntype = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.COLUMNTYPE, _columntype);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.COLUMNTYPE);
            }
        }
        private String _heading;
        [XmlElement]
        [Field(Name = "Heading")]
        public virtual String Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                if (_heading == value) { return; }
                _heading = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.HEADING, _heading);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.HEADING);
            }
        }
        private float _width = 0.0f;
        [XmlElement]
        [Field(Name = "Width")]
        public virtual float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (Math.Abs(_width - value) < float.Epsilon) { return; }
                _width = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.WIDTH, _width);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.WIDTH);
            }
        }
        private String _format;
        [XmlElement]
        [Field(Name = "Format")]
        public virtual String Format
        {
            get
            {
                return _format;
            }
            set
            {
                if (_format == value) { return; }
                _format = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.FORMAT, _format);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.FORMAT);
            }
        }
        private String _behavior;
        [XmlElement]
        [Field(Name = "Behavior")]
        public virtual String Behavior
        {
            get
            {
                return _behavior;
            }
            set
            {
                if (_behavior == value) { return; }
                _behavior = value;
                this.ValidateProperty(TREEFIELDSETUPDEFAULT.BEHAVIOR, _behavior);
                this.NotifyPropertyChanged(TREEFIELDSETUPDEFAULT.BEHAVIOR);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Method", this.Method) && isValid;
            isValid = ValidateProperty("Field", this.Field) && isValid;
            isValid = ValidateProperty("FieldName", this.FieldName) && isValid;
            isValid = ValidateProperty("FieldOrder", this.FieldOrder) && isValid;
            isValid = ValidateProperty("ColumnType", this.ColumnType) && isValid;
            isValid = ValidateProperty("Heading", this.Heading) && isValid;
            isValid = ValidateProperty("Width", this.Width) && isValid;
            isValid = ValidateProperty("Format", this.Format) && isValid;
            isValid = ValidateProperty("Behavior", this.Behavior) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as TreeFieldSetupDefaultDO);
        }

        public void SetValues(TreeFieldSetupDefaultDO obj)
        {
            if (obj == null) { return; }
            Method = obj.Method;
            Field = obj.Field;
            FieldName = obj.FieldName;
            FieldOrder = obj.FieldOrder;
            ColumnType = obj.ColumnType;
            Heading = obj.Heading;
            Width = obj.Width;
            Format = obj.Format;
            Behavior = obj.Behavior;
        }
    }
    #endregion
    #region Lookup Tables
    [Table("CruiseMethods")]
    public partial class CruiseMethodsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static CruiseMethodsDO()
        {
            _validator = new RowValidator();
        }

        public CruiseMethodsDO() { }

        public CruiseMethodsDO(CruiseMethodsDO obj) : this()
        {
            SetValues(obj);
        }

        public CruiseMethodsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "CruiseMethods_CN")]
        public Int64? CruiseMethods_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(CRUISEMETHODS.CODE, _code);
                this.NotifyPropertyChanged(CRUISEMETHODS.CODE);
            }
        }
        private String _friendlyvalue;
        [XmlElement]
        [Field(Name = "FriendlyValue")]
        public virtual String FriendlyValue
        {
            get
            {
                return _friendlyvalue;
            }
            set
            {
                if (_friendlyvalue == value) { return; }
                _friendlyvalue = value;
                this.ValidateProperty(CRUISEMETHODS.FRIENDLYVALUE, _friendlyvalue);
                this.NotifyPropertyChanged(CRUISEMETHODS.FRIENDLYVALUE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("FriendlyValue", this.FriendlyValue) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as CruiseMethodsDO);
        }

        public void SetValues(CruiseMethodsDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            FriendlyValue = obj.FriendlyValue;
        }
    }
    [Table("LoggingMethods")]
    public partial class LoggingMethodsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static LoggingMethodsDO()
        {
            _validator = new RowValidator();
        }

        public LoggingMethodsDO() { }

        public LoggingMethodsDO(LoggingMethodsDO obj) : this()
        {
            SetValues(obj);
        }

        public LoggingMethodsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "LoggingMethods_CN")]
        public Int64? LoggingMethods_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(LOGGINGMETHODS.CODE, _code);
                this.NotifyPropertyChanged(LOGGINGMETHODS.CODE);
            }
        }
        private String _friendlyvalue;
        [XmlElement]
        [Field(Name = "FriendlyValue")]
        public virtual String FriendlyValue
        {
            get
            {
                return _friendlyvalue;
            }
            set
            {
                if (_friendlyvalue == value) { return; }
                _friendlyvalue = value;
                this.ValidateProperty(LOGGINGMETHODS.FRIENDLYVALUE, _friendlyvalue);
                this.NotifyPropertyChanged(LOGGINGMETHODS.FRIENDLYVALUE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("FriendlyValue", this.FriendlyValue) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as LoggingMethodsDO);
        }

        public void SetValues(LoggingMethodsDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            FriendlyValue = obj.FriendlyValue;
        }
    }
    [Table("ProductCodes")]
    public partial class ProductCodesDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ProductCodesDO()
        {
            _validator = new RowValidator();
        }

        public ProductCodesDO() { }

        public ProductCodesDO(ProductCodesDO obj) : this()
        {
            SetValues(obj);
        }

        public ProductCodesDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "ProductCodes_CN")]
        public Int64? ProductCodes_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(PRODUCTCODES.CODE, _code);
                this.NotifyPropertyChanged(PRODUCTCODES.CODE);
            }
        }
        private String _friendlyvalue;
        [XmlElement]
        [Field(Name = "FriendlyValue")]
        public virtual String FriendlyValue
        {
            get
            {
                return _friendlyvalue;
            }
            set
            {
                if (_friendlyvalue == value) { return; }
                _friendlyvalue = value;
                this.ValidateProperty(PRODUCTCODES.FRIENDLYVALUE, _friendlyvalue);
                this.NotifyPropertyChanged(PRODUCTCODES.FRIENDLYVALUE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("FriendlyValue", this.FriendlyValue) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ProductCodesDO);
        }

        public void SetValues(ProductCodesDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            FriendlyValue = obj.FriendlyValue;
        }
    }
    [Table("UOMCodes")]
    public partial class UOMCodesDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static UOMCodesDO()
        {
            _validator = new RowValidator();
        }

        public UOMCodesDO() { }

        public UOMCodesDO(UOMCodesDO obj) : this()
        {
            SetValues(obj);
        }

        public UOMCodesDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "UOMCodes_CN")]
        public Int64? UOMCodes_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _code;
        [XmlElement]
        [Field(Name = "Code")]
        public virtual String Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (_code == value) { return; }
                _code = value;
                this.ValidateProperty(UOMCODES.CODE, _code);
                this.NotifyPropertyChanged(UOMCODES.CODE);
            }
        }
        private String _friendlyvalue;
        [XmlElement]
        [Field(Name = "FriendlyValue")]
        public virtual String FriendlyValue
        {
            get
            {
                return _friendlyvalue;
            }
            set
            {
                if (_friendlyvalue == value) { return; }
                _friendlyvalue = value;
                this.ValidateProperty(UOMCODES.FRIENDLYVALUE, _friendlyvalue);
                this.NotifyPropertyChanged(UOMCODES.FRIENDLYVALUE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Code", this.Code) && isValid;
            isValid = ValidateProperty("FriendlyValue", this.FriendlyValue) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as UOMCodesDO);
        }

        public void SetValues(UOMCodesDO obj)
        {
            if (obj == null) { return; }
            Code = obj.Code;
            FriendlyValue = obj.FriendlyValue;
        }
    }
    [Table("Regions")]
    public partial class RegionsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static RegionsDO()
        {
            _validator = new RowValidator();
        }

        public RegionsDO() { }

        public RegionsDO(RegionsDO obj) : this()
        {
            SetValues(obj);
        }

        public RegionsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Region_CN")]
        public Int64? Region_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private Int64 _number;
        [XmlElement]
        [Field(Name = "Number")]
        public virtual Int64 Number
        {
            get
            {
                return _number;
            }
            set
            {
                if (_number == value) { return; }
                _number = value;
                this.ValidateProperty(REGIONS.NUMBER, _number);
                this.NotifyPropertyChanged(REGIONS.NUMBER);
            }
        }
        private String _name;
        [XmlElement]
        [Field(Name = "Name")]
        public virtual String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) { return; }
                _name = value;
                this.ValidateProperty(REGIONS.NAME, _name);
                this.NotifyPropertyChanged(REGIONS.NAME);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Number", this.Number) && isValid;
            isValid = ValidateProperty("Name", this.Name) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as RegionsDO);
        }

        public void SetValues(RegionsDO obj)
        {
            if (obj == null) { return; }
            Number = obj.Number;
            Name = obj.Name;
        }
    }
    [Table("Forests")]
    public partial class ForestsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ForestsDO()
        {
            _validator = new RowValidator();
        }

        public ForestsDO() { }

        public ForestsDO(ForestsDO obj) : this()
        {
            SetValues(obj);
        }

        public ForestsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Forest_CN")]
        public Int64? Forest_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private long? _region_cn;
        [XmlIgnore]
        [Field(Name = "Region_CN")]
        public virtual long? Region_CN
        {
            get
            {

                if (_regions != null)
                {
                    return _regions.Region_CN;
                }
                return _region_cn;
            }
            set
            {
                if (_region_cn == value) { return; }
                if (value == null || value.Value == 0) { _regions = null; }
                _region_cn = value;
                this.ValidateProperty(FORESTS.REGION_CN, _region_cn);
                this.NotifyPropertyChanged(FORESTS.REGION_CN);
            }
        }
        public virtual RegionsDO GetRegions()
        {
            if (DAL == null) { return null; }
            if (this.Region_CN == null) { return null; }//  don't bother reading if identity is null
            return DAL.ReadSingleRow<RegionsDO>(this.Region_CN);
            //return DAL.ReadSingleRow<RegionsDO>(REGIONS._NAME, this.Region_CN);
        }

        private RegionsDO _regions = null;
        [XmlIgnore]
        public RegionsDO Regions
        {
            get
            {
                if (_regions == null)
                {
                    _regions = GetRegions();
                }
                return _regions;
            }
            set
            {
                if (_regions == value) { return; }
                _regions = value;
                Region_CN = (value != null) ? value.Region_CN : null;
            }
        }
        private String _state;
        [XmlElement]
        [Field(Name = "State")]
        public virtual String State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value) { return; }
                _state = value;
                this.ValidateProperty(FORESTS.STATE, _state);
                this.NotifyPropertyChanged(FORESTS.STATE);
            }
        }
        private String _name;
        [XmlElement]
        [Field(Name = "Name")]
        public virtual String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) { return; }
                _name = value;
                this.ValidateProperty(FORESTS.NAME, _name);
                this.NotifyPropertyChanged(FORESTS.NAME);
            }
        }
        private Int64 _number;
        [XmlElement]
        [Field(Name = "Number")]
        public virtual Int64 Number
        {
            get
            {
                return _number;
            }
            set
            {
                if (_number == value) { return; }
                _number = value;
                this.ValidateProperty(FORESTS.NUMBER, _number);
                this.NotifyPropertyChanged(FORESTS.NUMBER);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("State", this.State) && isValid;
            isValid = ValidateProperty("Name", this.Name) && isValid;
            isValid = ValidateProperty("Number", this.Number) && isValid;
            isValid = ValidateProperty("Region_CN", this.Region_CN) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ForestsDO);
        }

        public void SetValues(ForestsDO obj)
        {
            if (obj == null) { return; }
            State = obj.State;
            Name = obj.Name;
            Number = obj.Number;
        }
    }
    #endregion
    #region Utility Tables
    [Table("ErrorLog")]
    public partial class ErrorLogDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ErrorLogDO()
        {
            _validator = new RowValidator();
            _validator.Add(new NotNullRule("TableName", "ErrorLog", "TableName is Required"));
            _validator.Add(new NotNullRule("CN_Number", "ErrorLog", "CN_Number is Required"));
            _validator.Add(new NotNullRule("ColumnName", "ErrorLog", "ColumnName is Required"));
            _validator.Add(new NotNullRule("Level", "ErrorLog", "Level is Required"));
        }

        public ErrorLogDO() { }

        public ErrorLogDO(ErrorLogDO obj) : this()
        {
            SetValues(obj);
        }

        public ErrorLogDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _tablename;
        [XmlElement]
        [Field(Name = "TableName")]
        public virtual String TableName
        {
            get
            {
                return _tablename;
            }
            set
            {
                if (_tablename == value) { return; }
                _tablename = value;
                this.ValidateProperty(ERRORLOG.TABLENAME, _tablename);
                this.NotifyPropertyChanged(ERRORLOG.TABLENAME);
            }
        }
        private Int64 _cn_number;
        [XmlElement]
        [Field(Name = "CN_Number")]
        public virtual Int64 CN_Number
        {
            get
            {
                return _cn_number;
            }
            set
            {
                if (_cn_number == value) { return; }
                _cn_number = value;
                this.ValidateProperty(ERRORLOG.CN_NUMBER, _cn_number);
                this.NotifyPropertyChanged(ERRORLOG.CN_NUMBER);
            }
        }
        private String _columnname;
        [XmlElement]
        [Field(Name = "ColumnName")]
        public virtual String ColumnName
        {
            get
            {
                return _columnname;
            }
            set
            {
                if (_columnname == value) { return; }
                _columnname = value;
                this.ValidateProperty(ERRORLOG.COLUMNNAME, _columnname);
                this.NotifyPropertyChanged(ERRORLOG.COLUMNNAME);
            }
        }
        private String _level;
        [XmlElement]
        [Field(Name = "Level")]
        public virtual String Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (_level == value) { return; }
                _level = value;
                this.ValidateProperty(ERRORLOG.LEVEL, _level);
                this.NotifyPropertyChanged(ERRORLOG.LEVEL);
            }
        }
        private String _message;
        [XmlElement]
        [Field(Name = "Message")]
        public virtual String Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message == value) { return; }
                _message = value;
                this.ValidateProperty(ERRORLOG.MESSAGE, _message);
                this.NotifyPropertyChanged(ERRORLOG.MESSAGE);
            }
        }
        private String _program;
        [XmlElement]
        [Field(Name = "Program")]
        public virtual String Program
        {
            get
            {
                return _program;
            }
            set
            {
                if (_program == value) { return; }
                _program = value;
                this.ValidateProperty(ERRORLOG.PROGRAM, _program);
                this.NotifyPropertyChanged(ERRORLOG.PROGRAM);
            }
        }
        private bool _suppress = false;
        [XmlElement]
        [Field(Name = "Suppress")]
        public virtual bool Suppress
        {
            get
            {
                return _suppress;
            }
            set
            {
                if (_suppress == value) { return; }
                _suppress = value;
                this.ValidateProperty(ERRORLOG.SUPPRESS, _suppress);
                this.NotifyPropertyChanged(ERRORLOG.SUPPRESS);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("TableName", this.TableName) && isValid;
            isValid = ValidateProperty("CN_Number", this.CN_Number) && isValid;
            isValid = ValidateProperty("ColumnName", this.ColumnName) && isValid;
            isValid = ValidateProperty("Level", this.Level) && isValid;
            isValid = ValidateProperty("Message", this.Message) && isValid;
            isValid = ValidateProperty("Program", this.Program) && isValid;
            isValid = ValidateProperty("Suppress", this.Suppress) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ErrorLogDO);
        }

        public void SetValues(ErrorLogDO obj)
        {
            if (obj == null) { return; }
            TableName = obj.TableName;
            CN_Number = obj.CN_Number;
            ColumnName = obj.ColumnName;
            Level = obj.Level;
            Message = obj.Message;
            Program = obj.Program;
            Suppress = obj.Suppress;
        }
    }
    [Table("MessageLog")]
    public partial class MessageLogDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static MessageLogDO()
        {
            _validator = new RowValidator();
        }

        public MessageLogDO() { }

        public MessageLogDO(MessageLogDO obj) : this()
        {
            SetValues(obj);
        }

        public MessageLogDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Message_CN")]
        public Int64? Message_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _program;
        [XmlElement]
        [Field(Name = "Program")]
        public virtual String Program
        {
            get
            {
                return _program;
            }
            set
            {
                if (_program == value) { return; }
                _program = value;
                this.ValidateProperty(MESSAGELOG.PROGRAM, _program);
                this.NotifyPropertyChanged(MESSAGELOG.PROGRAM);
            }
        }
        private String _message;
        [XmlElement]
        [Field(Name = "Message")]
        public virtual String Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message == value) { return; }
                _message = value;
                this.ValidateProperty(MESSAGELOG.MESSAGE, _message);
                this.NotifyPropertyChanged(MESSAGELOG.MESSAGE);
            }
        }
        private String _date;
        [XmlElement]
        [Field(Name = "Date")]
        public virtual String Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date == value) { return; }
                _date = value;
                this.ValidateProperty(MESSAGELOG.DATE, _date);
                this.NotifyPropertyChanged(MESSAGELOG.DATE);
            }
        }
        private String _time;
        [XmlElement]
        [Field(Name = "Time")]
        public virtual String Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time == value) { return; }
                _time = value;
                this.ValidateProperty(MESSAGELOG.TIME, _time);
                this.NotifyPropertyChanged(MESSAGELOG.TIME);
            }
        }
        private String _level;
        [XmlElement]
        [Field(Name = "Level")]
        public virtual String Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (_level == value) { return; }
                _level = value;
                this.ValidateProperty(MESSAGELOG.LEVEL, _level);
                this.NotifyPropertyChanged(MESSAGELOG.LEVEL);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Program", this.Program) && isValid;
            isValid = ValidateProperty("Message", this.Message) && isValid;
            isValid = ValidateProperty("Date", this.Date) && isValid;
            isValid = ValidateProperty("Time", this.Time) && isValid;
            isValid = ValidateProperty("Level", this.Level) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as MessageLogDO);
        }

        public void SetValues(MessageLogDO obj)
        {
            if (obj == null) { return; }
            Program = obj.Program;
            Message = obj.Message;
            Date = obj.Date;
            Time = obj.Time;
            Level = obj.Level;
        }
    }
    [Table("Globals")]
    public partial class GlobalsDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static GlobalsDO()
        {
            _validator = new RowValidator();
        }

        public GlobalsDO() { }

        public GlobalsDO(GlobalsDO obj) : this()
        {
            SetValues(obj);
        }

        public GlobalsDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "RowID")]
        public Int64? RowID
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private String _block;
        [XmlElement]
        [Field(Name = "Block")]
        public virtual String Block
        {
            get
            {
                return _block;
            }
            set
            {
                if (_block == value) { return; }
                _block = value;
                this.ValidateProperty(GLOBALS.BLOCK, _block);
                this.NotifyPropertyChanged(GLOBALS.BLOCK);
            }
        }
        private String _key;
        [XmlElement]
        [Field(Name = "Key")]
        public virtual String Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key == value) { return; }
                _key = value;
                this.ValidateProperty(GLOBALS.KEY, _key);
                this.NotifyPropertyChanged(GLOBALS.KEY);
            }
        }
        private String _value;
        [XmlElement]
        [Field(Name = "Value")]
        public virtual String Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value) { return; }
                _value = value;
                this.ValidateProperty(GLOBALS.VALUE, _value);
                this.NotifyPropertyChanged(GLOBALS.VALUE);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("Block", this.Block) && isValid;
            isValid = ValidateProperty("Key", this.Key) && isValid;
            isValid = ValidateProperty("Value", this.Value) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as GlobalsDO);
        }

        public void SetValues(GlobalsDO obj)
        {
            if (obj == null) { return; }
            Block = obj.Block;
            Key = obj.Key;
            Value = obj.Value;
        }
    }
    [Table("Component")]
    public partial class ComponentDO : DataObject
    {
        private static RowValidator _validator;


        [XmlIgnore]
        public override RowValidator Validator
        {
            get
            {
                return _validator;
            }
        }


        #region Ctor
        static ComponentDO()
        {
            _validator = new RowValidator();
        }

        public ComponentDO() { }

        public ComponentDO(ComponentDO obj) : this()
        {
            SetValues(obj);
        }

        public ComponentDO(DAL DAL) : base(DAL)
        { }
        #endregion
        [XmlIgnore]
        [PrimaryKeyField(Name = "Component_CN")]
        public Int64? Component_CN
        {
            get { return base.rowID; }
            set { base.rowID = value; }
        }
        private Guid? _guid;
        [XmlElement]
        [Field(Name = "GUID")]
        public virtual Guid? GUID
        {
            get
            {
                return _guid;
            }
            set
            {
                if (_guid == value) { return; }
                _guid = value;
                this.ValidateProperty(COMPONENT.GUID, _guid);
                this.NotifyPropertyChanged(COMPONENT.GUID);
            }
        }
        private String _lastmerge;
        [XmlElement]
        [Field(Name = "LastMerge")]
        public virtual String LastMerge
        {
            get
            {
                return _lastmerge;
            }
            set
            {
                if (_lastmerge == value) { return; }
                _lastmerge = value;
                this.ValidateProperty(COMPONENT.LASTMERGE, _lastmerge);
                this.NotifyPropertyChanged(COMPONENT.LASTMERGE);
            }
        }
        private String _filename;
        [XmlElement]
        [Field(Name = "FileName")]
        public virtual String FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                if (_filename == value) { return; }
                _filename = value;
                this.ValidateProperty(COMPONENT.FILENAME, _filename);
                this.NotifyPropertyChanged(COMPONENT.FILENAME);
            }
        }

        protected override bool DoValidate()
        {
            if (ErrorCollection.ErrorsLoaded == false)
            {
                this.ErrorCollection.PopulateErrorList();
            }
            bool isValid = true;
            isValid = ValidateProperty("GUID", this.GUID) && isValid;
            isValid = ValidateProperty("LastMerge", this.LastMerge) && isValid;
            isValid = ValidateProperty("FileName", this.FileName) && isValid;
            return isValid;
        }
        public override void SetValues(DataObject obj)
        {
            this.SetValues(obj as ComponentDO);
        }

        public void SetValues(ComponentDO obj)
        {
            if (obj == null) { return; }
            GUID = obj.GUID;
            LastMerge = obj.LastMerge;
            FileName = obj.FileName;
        }
    }
    #endregion
}




