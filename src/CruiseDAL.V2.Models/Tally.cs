using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Tally")]
 public partial class Tally
 {
  [PrimaryKeyField("Tally_CN")]
  public Int32? Tally_CN { get; set; }

  [Field("Hotkey")]
  public String Hotkey { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("IndicatorValue")]
  public String IndicatorValue { get; set; }

  [Field("IndicatorType")]
  public String IndicatorType { get; set; }

 }

}
