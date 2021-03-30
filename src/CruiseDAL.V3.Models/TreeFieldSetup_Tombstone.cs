using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeFieldSetup_Tombstone")]
 public partial class TreeFieldSetup_Tombstone
 {
  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

  [Field("DefaultValueInt")]
  public Int32? DefaultValueInt { get; set; }

  [Field("DefaultValueReal")]
  public Double? DefaultValueReal { get; set; }

  [Field("DefaultValueBool")]
  public Boolean? DefaultValueBool { get; set; }

  [Field("DefaultValueText")]
  public String DefaultValueText { get; set; }

 }

}
