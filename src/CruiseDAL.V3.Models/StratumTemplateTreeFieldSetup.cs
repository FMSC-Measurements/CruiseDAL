using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("StratumTemplateTreeFieldSetup")]
 public partial class StratumTemplateTreeFieldSetup
 {
  [PrimaryKeyField("StratumTemplateTreeFieldSetup_CN")]
  public Int32? StratumTemplateTreeFieldSetup_CN { get; set; }

  [Field("StratumTemplateName")]
  public String StratumTemplateName { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

  [Field("IsHidden")]
  public Boolean? IsHidden { get; set; }

  [Field("IsLocked")]
  public Boolean? IsLocked { get; set; }

  [Field("DefaultValueInt")]
  public Int32? DefaultValueInt { get; set; }

  [Field("DefaultValueReal")]
  public Double? DefaultValueReal { get; set; }

  [Field("DefaultValueBool")]
  public Boolean? DefaultValueBool { get; set; }

  [Field("DefaultValueText")]
  public String DefaultValueText { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

 }

}
