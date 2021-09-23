using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_TallyEntryType")]
 public partial class LK_TallyEntryType
 {
  [PrimaryKeyField("LK_TallyEntryType_CN")]
  public Int32? LK_TallyEntryType_CN { get; set; }

  [Field("EntryType")]
  public String EntryType { get; set; }

 }

}
