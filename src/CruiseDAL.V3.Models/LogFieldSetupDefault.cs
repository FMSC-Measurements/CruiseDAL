using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogFieldSetupDefault")]
 public partial class LogFieldSetupDefault
 {
  [Field("LogFieldSetupDefault_CN")]
  public Int64 LogFieldSetupDefault_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int64 FieldOrder { get; set; }

  [Field("ColumnType")]
  public String ColumnType { get; set; }

  [Field("Heading")]
  public String Heading { get; set; }

  [Field("Width")]
  public Double Width { get; set; }

  [Field("Format")]
  public String Format { get; set; }

  [Field("Behavior")]
  public String Behavior { get; set; }

 }

}