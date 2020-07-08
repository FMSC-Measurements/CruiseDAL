using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Component")]
 public partial class Component
 {
  [PrimaryKeyField("Component_CN")]
  public Int32? Component_CN { get; set; }

  [Field("GUID")]
  public String GUID { get; set; }

  [Field("LastMerge")]
  public DateTime? LastMerge { get; set; }

  [Field("FileName")]
  public String FileName { get; set; }

 }

}
