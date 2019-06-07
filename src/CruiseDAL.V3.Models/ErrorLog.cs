using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("ErrorLog")]
 public partial class ErrorLog
 {
  [Field("RowID")]
  public Object RowID { get; set; }

  [Field("TableName")]
  public Object TableName { get; set; }

  [Field("CN_Number")]
  public Int64 CN_Number { get; set; }

  [Field("ColumnName")]
  public Object ColumnName { get; set; }

  [Field("Level")]
  public Object Level { get; set; }

  [Field("Message")]
  public Object Message { get; set; }

  [Field("Program")]
  public Object Program { get; set; }

  [Field("Suppress")]
  public Object Suppress { get; set; }

 }

}
