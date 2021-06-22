using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("StratumTemplateLogFieldSetup")]
 public partial class StratumTemplateLogFieldSetup
 {
  [PrimaryKeyField("StratumTemplateLogFieldSetup_CN")]
  public Int32? StratumTemplateLogFieldSetup_CN { get; set; }

  [Field("StratumTemplateName")]
  public String StratumTemplateName { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

 }

}
