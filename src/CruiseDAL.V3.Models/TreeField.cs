using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeField")]
 public partial class TreeField
 {
  [PrimaryKeyField("TreeField_CN")]
  public Int32? TreeField_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("DefaultHeading")]
  public String DefaultHeading { get; set; }

  [Field("DbType")]
  public String DbType { get; set; }

  [Field("IsTreeMeasurmentField")]
  public Boolean? IsTreeMeasurmentField { get; set; }

 }

}
