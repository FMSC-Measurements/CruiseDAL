namespace CruiseDAL.DownMigrators
{
    public class VolumeEquationDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.VolumeEquation (
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
FROM {fromDbName}.VolumeEquation
WHERE CruiseID = '{cruiseID}'
;";
        }
    }
}