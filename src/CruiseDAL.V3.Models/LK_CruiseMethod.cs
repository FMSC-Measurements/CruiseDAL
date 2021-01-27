using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_CruiseMethod")]
 public partial class LK_CruiseMethod
 {
  [PrimaryKeyField("LK_CruiseMethod_CN")]
  public Int32? LK_CruiseMethod_CN { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

  [Field("IsPlotMethod")]
  public Boolean IsPlotMethod { get; set; }

 }

}
