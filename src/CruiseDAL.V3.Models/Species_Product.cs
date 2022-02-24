using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Species_Product")]
 public partial class Species_Product
 {
  [PrimaryKeyField("Species_Product_CN")]
  public Int32? Species_Product_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("ContractSpecies")]
  public String ContractSpecies { get; set; }

 }

}
