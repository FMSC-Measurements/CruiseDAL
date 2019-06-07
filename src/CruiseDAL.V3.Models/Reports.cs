using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Reports")]
 public partial class Reports
 {
  [Field("ReportID")]
  public String ReportID { get; set; }

  [Field("Selected")]
  public Boolean Selected { get; set; }

  [Field("Title")]
  public String Title { get; set; }

 }

}
