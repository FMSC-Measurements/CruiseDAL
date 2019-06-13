using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogFieldSetup_V3")]
 public partial class LogFieldSetup_V3
 {
  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

  [Field("Heading")]
  public String Heading { get; set; }

  [Field("Width")]
  public Double? Width { get; set; }

 }

}
