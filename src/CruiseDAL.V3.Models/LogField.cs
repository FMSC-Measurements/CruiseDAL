using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogField")]
 public partial class LogField
 {
  [PrimaryKeyField("LogField_CN")]
  public Int32? LogField_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("DbType")]
  public String DbType { get; set; }

 }

}
