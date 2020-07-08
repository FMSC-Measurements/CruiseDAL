using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("TreeFieldSetup")]
 public partial class TreeFieldSetup
 {
  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

  [Field("ColumnType")]
  public String ColumnType { get; set; }

  [Field("Heading")]
  public String Heading { get; set; }

  [Field("Width")]
  public Double? Width { get; set; }

  [Field("Format")]
  public String Format { get; set; }

  [Field("Behavior")]
  public String Behavior { get; set; }

 }

}
