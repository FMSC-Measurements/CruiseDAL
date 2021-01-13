using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CruiseMethods")]
 public partial class CruiseMethods
 {
  [PrimaryKeyField("CruiseMethods_CN")]
  public Int32? CruiseMethods_CN { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

  [Field("IsPlotMethod")]
  public Boolean IsPlotMethod { get; set; }

 }

}
