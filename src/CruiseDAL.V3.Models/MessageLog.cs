using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("MessageLog")]
 public partial class MessageLog
 {
  [Field("Message_CN")]
  public Int64 Message_CN { get; set; }

  [Field("Program")]
  public String Program { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("Date")]
  public String Date { get; set; }

  [Field("Time")]
  public String Time { get; set; }

  [Field("Level")]
  public String Level { get; set; }

 }

}
