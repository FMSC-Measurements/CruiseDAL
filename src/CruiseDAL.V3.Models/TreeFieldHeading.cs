using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeFieldHeading")]
 public partial class TreeFieldHeading
 {
  [PrimaryKeyField("TreeFieldHeading_CN")]
  public Int32? TreeFieldHeading_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Heading")]
  public String Heading { get; set; }

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
