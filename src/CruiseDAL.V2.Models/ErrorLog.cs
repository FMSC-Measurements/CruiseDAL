using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("ErrorLog")]
 public partial class ErrorLog
 {
  [Field("TableName")]
  public String TableName { get; set; }

  [Field("CN_Number")]
  public Int32 CN_Number { get; set; }

  [Field("ColumnName")]
  public String ColumnName { get; set; }

  [Field("Level")]
  public String Level { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("Program")]
  public String Program { get; set; }

  [Field("Suppress")]
  public Boolean? Suppress { get; set; }

 }

}
