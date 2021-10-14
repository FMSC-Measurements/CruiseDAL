using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Tree_TreeDefaultValue")]
 public partial class Tree_TreeDefaultValue
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

 }

}
