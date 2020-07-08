using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Device")]
 public partial class Device
 {
  [Field("DeviceID")]
  public String DeviceID { get; set; }

  [Field("Name")]
  public String Name { get; set; }

 }

}
