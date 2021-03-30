using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Migrators
{
    public class VolumeEquationMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
			return
$@"INSERT INTO {toDbName}.VolumeEquation (
    	CruiseID,
		Species,
		PrimaryProduct,
		VolumeEquationNumber,
		StumpHeight,
		TopDIBPrimary,
		TopDIBSecondary,
		CalcTotal,
		CalcBoard,
		CalcCubic,
		CalcCord,
		CalcTopwood,
		CalcBiomass,
		Trim,
		SegmentationLogic,
		MinLogLengthPrimary,
		MaxLogLengthPrimary,
		MinLogLengthSecondary,
		MaxLogLengthSecondary,
		MinMerchLength,
		Model,
		CommonSpeciesName,
		MerchModFlag,
		EvenOddSegment
) 
SELECT 
    	'{cruiseID}',
		Species,
		PrimaryProduct,
		VolumeEquationNumber,
		StumpHeight,
		TopDIBPrimary,
		TopDIBSecondary,
		CalcTotal,
		CalcBoard,
		CalcCubic,
		CalcCord,
		CalcTopwood,
		CalcBiomass,
		Trim,
		SegmentationLogic,
		MinLogLengthPrimary,
		MaxLogLengthPrimary,
		MinLogLengthSecondary,
		MaxLogLengthSecondary,
		MinMerchLength,
		Model,
		CommonSpeciesName,
		MerchModFlag,
		EvenOddSegment
FROM {fromDbName}.VolumeEquation;";
        }
    }
}
