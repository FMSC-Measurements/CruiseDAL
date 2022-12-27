using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CruiseLog")]
 public partial class CruiseLog
 {
  [PrimaryKeyField("CruiseLog_CN")]
  public Int32? CruiseLog_CN { get; set; }

  [Field("CruiseLogID")]
  public String CruiseLogID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("CuttingUnitID")]
  public String CuttingUnitID { get; set; }

  [Field("StratumID")]
  public String StratumID { get; set; }

  [Field("SampleGroupID")]
  public String SampleGroupID { get; set; }

  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("TallyLedgerID")]
  public String TallyLedgerID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("LogID")]
  public String LogID { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("TableName")]
  public String TableName { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Program")]
  public String Program { get; set; }

  [Field("TimeStamp")]
  public DateTime TimeStamp { get; set; }

  [Field("Level")]
  public String Level { get; set; }

 }

}
