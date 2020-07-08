using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("tbl_ErrorLog")]
 public partial class tbl_ErrorLog
 {
  [PrimaryKeyField("RowID")]
  public Int32? RowID { get; set; }

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
