using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Reports")]
 public partial class Reports
 {
  [Field("ReportID")]
  public String ReportID { get; set; }

  [Field("Selected")]
  public Boolean? Selected { get; set; }

  [Field("Title")]
  public String Title { get; set; }

 }

}
