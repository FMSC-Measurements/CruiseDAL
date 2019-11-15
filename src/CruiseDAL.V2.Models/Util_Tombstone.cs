using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Util_Tombstone")]
 public partial class Util_Tombstone
 {
  [Field("RecordID")]
  public Int32? RecordID { get; set; }

  [Field("RecordGUID")]
  public String RecordGUID { get; set; }

  [Field("TableName")]
  public String TableName { get; set; }

  [Field("Data")]
  public String Data { get; set; }

  [Field("DeletedDate")]
  public String DeletedDate { get; set; }

 }

}
