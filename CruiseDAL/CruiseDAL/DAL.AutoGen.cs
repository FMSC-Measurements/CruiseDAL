
using System;
namespace CruiseDAL
{
    public partial class DAL
    {

        internal const string CURENT_DBVERSION = "2015.01.05";

        //function GetParser
        //factory method that creats an instance of a parser object 
        //for a given data object type. 
        /// <summary>
        /// Retruns a Parser for a given DataObject type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //internal DataParser GetParser(Type T)
        //{
            //object[] attrs = T.GetCustomAttributes(typeof(TableAttribute), true);
            //TableAttribute tattr = (attrs.Length > 0) ? (TableAttribute)attrs[0] : null;
            //if (tattr == null) { throw new InvalidOperationException("No Data Parser exists for DO type " + T.Name); }
//
            //switch (tattr.TableName)
            //{
                //case "Sale":
                //{
                    //return new SaleParser();
                //}
                //case "CuttingUnit":
                //{
                    //return new CuttingUnitParser();
                //}
                //case "Stratum":
                //{
                    //return new StratumParser();
                //}
                //case "CuttingUnitStratum":
                //{
                    //return new CuttingUnitStratumParser();
                //}
                //case "SampleGroup":
                //{
                    //return new SampleGroupParser();
                //}
                //case "SampleGroupTreeDefaultValue":
                //{
                    //return new SampleGroupTreeDefaultValueParser();
                //}
                //case "TreeDefaultValue":
                //{
                    //return new TreeDefaultValueParser();
                //}
                //case "TreeDefaultValueTreeAuditValue":
                //{
                    //return new TreeDefaultValueTreeAuditValueParser();
                //}
                //case "TreeAuditValue":
                //{
                    //return new TreeAuditValueParser();
                //}
                //case "AuditValue":
                //{
                    //return new AuditValueParser();
                //}
                //case "VolumeEquation":
                //{
                    //return new VolumeEquationParser();
                //}
                //case "BiomassEquation":
                //{
                    //return new BiomassEquationParser();
                //}
                //case "MessageLog":
                //{
                    //return new MessageLogParser();
                //}
                //case "Globals":
                //{
                    //return new GlobalsParser();
                //}
                //case "Regression":
                //{
                    //return new RegressionParser();
                //}
                //case "CountTree":
                //{
                    //return new CountTreeParser();
                //}
                //case "Component":
                //{
                    //return new ComponentParser();
                //}
                //case "Tally":
                //{
                    //return new TallyParser();
                //}
                //case "TreeEstimate":
                //{
                    //return new TreeEstimateParser();
                //}
                //case "Tree":
                //{
                    //return new TreeParser();
                //}
                //case "Plot":
                //{
                    //return new PlotParser();
                //}
                //case "LogFieldSetup":
                //{
                    //return new LogFieldSetupParser();
                //}
                //case "TreeFieldSetup":
                //{
                    //return new TreeFieldSetupParser();
                //}
                //case "Log":
                //{
                    //return new LogParser();
                //}
                //case "Reports":
                //{
                    //return new ReportsParser();
                //}
                //case "Stem":
                //{
                    //return new StemParser();
                //}
                //case "ValueEquation":
                //{
                    //return new ValueEquationParser();
                //}
                //case "QualityAdjEquation":
                //{
                    //return new QualityAdjEquationParser();
                //}
                //case "ErrorLog":
                //{
                    //return new ErrorLogParser();
                //}
                //case "TreeCalculatedValues":
                //{
                    //return new TreeCalculatedValuesParser();
                //}
                //case "LCD":
                //{
                    //return new LCDParser();
                //}
                //case "POP":
                //{
                    //return new POPParser();
                //}
                //case "PRO":
                //{
                    //return new PROParser();
                //}
                //case "LogStock":
                //{
                    //return new LogStockParser();
                //}
                //case "SampleGroupStats":
                //{
                    //return new SampleGroupStatsParser();
                //}
                //case "StratumStats":
                //{
                    //return new StratumStatsParser();
                //}
                //case "SampleGroupStatsTreeDefaultValue":
                //{
                    //return new SampleGroupStatsTreeDefaultValueParser();
                //}
                //case "LogFieldSetupDefault":
                //{
                    //return new LogFieldSetupDefaultParser();
                //}
                //case "TreeFieldSetupDefault":
                //{
                    //return new TreeFieldSetupDefaultParser();
                //}
                //case "CruiseMethods":
                //{
                    //return new CruiseMethodsParser();
                //}
                //case "LoggingMethods":
                //{
                    //return new LoggingMethodsParser();
                //}
                //case "ProductCodes":
                //{
                    //return new ProductCodesParser();
                //}
                //case "UOMCodes":
                //{
                    //return new UOMCodesParser();
                //}
                //case "Regions":
                //{
                    //return new RegionsParser();
                //}
                //case "Forests":
                //{
                    //return new ForestsParser();
                //}
                //case "LogMatrix":
                //{
                    //return new LogMatrixParser();
                //}
                //default:
                //{
                    //throw new InvalidOperationException("No Data Parser exists for table " + tattr.TableName);
                //}
                //
            //}
        //}
    
    }
}