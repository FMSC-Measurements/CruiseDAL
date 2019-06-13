using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeFieldSetupDefault")]
 public partial class TreeFieldSetupDefault
 {
  [PrimaryKeyField("TreeFieldSetupDefault_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? TreeFieldSetupDefault_CN { get; set; }

  [Field("Method")]
  public String Method { get; set; }

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
